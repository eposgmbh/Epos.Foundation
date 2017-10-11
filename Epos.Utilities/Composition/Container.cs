using System;
using System.Collections.Generic;

namespace Epos.Utilities.Composition
{
    public sealed class Container
    {
        private readonly IDictionary<Type, ComponentRegistration> myComponents;
        private bool myAlreadyResolved;

        public Container() : this(ContainerInstaller.Empty) {
        }

        public Container(ContainerInstaller installer) {
            if (installer == null) {
                throw new ArgumentNullException(nameof(installer));
            }

            myComponents = new Dictionary<Type, ComponentRegistration>();

            installer.Install(this);
        }

        public RegisterOptions<T> Register<T>() {
            TestAlreadyResolved();

            var theComponentRegistration = new ComponentRegistration(typeof(T), this);
            myComponents[typeof(T)] = theComponentRegistration;

            return new RegisterOptions<T>(theComponentRegistration);
        }

        public void UnregisterAll() {
            myComponents.Clear();
            myAlreadyResolved = false;
        }

        public T Resolve<T>() {
            return (T) Resolve(typeof (T));
        }

        public object Resolve(Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (!myComponents.TryGetValue(type, out ComponentRegistration theComponentRegistration)) {
                throw new InvalidOperationException(
                    $"Found no registered component for {type.Dump()}"
                );
            }

            object theResult = theComponentRegistration.GetComponentInstance();
            myAlreadyResolved = true;
            return theResult;
        }

        internal void TestAlreadyResolved() {
            if (myAlreadyResolved) {
                throw new InvalidOperationException("Registering further components is not allowed after resolving.");
            }
        }
    }
}