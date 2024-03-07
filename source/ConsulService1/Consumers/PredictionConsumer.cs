using ConsulService1.Services;
using Core.Models;
using MassTransit;

namespace ConsulService1.Consumers
{
    public class PredictionConsumer : IConsumer<NewPrediction>
    {
        IPredictionsGenerator _prediction;
        public PredictionConsumer(IPredictionsGenerator prediction)
        {
            _prediction = prediction;
        }

        public async Task Consume(ConsumeContext<NewPrediction> context)
        {
            if(context.Message.Prediction == null)
            {
                return;
            }
            await _prediction.AddNewPredictionWordAsync(context.Message.Prediction);
        }
    }
}
