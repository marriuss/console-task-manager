using Ninject;
using Ninject.Modules;
using Presentation.Interfaces;

namespace Application.Utils
{
    internal class ApplicationInitializer
    {
        private IService _application;

        public IService GetApplicationInstance()
        {
            if (_application == null)
                InitializeApplicationInstance();

            return _application;
        }

        private void InitializeApplicationInstance()
        {
            NinjectModule bindings = new ProjectBilder();
            StandardKernel kernel = new StandardKernel();
            kernel.Load(bindings);
            IService application = kernel.Get<IService>();
            _application = application;
        }
    }
}
