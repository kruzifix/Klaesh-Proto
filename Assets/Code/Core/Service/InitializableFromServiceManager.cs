using System;

namespace Klaesh.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InitializableFromServiceManager : Attribute
    {
    }
}
