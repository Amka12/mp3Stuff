using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace Mp3Stuff.Services
{
    class LastFMService
    {
        private static readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://ws.audioscrobbler.com/2.0/";
        private readonly string _apiKey = "b1f4fa74c9f64a8bb21bed37301eda26";

        //public string Album { get; set; }
        //public string Cover { get; set; }

        static LastFMService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string?> GetAlbumInfoAsync(Track track)
        {
            if (track is null ||
                string.IsNullOrWhiteSpace(track.Artist) ||
                string.IsNullOrWhiteSpace(track.Title))
            {
                return null;
            }

            // Экранируем параметры
            var artist = HttpUtility.UrlEncode(track.Artist);
            var title = HttpUtility.UrlEncode(track.Title);

            var requestUrl = $"{_baseUrl}?method=track.getInfo&api_key={_apiKey}&artist={artist}&track={title}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                var xDoc = XDocument.Parse(content);

                var root = xDoc.Root;
                if (root?.Attribute("status")?.Value != "ok")
                    return null;

                var albumTitle = root
                    .Element("track")?
                    .Element("album")?
                    .Element("title")?
                    .Value;

                return string.IsNullOrWhiteSpace(albumTitle) ? null : albumTitle;
            }
            catch
            {
                // Желательно логировать исключения
                return null;
            }
        }

        public string GetAlbumInfo(Track track)
        {
            //Album = string.Empty;
            //string result;
            if (track is null) return string.Empty;

            //string fullURL = $"{_baseURL}?method=track.getInfo&api_key={_apiKey}&artist={track.Artist}&track={track.Title}";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load($"{_baseUrl}?method=track.getInfo&api_key={_apiKey}&artist={track.Artist}&track={track.Title}");
            XmlElement xRoot = xDoc.DocumentElement;
            var status = xRoot.GetAttribute("status");
            if (!string.Equals(status, "ok")) return "track not found";
            var child = xRoot.FirstChild.SelectSingleNode("album");
            return child == null ? "album not found" : child.SelectSingleNode("title")?.InnerText;
            //var img = child.SelectNodes("image");
                //foreach (XmlElement el in img)
                //{
                //    foreach (XmlAttribute attr in el.Attributes)
                //    {
                //        if (attr.Value != "extralarge") continue;
                //        Cover = el.InnerText;
                //    }
                //}
        }

    }
}
