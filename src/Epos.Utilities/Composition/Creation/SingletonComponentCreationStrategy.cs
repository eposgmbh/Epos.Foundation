using System;
using System.Reflection;

namespace Epos.Utilities.Composition
{
    internal sealed class SingletonComponentCreationStrategy : ComponentCreationStrategy
    {
        private object mySingleton;

        protected override object GetInstance(ConstructorInfo constructorInfo, object[] parameterValues) {
            return mySingleton ?? (mySingleton = constructorInfo.Invoke(parameterValues));
        }

        protected override object GetInstance(Delegate factoryMethod) {
            return mySingleton ?? (mySingleton = factoryMethod.DynamicInvoke());
        }
    }
}