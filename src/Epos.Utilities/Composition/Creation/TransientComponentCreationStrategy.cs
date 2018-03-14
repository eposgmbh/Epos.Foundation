using System;
using System.Reflection;

namespace Epos.Utilities.Composition
{
    internal sealed class TransientComponentCreationStrategy : ComponentCreationStrategy
    {
        protected override object GetInstance(ConstructorInfo constructorInfo, object[] parameterValues) {
            return constructorInfo.Invoke(parameterValues);
        }

        protected override object GetInstance(Delegate factoryMethod) {
            return factoryMethod.DynamicInvoke();
        }
    }
}