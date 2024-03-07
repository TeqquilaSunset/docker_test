using Microsoft.AspNetCore.SignalR;

namespace ConsulService1.Services
{
    public interface IPredictionsGenerator
    {
        public Task<string> GeneratePrediction();
        public Task AddNewPredictionWordAsync(string newWord);
    }
}
