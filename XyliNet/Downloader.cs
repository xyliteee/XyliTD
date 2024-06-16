using System.IO;
using System;
using System.Diagnostics;

namespace XyliNet
{
    public enum State
    {
        IsWaitting,
        IsDownloading,
        IsError,
        IsCancelled,
        IsSucessful,
    }
    public class Downloader
    {
        private const string AgentDefault = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 Edg/123.0.0.0";
        private readonly HttpClient client = new();
        public long fileSize;
        private long _fileDateHaveAlreadyDownloaded;
        public string errorMessage = string.Empty;
        public Action? action;
        public State State;
        private static readonly Dictionary<string, string> HeaderDefault = new()
        {
            { "User-Agent", AgentDefault },
        };

        public long FileDateHaveAlreadyDownloaded
        {
            get { return _fileDateHaveAlreadyDownloaded; }
            set
            {
                _fileDateHaveAlreadyDownloaded = value;
                action?.Invoke();
            }
        }
        public Downloader()
        {
            ResetHeader();
        }
        public void ResetHeader() {SetHeader(HeaderDefault);}

        public void SetHeader(Dictionary<string,string> header) 
        {
            client.DefaultRequestHeaders.Clear();
            foreach (var headerItem in header) 
            {
                client.DefaultRequestHeaders.Add(headerItem.Key, headerItem.Value);
            }
        }
        public async static Task<MemoryStream> DownloadAsync(string url) {return await DownloadAsync(url, HeaderDefault);}
        public async static Task<MemoryStream> DownloadAsync(string url,Dictionary<string,string> header)
        {
            MemoryStream stream = new();
            using HttpClient client = new();
            foreach (var headerItem in header)
            {
                client.DefaultRequestHeaders.Add(headerItem.Key, headerItem.Value);
            }
            using HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            using Stream originalStream = await response.Content.ReadAsStreamAsync();
            await originalStream.CopyToAsync(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task DownloadToLocalAsync(string url, string path)
        {
            try
            {
                State = State.IsDownloading;
                using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                fileSize = response.Content.Headers.ContentLength ?? 0;
                using Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
                var totalRead = 0L;
                var buffer = new byte[8192];
                var isMoreToRead = true;
                do
                {
                    if (State != State.IsDownloading)
                    {
                        throw new TaskCanceledException();
                    }
                    var read = await contentStream.ReadAsync(buffer);
                    if (read == 0)
                    {
                        isMoreToRead = false;
                    }
                    else
                    {
                        await fileStream.WriteAsync(buffer.AsMemory(0, read));

                        totalRead += read;
                        FileDateHaveAlreadyDownloaded = totalRead;
                    }
                } while (isMoreToRead);
                State = State.IsSucessful;
            }
            catch (TaskCanceledException)
            {
                State = State.IsCancelled;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                State = State.IsError;
            }
            finally 
            {
                FileDateHaveAlreadyDownloaded = 0;
                errorMessage = string.Empty;
            }
        }
        public async Task<MemoryStream> DownloadToMemAsync(string url)
        {
            MemoryStream memoryStream = new();
            try
            {
                State = State.IsDownloading;
                using HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                fileSize = response.Content.Headers.ContentLength ?? 0;
                using Stream contentStream = await response.Content.ReadAsStreamAsync();
                var totalRead = 0L;
                var buffer = new byte[8192];
                var isMoreToRead = true;

                do
                {
                    if (State != State.IsDownloading)
                    {
                        throw new TaskCanceledException();
                    }
                    var read = await contentStream.ReadAsync(buffer);
                    if (read == 0)
                    {
                        isMoreToRead = false;
                    }
                    else
                    {
                        await memoryStream.WriteAsync(buffer.AsMemory(0, read));

                        totalRead += read;
                        FileDateHaveAlreadyDownloaded = totalRead;
                    }
                } while (isMoreToRead);
                State = State.IsSucessful;
            }
            catch (TaskCanceledException)
            {
                State = State.IsCancelled;
            }
            catch (Exception e)
            {
                errorMessage = e.ToString();
                State = State.IsError;
            }
            finally
            {
                FileDateHaveAlreadyDownloaded = 0;
                errorMessage = string.Empty;
            }
            memoryStream.Position = 0;
            return memoryStream;
        }

    }

}
