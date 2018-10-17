using System;

namespace Klaesh.Core
{
    public static class ServiceLocator
    {
        private static IServiceLocator _instance;

        public static IServiceLocator Instance
        {
            get { return _instance; }
            set
            {
                if (_instance != null)
                    throw new Exception("Only one ServiceLocator should exist!");
                _instance = value;
            }
        }
    }
}
