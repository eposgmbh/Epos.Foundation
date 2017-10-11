namespace Epos.Utilities.Composition
{
    public abstract class ContainerInstaller
    {
        public static readonly ContainerInstaller Empty = new EmptyContainerInstaller();

        public abstract void Install(Container container);

        private sealed class EmptyContainerInstaller : ContainerInstaller
        {
            public override void Install(Container container) {
            }
        }
    }
}