using Polly.Wrap;

namespace Epos.Utilities.Web;

/// <summary> Provides Polly policies for resilient Web API calls.
/// </summary>
public interface IPolicyProvider
{
    /// <summary> Provides a Polly policy. </summary>
    /// <param name="url">URL for caching</param>
    /// <returns><see cref="AsyncPolicyWrap"/> instance</returns>
    AsyncPolicyWrap ProvidePolicy(string url);
}
