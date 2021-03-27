using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mp3Stuff.Services
{
    class LastFMService
    {
        private static readonly HttpClient _httpClient;
        private readonly string _baseURL = "http://ws.audioscrobbler.com/2.0/";
        private readonly string _apiKey = "b1f4fa74c9f64a8bb21bed37301eda26";

        static LastFMService()
        {
            _httpClient = new HttpClient();
        }
        public async Task GetAlbumInfo(Track track)
        {
            try
            {
                string fullURL = $"{_baseURL}?method=track.getInfo&api_key={_apiKey}&artist={track.Artist}&track={track.Title}&format=json";
                HttpResponseMessage response = await _httpClient.GetAsync(fullURL);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                Console.WriteLine(responseBody);
                MessageBox.Show(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                MessageBox.Show(e.Message);
            }
        }
    }
}
