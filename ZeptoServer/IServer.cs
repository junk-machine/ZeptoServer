using System.Threading;
using System.Threading.Tasks;

namespace ZeptoServer
{
    /// <summary>
    /// Light-weight server.
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Runs the server.
        /// </summary>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        Task Run(CancellationToken cancellation);
    }
}
