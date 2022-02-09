using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json.Nodes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CurrencyAPI_Test.Controllers
{
    
    [ApiController]
    public class CurrencyController : ControllerBase
    {

        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(ILogger<CurrencyController> logger)
        {
            _logger = logger;
        }

        protected JsonNode? Update()
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri("https://www.cbr-xml-daily.ru/");
                HttpResponseMessage response = client.GetAsync("daily_json.js").Result;
                //response.EnsureSuccessStatusCode();
                var res = response.Content.ReadAsStringAsync().Result;
                return JsonNode.Parse(res);
            }
        }

        [Route("/currencies/{take}/{skip}")]
        [HttpGet]
        public JsonObject[] Get(int take = 10, int skip = 0)
        {            
            var x = Update()?["Valute"].AsObject().ToList().Skip(skip).Take(take);
            var res = x.Select(x => new JsonObject { [x.Key] = JsonNode.Parse(x.Value.AsObject().ToJsonString())}).ToArray();

            return res;
        }

        [Route("/currencie/{id}")]
        [HttpGet]
        public JsonNode? Get(string id)
        {
           

            var x = Update()?["Valute"].AsObject().ToList();
            return  x.Where(n => n.Key == id).FirstOrDefault().Value;
        }

       
    }
}
