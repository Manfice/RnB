using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;
using Ninject.Web.Common;
using Web.Domen.Abstract;
using Web.Domen.Repositorys;

namespace Web.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            _kernel.Bind<IEvents>().To<DbPaty>().InRequestScope();
            _kernel.Bind<IHome>().To<DbHome>().InRequestScope();
            _kernel.Bind<IAuth>().To<DbAuth>().InRequestScope();
            _kernel.Bind<IPhoto>().To<DbPhoto>().InRequestScope();
            _kernel.Bind<ICustomer>().To<DbCustomer>().InRequestScope();
            _kernel.Bind<ICmc>().To<DbCmc>().InRequestScope();
        }
    }
}