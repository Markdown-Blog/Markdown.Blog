using System.IO;
using System.Text;
using System.Threading.Tasks;
using Markdown.Blog.Domain.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

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

        public async Task<(bool Success, BlogIndex? BlogIndex)> TryGetBlogIndexAsync(string divisionDirectory)
        {
            var jsonFile = Path.Combine(divisionDirectory, BlogIndexFileNames.Json);
            if (!File.Exists(jsonFile))
                return (false, null);

            var json = await File.ReadAllTextAsync(jsonFile, Encoding.UTF8);
            var blogIndex = JsonConvert.DeserializeObject<BlogIndex>(json);
            return (true, blogIndex);
        }

        public async Task SaveBlogIndexChangesetAsync(string divisionDirectory, BlogIndexChangeset changeset)
        {
            var diffFile = Path.Combine(divisionDirectory, string.Format(BlogIndexFileNames.Diff, changeset.ToVersion));
            var json = JsonConvert.SerializeObject(changeset, Formatting.None);
            await File.WriteAllTextAsync(diffFile, json, Encoding.UTF8);
        }

        public Task CleanUpOldChangesetsAsync(string divisionDirectory, int keepCount)
        {
            var diffFiles = Directory.GetFiles(divisionDirectory, "index.*.diff.json")
                                     .Select(f => new { File = new FileInfo(f), Version = GetVersionFromDiffFileName(f) })
                                     .Where(x => x.Version > 0)
                                     .OrderByDescending(x => x.Version)
                                     .ToList();

            var toDelete = diffFiles.Skip(keepCount).ToList();

            foreach (var fileToDelete in toDelete)
            {
                try
                {
                    fileToDelete.File.Delete();
                }
                catch (IOException ex)
                {
                    // Log the error, e.g., using a logging framework
                    System.Console.WriteLine($"Error deleting old changeset file {fileToDelete.File.Name}: {ex.Message}");
                }
            }

            return Task.CompletedTask;
        }

        private int GetVersionFromDiffFileName(string fileName)
        {
            var match = System.Text.RegularExpressions.Regex.Match(Path.GetFileName(fileName), @"index\.(\d+)\.diff\.json");
            return match.Success && int.TryParse(match.Groups[1].Value, out int version) ? version : 0;
        }
    }
}