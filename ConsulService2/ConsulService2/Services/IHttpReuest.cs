namespace ConsulService2.Services
{
    public interface IHttpReuest
    {
        public Task<string> GetStringPredictionAsync();
    }
}
