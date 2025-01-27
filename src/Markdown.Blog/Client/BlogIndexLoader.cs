using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Markdown.Blog.Shared;

namespace Markdown.Blog.Client
{
	/// <summary>
	/// Handles loading of blog indexes from remote GitHub repository
	/// </summary>
	public static class BlogIndexLoader
	{
		private static readonly HttpClient httpClient = new HttpClient();

		public static async Task<BlogIndex> GetBlogIndexAsync(Division division)
		{
			string url = BlogContentLoader.ConstructGitHubRawUrl(division, "index.json.gz");
			var compressedData = await httpClient.GetByteArrayAsync(url);
			return BlogIndexDeserializer.RestoreBlogIndexFromBinary(compressedData);
		}

		public static async Task<int> GetBlogIndexVersionAsync(Division division)
		{
			string url = BlogContentLoader.ConstructGitHubRawUrl(division, "index.version");
			string versionStr = await httpClient.GetStringAsync(url);
			return int.TryParse(versionStr, out int version) ? version : 0;
		}

		public static async Task<string> GetBlogIndexJsonAsync(Division division)
		{
			string url = BlogContentLoader.ConstructGitHubRawUrl(division, "index.json");
			return await httpClient.GetStringAsync(url);
		}
	}
}