namespace Epos.Utilities.Web
{
    /// <summary> Interface for an ASP.NET Core session that can hold objects.
    /// </summary>
    public interface IObjectSession
    {
        /// <summary> Gets or sets a session object. </summary>
        /// <param name="key">Key</param>
        /// <value>Session object</value>
        object this[string key] { get; set; }
    }
}
