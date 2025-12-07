using Markdown.Blog.Domain.Models;
using Markdown.Blog.Infrastructure.Compression;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.Blog.Infrastructure.Storage
{
	/// <summary>
	/// File system implementation of BlogIndex storage.
	/// </summary>
	public class FileSystemBlogIndexStorage : IBlogIndexStorage
	{
		public const string MetaDataDirectory = ".markdown.blog";

		public async Task<int> GetCurrentVersionAsync(string divisionDirectory)
		{
			var versionFile = Path.Combine(divisionDirectory, MetaDataDirectory, BlogIndexFileNames.Version);
			if (!File.Exists(versionFile))
				return 0;

			var text = await File.ReadAllTextAsync(versionFile);
			return int.TryParse(text, out var v) ? v : 0;
		}

		public async Task SaveIndexAsync(string divisionDirectory, BlogIndex blogIndex, int newVersion)
		{
			var metaDataDir = Path.Combine(divisionDirectory, MetaDataDirectory);
			Directory.CreateDirectory(metaDataDir);

			var jsonFile = Path.Combine(metaDataDir, BlogIndexFileNames.Full);
			var gzipFile = Path.Combine(metaDataDir, BlogIndexFileNames.FullCompressed);
			var versionFile = Path.Combine(metaDataDir, BlogIndexFileNames.Version);

			var json = JsonConvert.SerializeObject(blogIndex, Formatting.None);
			var compressor = new GzipCompressor();
			var binary = compressor.Compress(json);

			await File.WriteAllTextAsync(jsonFile, json, Encoding.UTF8);
			await File.WriteAllBytesAsync(gzipFile, binary);
			await File.WriteAllTextAsync(versionFile, newVersion.ToString());
		}

		public async Task<(bool Success, BlogIndex? BlogIndex)> TryGetBlogIndexAsync(string divisionDirectory)
		{
			var jsonFile = Path.Combine(divisionDirectory, MetaDataDirectory, BlogIndexFileNames.Full);
			if (!File.Exists(jsonFile))
				return (false, null);

			var json = await File.ReadAllTextAsync(jsonFile, Encoding.UTF8);
			var blogIndex = JsonConvert.DeserializeObject<BlogIndex>(json);
			return (true, blogIndex);
		}

        public async Task SaveBlogIndexChangesetAsync(string divisionDirectory, BlogIndexChangeset changeset)
        {
            var metaDataDir = Path.Combine(divisionDirectory, MetaDataDirectory);
            var diffFile = Path.Combine(metaDataDir, string.Format(BlogIndexFileNames.Diff, changeset.ToVersion));
            var compressedDiffFile = Path.Combine(metaDataDir, string.Format(BlogIndexFileNames.DiffCompressed, changeset.ToVersion));

			var json = JsonConvert.SerializeObject(changeset, Formatting.None);
			var compressor = new GzipCompressor();
			var binary = compressor.Compress(json);
			
			await File.WriteAllTextAsync(diffFile, json, Encoding.UTF8);
            await File.WriteAllBytesAsync(compressedDiffFile, binary);
        }

		public Task CleanUpOldChangesetsAsync(string divisionDirectory, int keepCount)
		{
			var metaDataDir = Path.Combine(divisionDirectory, MetaDataDirectory);
			if (!Directory.Exists(metaDataDir)) return Task.CompletedTask;

			var diffFiles = Directory.GetFiles(metaDataDir, "index.*.diff.json")
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

                    var compressedFileName = string.Format(BlogIndexFileNames.DiffCompressed, fileToDelete.Version);
                    var compressedFilePath = Path.Combine(metaDataDir, compressedFileName);
                    if (File.Exists(compressedFilePath))
                    {
                        File.Delete(compressedFilePath);
                    }
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