using System.Threading.Tasks;

namespace Markdown.Blog.Infrastructure.Storage
{
    /// <summary>
    /// Storage adapter for reading/writing BlogIndex artifacts.
    /// </summary>
    public interface IBlogIndexStorage
    {
        /// <summary>
        /// Get current index version. If file missing, returns 0.
        /// </summary>
        Task<int> GetCurrentVersionAsync(string divisionDirectory);

        /// <summary>
        /// Persist index JSON and compressed bytes with the provided version.
        /// </summary>
        Task SaveIndexAsync(string divisionDirectory, string json, byte[] binary, int newVersion);
    }
}