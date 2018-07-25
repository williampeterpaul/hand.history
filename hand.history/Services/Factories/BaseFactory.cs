using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace hand.history.Services.Factories
{
    public abstract class BaseFactory<T>
    {
        internal IUnityContainer Container { get; private set; }

        internal abstract void Register();

        public T InstantiateService()
        {
            return InstantiateService(new UnityContainer());
        }

        public T InstantiateService(IUnityContainer container)
        {
            if (container == null) container = new UnityContainer();

            Container = container;

            Register();

            return Container.Resolve<T>();
        }
    }
}
