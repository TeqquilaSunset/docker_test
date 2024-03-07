using ConsulService1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace ConsulService1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictionsController : Controller
    {
        private readonly IPredictionsGenerator _predictionServices;

        public PredictionsController(IPredictionsGenerator predictionServices)
        {
            
            _predictionServices = predictionServices;

        }

        [HttpGet("/healthCheck")]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        [HttpGet("/prediction")]
        public async Task<IActionResult> GetPrediction()
        {
            var prediction = await _predictionServices.GeneratePrediction();
            return Ok(prediction);
        }

        [HttpPut("/prediction/{word}")]

        public async Task<IActionResult> GetPrediction(string word)
        {
            await _predictionServices.AddNewPredictionWordAsync(word);
            return Ok();
        }
    }
}
