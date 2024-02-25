namespace ConsulService2.Services
{
    public interface аIConsulHostedService
    {
        public Task StartAsync();
        public Task StopAsync();
    }
}
