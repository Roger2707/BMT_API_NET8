using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store_API.DTOs.Location;

namespace Store_API.Controllers
{
    [Route("api/location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public LocationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("get-cities")]
        public async Task<IActionResult> GetCities()
        {
            var url = "https://provinces.open-api.vn/api/";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            var cityJson = JsonConvert.DeserializeObject<List<dynamic>>(result);
            var cities = new List<LocationDTO>() { new LocationDTO { Code = 0, Name = "-- Choose Cities --" } };

            foreach (var item in cityJson)
                cities.Add(new LocationDTO { Name = item.name, Code = item.code });

            return Ok(cities);
        }

        [HttpGet("get-districts")]
        public async Task<IActionResult> GetDistricts(string cityCode)
        {
            var url = $"https://provinces.open-api.vn/api/p/{cityCode}?depth=2";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            var cityJson = JsonConvert.DeserializeObject<dynamic>(result);
            var districts = new List<LocationDTO>() { new LocationDTO { Code = 0, Name = "-- Choose Districts --" } };

            foreach (var item in cityJson.districts)
                districts.Add(new LocationDTO { Name = item.name, Code = item.code });

            return Ok(districts);
        }

        [HttpGet("get-wards")]
        public async Task<IActionResult> GetWards(string districtCode)
        {
            var url = $"https://provinces.open-api.vn/api/d/{districtCode}?depth=2";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            var cityJson = JsonConvert.DeserializeObject<dynamic>(result);
            var districts = new List<LocationDTO>() { new LocationDTO { Code = 0, Name = "-- Choose Wards --" } };

            foreach (var item in cityJson.wards)
                districts.Add(new LocationDTO { Name = item.name, Code = item.code });

            return Ok(districts);
        }
    }
}
