namespace ConsulService2.Services
{
    public interface IHttpRequest
    {
        public Task<string> GetStringPredictionAsync();
    }
}
