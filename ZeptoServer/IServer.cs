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
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        Task Run();
    }
}
