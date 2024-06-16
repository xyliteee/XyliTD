using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XyliNet;

namespace XyliTDMain.Dynamic
{
    public class MusicInfo
    {
        public readonly string? musicId;
        public readonly string? musicName;
        public readonly string[][]? artist;
        public readonly string? albumId;
        public readonly string? album;
        public readonly string? albumPic;
        public readonly string? bitrate;
        public readonly string? format;
        public readonly string artistString;
        public MusicInfo(JObject meta)
        {
            musicId = (string?)meta.GetValue("musicId");
            musicName = (string?)meta.GetValue("musicName");
            albumId = (string?)meta.GetValue("albumId");
            album = (string?)meta.GetValue("album");
            albumPic = (string?)meta.GetValue("albumPic");
            bitrate = (string?)meta.GetValue("bitrate");
            format = (string?)meta.GetValue("format");
            JArray artistArray = (JArray)meta.GetValue("artist")!;
            artist = artistArray.Select(a => a.Select(t => (string?)t).ToArray()).ToArray()!;
            artistString = GetArtistString();
        }

        private string GetArtistString() 
        {
            string artists = string.Empty;
            foreach (string[] artist in artist!)
            {
                artists += $"{artist[0]},";
            }
            artists = artists.Remove(artists.Length - 1);
            return artists;
        }
    }
}
