using System.Threading.Tasks;

namespace ZeptoServer.Ftp
{
    /// <summary>
    /// Defines common instances of <see cref="Task{T}"/> that can be shared.
    /// </summary>
    public static class CommonTasks
    {
        /// <summary>
        /// Static instance of <see cref="Task{Boolean}"/> that returns true.
        /// </summary>
        public static readonly Task<bool> True = Task.FromResult(true);

        /// <summary>
        /// Static instance of <see cref="Task{Boolean}"/> that returns false.
        /// </summary>
        public static readonly Task<bool> False = Task.FromResult(false);
    }
}
