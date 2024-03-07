using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;

namespace ConsulService1.Services
{
    public class PredictionService : IPredictionsGenerator
    {
        private readonly AppDbContext _dbContext;
        private List<string> predictionWordList;
        Random random = new Random();

        public PredictionService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddNewPredictionWordAsync(string newWord)
        {
            await _dbContext.Predictions.AddAsync(new Models.Prediction() { PredictionWord = newWord });
            _dbContext.SaveChanges();
        }

        public async Task<string> GeneratePrediction()
        {
            predictionWordList = await _dbContext.Predictions.Select(x => x.PredictionWord).ToListAsync();
            if (predictionWordList.Count == 0 || predictionWordList == null)
            {
                return "Похоже у нас нет предсказаний";
            }
            var randInt = random.Next(0, predictionWordList.Count);
            var resultWord = predictionWordList[randInt];
            return $"Тебя ждет {resultWord} день.";
        }
    }
}
