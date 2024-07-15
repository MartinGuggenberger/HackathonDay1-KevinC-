namespace WeatherMap
{
    using System.ComponentModel;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    public class MeteoApi
    {
        public static async Task<GeoData[]?> GetGeoData(string cityPostal, int count)
        {
            string geocodingUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={cityPostal}&count={count}&language=de&format=json";
            using HttpClient client = new HttpClient();
            HttpResponseMessage geocodingResponse = await client.GetAsync(geocodingUrl);
            geocodingResponse.EnsureSuccessStatusCode();
            GeoResult? geoResult = await geocodingResponse.Content.ReadFromJsonAsync<GeoResult>();

            return geoResult?.Results;
        }

        public static async Task<WeatherData?> GetWeatherDataTest1(double latitude, double longitude)
        {
            return await GetWeatherData(latitude, longitude, "hourly=temperature_2m");
        }

        public static async Task<WeatherData?> GetWeatherDataTest2(double latitude, double longitude)
        {
            return await GetWeatherData(latitude, longitude, "current=temperature_2m,relative_humidity_2m&hourly=temperature_2m,rain,showers,snowfall&daily=temperature_2m_max,temperature_2m_min&timezone=Europe%2FBerlin&forecast_days=1");
        }

        public static async Task<WeatherData?> GetWeatherData(double latitude, double longitude, string weatherParams)
        {
            using HttpClient client = new HttpClient();
            string weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latitude.ToString(CultureInfo.InvariantCulture)}&longitude={longitude.ToString(CultureInfo.InvariantCulture)}&{weatherParams}";
            HttpResponseMessage weatherResponse = await client.GetAsync(weatherUrl);
            weatherResponse.EnsureSuccessStatusCode();
            WeatherData? weatherData = await weatherResponse.Content.ReadFromJsonAsync<WeatherData>();
            return weatherData;
        }
    }

    public class GeoResult
    {
        public GeoData[]? Results { get; set; }
    }

    [TypeConverter(typeof(GeoDataTypeConverter))]
    public class GeoData
    {
        public string? Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string[]? PostalCodes { get; set; }
        public string? Country { get; set; }
        public string? Admin1 { get; set; }
        public string? Admin2 { get; set; }
        public string? Admin3 { get; set; }
        public string? Admin4 { get; set; }
    }

    public class WeatherData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public WeatherDataCurrent current { get; set; }
        public WeatherDataHourly hourly { get; set; }
        public WeatherDataDaily daily { get; set; }
    }

    public class WeatherDataHourly
    {
        public DateTime[] time { get; set; }
        public double[] temperature_2m { get; set; }
        public double[] rain { get; set; }
        public double[] showers { get; set; }
        public double[] snowfall { get; set; }
    }

    public class WeatherDataDaily
    {
        public DateTime[] time { get; set; }
        public double[] temperature_2m_max { get; set; }
        public double[] temperature_2m_min { get; set; }
    }

    public class WeatherDataCurrent
    {
        public DateTime time { get; set; }
        public double temperature_2m { get; set; }
        public double relative_humidity_2m { get; set; }
    }

    public class GeoDataTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                var parts = str.Split('|');
                if (parts.Length >= 4)
                {
                    return new GeoData
                    {
                        Name = parts[0].Trim(),
                        Latitude = double.Parse(parts[1].Trim(), CultureInfo.InvariantCulture),
                        Longitude = double.Parse(parts[2].Trim(), CultureInfo.InvariantCulture),
                        PostalCodes = parts[3].Split(',').Select(p => p.Trim()).ToArray(),
                        Country = parts.Length > 4 ? parts[4].Trim() : null,
                        Admin1 = parts.Length > 5 ? parts[5].Trim() : null,
                        Admin2 = parts.Length > 6 ? parts[6].Trim() : null,
                        Admin3 = parts.Length > 7 ? parts[7].Trim() : null,
                        Admin4 = parts.Length > 8 ? parts[8].Trim() : null
                    };
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is GeoData geoData)
            {
                var postalCodes = geoData.PostalCodes != null ? string.Join(",", geoData.PostalCodes) : string.Empty;
                return $"{geoData.Name}|{geoData.Latitude}|{geoData.Longitude}|{postalCodes}|{geoData.Country}|{geoData.Admin1}|{geoData.Admin2}|{geoData.Admin3}|{geoData.Admin4}";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}