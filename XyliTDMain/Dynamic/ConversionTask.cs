using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using XyliNet;
using XyliTDMain.Static;
using static XyliTDMain.Windows.ConvertConfiguratorWindow;

namespace XyliTDMain.Dynamic
{
    public class ConversionTask
    {
        private static readonly byte[] coreKey = StringToByteArray("687A4852416D736F356B496E62617857");
        private static readonly byte[] metaKey = StringToByteArray("2331346C6A6B5F215C5D2630553C2728");
        public readonly string inputFilePath;
        public readonly string outputFilePath;
        public readonly string? title;
        public MusicInfo MusicInfo;
        public int percentConverted = 0;
        private int lastPercentConverted = 0;
        public SingleCard? UISingleCard;
        private readonly FileStream ncmFile;
        private ConvertConfiguration convertConfiguration;
        public ConversionTask(ConvertConfiguration c) 
        {
            convertConfiguration = c;
            inputFilePath = convertConfiguration.url;
            outputFilePath = convertConfiguration.filePath;
            ncmFile = convertConfiguration.ncmFile;
            MusicInfo = convertConfiguration.MusicInfo;
        }
        public static (MusicInfo,FileStream)Analysis(string inputFilePath)
        {
            MusicInfo musicInfo;
            FileStream ncmFile = new(inputFilePath, FileMode.Open, FileAccess.Read);
    

            byte[] header = ReadBytes(ncmFile, 8);
            if (BitConverter.ToString(header).Replace("-", "") != "4354454E4644414D")
                throw new Exception("Invalid file format.");

            ncmFile.Seek(2, SeekOrigin.Current);
            int keyLength = BitConverter.ToInt32(ReadBytes(ncmFile, 4), 0);
            byte[] keyData = ReadBytes(ncmFile, keyLength);

            int metaLength = BitConverter.ToInt32(ReadBytes(ncmFile, 4), 0);
            byte[] metaData = ReadBytes(ncmFile, metaLength);
            for (int i = 0; i < metaData.Length; i++)
                metaData[i] ^= 0x63;

            metaData = Convert.FromBase64String(Encoding.UTF8.GetString(metaData, 22, metaData.Length - 22));
            byte[] decryptedMetaData = DecryptAES(metaKey, metaData);
            string metaJson = Encoding.UTF8.GetString(decryptedMetaData, 6, decryptedMetaData.Length - 7) + "}";
            JObject meta = JObject.Parse(metaJson);
            musicInfo = new(meta);
            ncmFile.Seek(0, SeekOrigin.Begin);
            return (musicInfo,ncmFile);
        }

        private void GetImage() 
        {
            string url = MusicInfo.albumPic!;
            string imagePath = Path.Combine(WorkDirectory.ImagePath, MusicInfo.musicName! + ".jpg");
            void UpdataImage()
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        BitmapImage imageBitmap = new();
                        imageBitmap.BeginInit();
                        imageBitmap.CacheOption = BitmapCacheOption.OnLoad;
                        imageBitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                        imageBitmap.DecodePixelWidth = 60;
                        imageBitmap.EndInit();
                        imageBitmap.Freeze();
                        UISingleCard!.MusicImage.Source = imageBitmap;
                    }
                    catch { }
                });
            }
            async Task DownloadImage() 
            {
                try 
                {
                    using MemoryStream stream = await Downloader.DownloadAsync(url);
                    using FileStream fileStream = new(imagePath, FileMode.Create, FileAccess.Write);
                    await stream.CopyToAsync(fileStream);
                } catch 
                {
                }
            }
            Task.Run(async () => 
            {
                if (!File.Exists(imagePath))
                {
                    await DownloadImage();
                }
                UpdataImage();
            });
            
        }

        public async Task ConvertAsync()
        {
            GetImage();
            byte[] header = ReadBytes(ncmFile, 8);
            if (BitConverter.ToString(header).Replace("-", "") != "4354454E4644414D")
                throw new Exception("Invalid file format.");

            ncmFile.Seek(2, SeekOrigin.Current);
            int keyLength = BitConverter.ToInt32(ReadBytes(ncmFile, 4), 0);
            byte[] keyData = ReadBytes(ncmFile, keyLength);
            for (int i = 0; i < keyData.Length; i++)
                keyData[i] ^= 0x64;

            byte[] decryptedKeyData = DecryptAES(coreKey, keyData);
            keyData = decryptedKeyData.Skip(17).ToArray();
            keyLength = keyData.Length;

            byte[] keyBox = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();
            int c = 0;
            int lastByte = 0;
            int keyOffset = 0;
            for (int i = 0; i < 256; i++)
            {
                int swap = keyBox[i];
                c = (swap + lastByte + keyData[keyOffset]) & 0xff;
                keyOffset++;
                if (keyOffset >= keyLength)
                    keyOffset = 0;
                keyBox[i] = keyBox[c];
                keyBox[c] = (byte)swap;
                lastByte = c;
            }

            int metaLength = BitConverter.ToInt32(ReadBytes(ncmFile, 4), 0);
            ncmFile.Seek(metaLength, SeekOrigin.Current);

            Application.Current.Dispatcher.Invoke(() =>
            {
                UISingleCard!.TitleLabel.Content = MusicInfo.musicName;
                UISingleCard!.DescriptionLabel.Content = MusicInfo.artistString;
            });

            ncmFile.Seek(9, SeekOrigin.Current);
            int imageSize = BitConverter.ToInt32(ReadBytes(ncmFile, 4), 0);
            byte[] imageData = ReadBytes(ncmFile, imageSize);

            using FileStream outputFs = new(outputFilePath, FileMode.Create, FileAccess.Write);
            byte[] chunk;
            long totalBytes = ncmFile.Length;
            long totalBytesRead = 0;
            long audioDataSize = totalBytes - ncmFile.Position;

            while ((chunk = ReadBytes(ncmFile, 0x8000)).Length > 0)
            {
                for (int i = 0; i < chunk.Length; i++)
                {
                    int j = (i + 1) & 0xff;
                    chunk[i] ^= keyBox[(keyBox[j] + keyBox[(keyBox[j] + j) & 0xff]) & 0xff];
                }
                await outputFs.WriteAsync(chunk);
                totalBytesRead += chunk.Length;
                percentConverted = (int)(totalBytesRead * 100 / audioDataSize);
                if (percentConverted == lastPercentConverted) continue;
                lastPercentConverted = percentConverted;
                Application.Current.Dispatcher.Invoke(() => 
                {
                    UISingleCard!.StateLabel.Content = $"当前已转换：{percentConverted}%";
                    UISingleCard!.ProgressBar.Value = percentConverted;
                });
            }
            
            Application.Current.Dispatcher.Invoke(() => 
            {
                UISingleCard!.StateLabel.Content = "转换完成";
                UISingleCard!.DirButton.IsEnabled = true;
                UISingleCard!.FileButton.IsEnabled = true;
                UISingleCard!.ShowDialogButton.IsEnabled = true;
            });
        }

        private static byte[] ReadBytes(Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            int read = stream.Read(buffer, 0, count);
            if (read < count)
                Array.Resize(ref buffer, read);
            return buffer;
        }
        private static byte[] DecryptAES(byte[] key, byte[] data)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(data, 0, data.Length);
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
