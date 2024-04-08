using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Epos.Utilities.Composition;

internal sealed class ComponentRegistration
{
    private ConstructorInfo? myConstructurInfo;
    private ComponentCreationStrategy? myComponentCreationStrategy;

    public ComponentRegistration(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type,
        Container container
    ) {
        Container = container;
        ConcreteType = type;
        ComponentCreationStrategy = new TransientComponentCreationStrategy();
        Parameters = new Dictionary<string, object>();
    }

    public Container Container { get; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type ConcreteType { get; set; }

    public IDictionary<string, object> Parameters { get; }

    public Delegate? FactoryMethod { get; set; }

    public ConstructorInfo ConstructurInfo => myConstructurInfo ?? (myConstructurInfo = GetConstructorInfo());

    public ComponentCreationStrategy ComponentCreationStrategy {
        get => myComponentCreationStrategy!;
        set {
            myComponentCreationStrategy = value;
            myComponentCreationStrategy.ComponentRegistration = this;
        }
    }

    internal object GetComponentInstance() => ComponentCreationStrategy.GetInstance();

    #region Hilfsmethoden

    private ConstructorInfo GetConstructorInfo() {
        // Konstruktur mit den meisten Dependencies finden:
        if (ConcreteType.IsAbstract) {
            throw new InvalidOperationException(
                "Registered component '" + ConcreteType.Dump() + "' must not be an abstract type."
            );
        }

        ConstructorInfo[] theConstructorInfos = ConcreteType.GetConstructors();
        if (theConstructorInfos.Length == 0) {
            throw new InvalidOperationException(
                "Registered component '" + ConcreteType.Dump() +
                "' must have at least one public constructor."
            );
        }

        if (theConstructorInfos.Length == 1) {
            // Regelfall:
            return theConstructorInfos[0];
        } else {
            var theConstructorsWithParameterLengthDescending = (
                from theConstructorInfo in theConstructorInfos
                orderby theConstructorInfo.GetParameters().Length descending
                select theConstructorInfo
            ).ToList();

            ConstructorInfo theFirstConstructorInfo =
                theConstructorsWithParameterLengthDescending[0];
            ConstructorInfo theSecondConstructorInfo =
                theConstructorsWithParameterLengthDescending[1];

            if (theFirstConstructorInfo.GetParameters().Length ==
                theSecondConstructorInfo.GetParameters().Length) {
                throw new InvalidOperationException(
                    "Registered component '" + ConcreteType.Dump() +
                    " must have a nonambiguous constructor."
                );
            }

            return theFirstConstructorInfo;
        }
    }

    #endregion
}
