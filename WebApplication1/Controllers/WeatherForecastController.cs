using Microsoft.AspNetCore.Mvc;
using WYF;
using WYF.Form.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    //[Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("api/[controller]/GetWeatherForecast2")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpPost]
        [Route("api/[controller]/GetWeatherForecast")]
        public IEnumerable<WeatherForecast> GetWeatherForecast()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpPost]
        [Route("api/FormBillService/GetConfig")]
        public Task<dynamic> GetConfig(Dictionary<string, object> parameter)
        {
            
            string formId = parameter.GetString("formId");
            string rootPageId = parameter.GetString("rootPageId");
            string pageId = string.Empty;
            if (string.IsNullOrEmpty(rootPageId))
            {
                pageId = "root" + Guid.NewGuid().ToNullString().Replace("-", "");
                parameter.Add("pageId", pageId);
                parameter.Add("rootPageId", pageId);
            }
            parameter.Remove("hasRight");
            parameter.Remove("permissionItemId");
            //var keys= CacheUtil.Instance().GetAllKeys();
            Dictionary<string, object> config = FormConfigFactory.CreateConfig(parameter);
            //string config= formMetadata.GetFormConfig("pc_main_console");
            return Task.FromResult<dynamic>(config);
        }


    }
}
