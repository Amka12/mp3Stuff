using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Mp3Stuff.Services
{
    class LastFMService
    {
        private static readonly HttpClient _httpClient;
        private readonly string _baseURL = "http://ws.audioscrobbler.com/2.0/";
        private readonly string _apiKey = "b1f4fa74c9f64a8bb21bed37301eda26";

        //public string Album { get; set; }
        //public string Cover { get; set; }

        static LastFMService()
        {
            _httpClient = new HttpClient();
        }
        //public async Task GetAlbumInfoAsync(Track track)
        //{
        //    Album = string.Empty;
        //    try
        //    {
        //        string fullURL = $"{_baseURL}?method=track.getInfo&api_key={_apiKey}&artist={track.Artist}&track={track.Title}";
        //        HttpResponseMessage response = await _httpClient.GetAsync(fullURL);
        //        response.EnsureSuccessStatusCode();
        //        string responseBody = await response.Content.ReadAsStringAsync();
        //        XmlDocument xDoc = new XmlDocument();
        //        //xDoc.Load(fullURL);
        //        xDoc.LoadXml(responseBody);
        //        XmlElement xRoot = xDoc.DocumentElement;
        //        var status = xRoot.GetAttribute("status");
        //        var child = xRoot.FirstChild.SelectSingleNode("album");
        //        Album = child.SelectSingleNode("title").InnerText;
        //        var img = child.SelectNodes("image");
        //        foreach (XmlElement el in img)
        //        {
        //            foreach (XmlAttribute attr in el.Attributes)
        //            {
        //                if (attr.Value != "extralarge") continue;
        //                Cover = el.InnerText;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("\nException Caught!");
        //        Console.WriteLine("Message :{0} ", e.Message);                
        //    }
        //}

        public string GetAlbumInfo(Track track)
        {
            //Album = string.Empty;
            //string result;

            //string fullURL = $"{_baseURL}?method=track.getInfo&api_key={_apiKey}&artist={track.Artist}&track={track.Title}";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load($"{_baseURL}?method=track.getInfo&api_key={_apiKey}&artist={track.Artist}&track={track.Title}");
            XmlElement xRoot = xDoc.DocumentElement;
            var status = xRoot.GetAttribute("status");
            if (!string.Equals(status, "ok")) return "track not found";
            var child = xRoot.FirstChild.SelectSingleNode("album");
            if (child == null) return "album not found";
            else return child.SelectSingleNode("title").InnerText;
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
