namespace Epos.Utilities.Composition
{
    public abstract class RegistrationStep
    {
        internal RegistrationStep(ComponentRegistration componentRegistration) {
            ComponentRegistration = componentRegistration;
        } 

        internal ComponentRegistration ComponentRegistration { get; }
    }
}