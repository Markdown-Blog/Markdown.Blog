using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.Blog.Infrastructure.Storage
{
    /// <summary>
    /// File system implementation of BlogIndex storage.
    /// </summary>
    public class FileSystemBlogIndexStorage : IBlogIndexStorage
    {
        public async Task<int> GetCurrentVersionAsync(string divisionDirectory)
        {
            var versionFile = Path.Combine(divisionDirectory, BlogIndexFileNames.Version);
            if (!File.Exists(versionFile))
                return 0;

            var text = await File.ReadAllTextAsync(versionFile);
            return int.TryParse(text, out var v) ? v : 0;
        }

        public async Task SaveIndexAsync(string divisionDirectory, string json, byte[] binary, int newVersion)
        {
            Directory.CreateDirectory(divisionDirectory);

            var jsonFile = Path.Combine(divisionDirectory, BlogIndexFileNames.Json);
            var gzipFile = Path.Combine(divisionDirectory, BlogIndexFileNames.CompressedJson);
            var versionFile = Path.Combine(divisionDirectory, BlogIndexFileNames.Version);

            await File.WriteAllTextAsync(jsonFile, json, Encoding.UTF8);
            await File.WriteAllBytesAsync(gzipFile, binary);
            await File.WriteAllTextAsync(versionFile, newVersion.ToString());
        }
    }
}