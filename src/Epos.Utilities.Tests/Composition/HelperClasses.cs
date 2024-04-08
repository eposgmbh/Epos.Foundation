namespace Epos.Utilities.Composition;

#region ITestService

public interface ITestService
{
    IDependency Dependency { get; set; }

    string ConnectionString { get; set; }

    int MaxCount { get; set; }
}

public class TestService : ITestService
{
    public IDependency Dependency { get; set; }

    public string ConnectionString { get; set; }

    public int MaxCount { get; set; }

    public TestService(IDependency dependency, string connectionString, int maxCount) {
        Dependency = dependency;
        ConnectionString = connectionString;
        MaxCount = maxCount;
    }
}

public interface IDependency {}

public class Dependency : IDependency {}

#endregion

#region IAnimal

public interface IAnimal
{
    IFood? Food { get; set; }
}

public class Dog : IAnimal
{
    public IFood? Food { get; set; }

    public Dog() {}

    public Dog(IFood food) {
        Food = food;
    }
}

public interface IFood {}

public class DogFood : IFood {}

#endregion
