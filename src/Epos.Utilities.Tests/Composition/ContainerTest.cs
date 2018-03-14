using System;

using NUnit.Framework;

namespace Epos.Utilities.Composition
{
    [TestFixture]
    public class ContainerTest
    {
        [Test]
        public void ConstructorWithNullArgumentForInstaller() {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Container(null));
        }

        [Test]
        public void VerySimpleRegistrationAndResolval() {
            Container theContainer = new Container();

            theContainer.Register<Dog>();
            theContainer.Register<IFood>().ImplementedBy<DogFood>();
            
            Dog theAnimal1 = theContainer.Resolve<Dog>();
            Dog theAnimal2 = theContainer.Resolve<Dog>();
            
            Assert.That(theAnimal1, Is.Not.SameAs(theAnimal2));
            Assert.That(theAnimal1.Food, Is.Not.SameAs(theAnimal2.Food));

        }

        [Test]
        public void SimpleRegistrationAndResolval() {
            Container theContainer = new Container();

            theContainer.Register<IAnimal>();
            Assert.Throws<InvalidOperationException>(() => theContainer.Resolve<IAnimal>());

            theContainer.Register<IAnimal>().ImplementedBy<Dog>();
            Assert.Throws<InvalidOperationException>(() => theContainer.Resolve<IAnimal>());

            theContainer.Register<IFood>().ImplementedBy<DogFood>();
            IAnimal theAnimal1 = theContainer.Resolve<IAnimal>();
            IAnimal theAnimal2 = theContainer.Resolve<IAnimal>();
            
            Assert.That(theAnimal1, Is.Not.SameAs(theAnimal2));
            Assert.That(theAnimal1.Food, Is.Not.SameAs(theAnimal2.Food));

        }

        [Test]
        public void RegistrationAndResolvalWithSingletonFood() {
            Container theContainer = new Container();

            theContainer.Register<IAnimal>().ImplementedBy<Dog>();
            theContainer.Register<IFood>().ImplementedBy<DogFood>().WithLifetime(Lifetime.Singleton);
            
            IAnimal theAnimal1 = theContainer.Resolve<IAnimal>();
            IAnimal theAnimal2 = theContainer.Resolve<IAnimal>();
            
            Assert.That(theAnimal1, Is.Not.SameAs(theAnimal2));
            Assert.That(theAnimal1.Food, Is.SameAs(theAnimal2.Food));
        }

        [Test]
        public void RegistrationAndResolvalWithSingletonAnimal() {
            Container theContainer = new Container();

            theContainer.Register<IAnimal>().ImplementedBy<Dog>().WithLifetime(Lifetime.Singleton);
            theContainer.Register<IFood>().ImplementedBy<DogFood>();
            
            IAnimal theAnimal1 = theContainer.Resolve<IAnimal>();
            IAnimal theAnimal2 = theContainer.Resolve<IAnimal>();
            
            Assert.That(theAnimal1, Is.SameAs(theAnimal2));
            Assert.That(theAnimal1.Food, Is.SameAs(theAnimal2.Food));
        }

        [Test]
        public void ConstructorParameters() {
            Container theContainer = new Container();

            theContainer
                .Register<ITestService>()
                .ImplementedBy<TestService>()
                .WithParameter("connectionString", "Hello World!")
                .AndParameter("maxCount", 10)
                .WithLifetime(Lifetime.Singleton);

            theContainer
                .Register<IDependency>()
                .ImplementedBy<Dependency>()
                .WithLifetime(Lifetime.Singleton);

            ITestService theTestService = theContainer.Resolve<ITestService>();
            IDependency theDependency = theContainer.Resolve<IDependency>();

            Assert.That(theTestService.Dependency, Is.SameAs(theDependency)); // wg. Singleton
            Assert.That(theTestService.ConnectionString, Is.EqualTo("Hello World!"));
            Assert.That(theTestService.MaxCount, Is.EqualTo(10));
        }
    }
}