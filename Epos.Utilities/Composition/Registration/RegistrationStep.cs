namespace Epos.Utilities.Composition
{
    /// <summary>Registration fluent interface step helper class.
    /// </summary>
    public abstract class RegistrationStep
    {
        internal RegistrationStep(ComponentRegistration componentRegistration) {
            ComponentRegistration = componentRegistration;
        } 

        internal ComponentRegistration ComponentRegistration { get; }
    }
}
