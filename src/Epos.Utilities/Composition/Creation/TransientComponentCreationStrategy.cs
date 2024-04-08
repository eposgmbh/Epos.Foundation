using System;
using System.Reflection;

namespace Epos.Utilities.Composition;

internal sealed class TransientComponentCreationStrategy : ComponentCreationStrategy
{
    protected override object GetInstance(ConstructorInfo constructorInfo, object[] parameterValues)
        => constructorInfo.Invoke(parameterValues);

    protected override object GetInstance(Delegate factoryMethod)
        => factoryMethod.DynamicInvoke()!;
}
