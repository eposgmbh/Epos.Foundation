using System;
using System.Reflection;

namespace Epos.Utilities.Composition;

internal sealed class SingletonComponentCreationStrategy : ComponentCreationStrategy
{
    private object? mySingleton;

    protected override object GetInstance(ConstructorInfo constructorInfo, object[] parameterValues)
        => mySingleton ??= constructorInfo.Invoke(parameterValues);

    protected override object GetInstance(Delegate factoryMethod)
        => mySingleton ??= factoryMethod.DynamicInvoke()!;
}
