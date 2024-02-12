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
        private readonly IConnection _rabbitConnection;
        private readonly IModel _channel;

        public PredictionsController(IPredictionsGenerator predictionServices)
        {
            
            _predictionServices = predictionServices;

            //rabbit
            //var factory = new ConnectionFactory()
            //{
            //    HostName = "localhost",
            //    Port = 9999,
            //    VirtualHost = "/rabbit",
            //    UserName = "rmuser",
            //    Password = "rmpassword"
            //}; 
            //_rabbitConnection = factory.CreateConnection();
            //_channel = _rabbitConnection.CreateModel();

            //_channel.QueueDeclare(queue: "predictionsQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        [HttpGet("/healthCheck")]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        [HttpGet("/prediction")]
        public IActionResult GetPrediction()
        {
            var prediction = _predictionServices.CreatePrediction();

            //rabbit
            //var body = Encoding.UTF8.GetBytes(prediction);
            //_channel.BasicPublish(exchange: "", routingKey: "predictionsQueue", basicProperties: null, body: body);

            return Ok(prediction);
        }
    }
}
