using Ninject;
using Ninject.Modules;
using Presentation.Interfaces;

namespace Application.Utils
{
    internal class ApplicationBuilder
    {
        private IKernel _kernel;
        private IService _service;

        public ApplicationBuilder()
        {
            NinjectModule bindings = new BindingsBuilder();
            _kernel = new StandardKernel();
            _kernel.Load(bindings);
        }

        public IService GetServiceInstance()
        {
            if (_service == null) 
                InitializeServiceInstance();

            return _service;
        }

        private void InitializeServiceInstance()
        {
            IService service = _kernel.Get<IService>();
            _service = service;
        }
    }
}
