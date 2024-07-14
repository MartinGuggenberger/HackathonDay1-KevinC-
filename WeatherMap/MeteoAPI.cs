namespace WeatherMap
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    public class IndexModel
    {

        public IndexModel()
        {

        }

        public async Task<GeoData[]?> GetGeoData()
        {
            string geocodingUrl = $"https://geocoding-api.open-meteo.com/v1/search?name=Rosenheim&count=10&language=de&format=json";
            using HttpClient client = new HttpClient();
            HttpResponseMessage geocodingResponse = await client.GetAsync(geocodingUrl);
            geocodingResponse.EnsureSuccessStatusCode();
            string geocodingResponseBody = await geocodingResponse.Content.ReadAsStringAsync();
            GeoResult? test = await geocodingResponse.Content.ReadFromJsonAsync<GeoResult>();
            
            return test?.Results;
        }

        public async Task OnGet()
        {
            string geocodingUrl = $"https://geocoding-api.open-meteo.com/v1/search?name=Rosenheim&count=1&language=de&format=json";

            using HttpClient client = new HttpClient();
            HttpResponseMessage geocodingResponse = await client.GetAsync(geocodingUrl);
            geocodingResponse.EnsureSuccessStatusCode();
            string geocodingResponseBody = await geocodingResponse.Content.ReadAsStringAsync();
            GeoResult? test = await geocodingResponse.Content.ReadFromJsonAsync<GeoResult>();

            // Hier kannst du die Koordinaten aus der Antwort extrahieren und für den Wetter-API-Aufruf verwenden
            // Beispiel: latitude = 47.8561, longitude = 12.1225
            string weatherUrl = "https://api.open-meteo.com/v1/forecast?latitude=47.8561&longitude=12.1225&hourly=temperature_2m";
            HttpResponseMessage weatherResponse = await client.GetAsync(weatherUrl);
            weatherResponse.EnsureSuccessStatusCode();
            string weatherResponseBody = await weatherResponse.Content.ReadAsStringAsync();
            Console.WriteLine(weatherResponseBody);
        }
    }


    public class GeoResult
    {
        public GeoData[]? Results { get; set; }
    }

    public class GeoData
    {
        public string? Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Country { get; set; }
        public string? Admin1 { get; set; }
        public string? Admin2 { get; set; }
        public string? Admin3 { get; set; }
        public string? Admin4 { get; set; }
    }

}
