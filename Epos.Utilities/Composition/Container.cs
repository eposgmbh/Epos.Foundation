using System;
using System.Collections.Generic;

namespace Epos.Utilities.Composition
{
    /// <summary>Provides a simple lightweight DI (Dependency Injection) Container.
    /// </summary>
    public sealed class Container
    {
        private readonly IDictionary<Type, ComponentRegistration> myComponents;
        private bool myAlreadyResolved;

        /// <summary>Initializes an instance of the <see cref="Container"/> class.</summary>
        public Container() : this(ContainerInstaller.Empty) { }

        /// <summary>Initializes an instance of the <see cref="Container"/> class with the
        /// specified <paramref name="installer"/>.</summary>
        /// <param name="installer">Container installer</param>
        public Container(ContainerInstaller installer) {
            if (installer == null) {
                throw new ArgumentNullException(nameof(installer));
            }

            myComponents = new Dictionary<Type, ComponentRegistration>();

            installer.Install(this);
        }

        /// <summary>Registers the abstraction of type <typeparamref name="T"/>
        /// in the container.</summary>
        /// <remarks>Type <typeparamref name="T"/> may also be a concrete type. If an
        /// abstraction is given, you can register a concrete implementation via the fluent
        /// interface <see cref="RegisterOptions{TAbstract}.ImplementedBy{TConcrete}()"/>.
        /// Other options can also be specified via the fluent interface.</remarks>
        /// <example><code><![CDATA[
        /// Container theContainer = new Container();
        ///
        /// theContainer
        ///     .Register<ITestService>()
        ///     .ImplementedBy<TestService>()
        ///     .WithParameter("connectionString", "Hello World!") // Constructor params
        ///     .AndParameter("maxCount", 10)
        ///     .WithLifetime(Lifetime.Singleton);
        /// ]]></code></example>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Register options fluent interface step</returns>
        public RegisterOptions<T> Register<T>() {
            TestAlreadyResolved();

            var theComponentRegistration = new ComponentRegistration(typeof(T), this);
            myComponents[typeof(T)] = theComponentRegistration;

            return new RegisterOptions<T>(theComponentRegistration);
        }

        /// <summary>Resolves an instance for the abstraction type
        /// <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Instance</returns>
        public T Resolve<T>() {
            return (T) Resolve(typeof (T));
        }

        /// <summary>Resolves an instance for the abstraction
        /// <paramref name="type"/>.</summary>
        /// <param name="type">Type</param>
        /// <returns>Instance</returns>
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
                throw new InvalidOperationException(
                    "Registering further components is not allowed after resolving for the first time."
                );
            }
        }
    }
}
