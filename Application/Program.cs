using Application.Utils;

namespace Application
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ApplicationInitializer applicationInitializer = new ApplicationInitializer();
            var application = applicationInitializer.GetApplicationInstance();
            application.Start();
        }
    }
}
