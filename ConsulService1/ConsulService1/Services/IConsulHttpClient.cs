namespace ConsulService1.Services
{
    public interface IConsulHttpClient
    {
        public Task RegisterServiceAsync();
        public Task DeregisterServiceAsync();
    }
}
