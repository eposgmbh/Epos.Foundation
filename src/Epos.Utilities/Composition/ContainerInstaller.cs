namespace Epos.Utilities.Composition;

/// <summary>Provides the base class for a container installer.
/// </summary>
public abstract class ContainerInstaller
{
    /// <summary>Empty container installer</summary>
    public static readonly ContainerInstaller Empty = new EmptyContainerInstaller();

    /// <summary>Registers types with the specified <paramref name="container"/>.</summary>
    /// <param name="container">Container</param>
    public abstract void Install(Container container);

    private sealed class EmptyContainerInstaller : ContainerInstaller
    {
        public override void Install(Container container) {
        }
    }
}
