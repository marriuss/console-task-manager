using Application.Utils;

namespace Application
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ApplicationBuilder applicationBuilder = new ApplicationBuilder();
            var service = applicationBuilder.GetServiceInstance();
            service.Start();
        }
    }
}
