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

        /// <summary>
        /// Try to get the full blog index. Returns a success flag and the index if found.
        /// </summary>
        Task<(bool Success, Domain.Models.BlogIndex? BlogIndex)> TryGetBlogIndexAsync(string divisionDirectory);

        /// <summary>
        /// Save a changeset to a versioned diff file.
        /// </summary>
        Task SaveBlogIndexChangesetAsync(string divisionDirectory, Domain.Models.BlogIndexChangeset changeset);

        /// <summary>
        /// Keep the latest N changesets and delete the rest.
        /// </summary>
        Task CleanUpOldChangesetsAsync(string divisionDirectory, int keepCount);
    }
}