using System;
using System.Collections.Generic;

namespace Account
{
    public class ServiceLocator : IServiceLocator
    {

        private Dictionary<object, object> _services;

        internal ServiceLocator()
        {
            _services = new Dictionary<object, object>();
            _services.Add(typeof(FirebaseAnalytics), new FirebaseAnalytics());
        }

        public T GetService<T>()
        {
            try
            {
                return (T)_services[typeof(T)];
            }
            catch (KeyNotFoundException)
            {
                throw new Exception("The requested service is not registered");
            }
        }
    }

    public interface IServiceLocator
    {
        T GetService<T>();
    }
}
