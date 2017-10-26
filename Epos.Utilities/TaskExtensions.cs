using System.Threading.Tasks;

namespace Epos.Utilities
{
    /// <summary>Collection of extension methods for the
    /// <see cref="System.Threading.Tasks.Task" /> type.</summary>
    public static class TaskExtensions
    {
        /// <summary> Prevents the compiler warning "This async method lacks
        /// 'await' operators and will run synchronously...".</summary>
        /// <param name="task">Task</param>
        // ReSharper disable once UnusedParameter.Global
        public static void FireAndForget(this Task task) { }
    }
}
