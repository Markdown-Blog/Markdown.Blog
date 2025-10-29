using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Markdown.Blog.Shared.Models;
using Markdown.Blog.Shared.Constants;

namespace Markdown.Blog.Client
{
	/// <summary>
	/// Handles loading of blog indexes from remote GitHub repository
	/// </summary>
	public static class BlogIndexLoader
	{
		private static readonly HttpClient httpClient;

		static BlogIndexLoader()
		{
			httpClient = new HttpClient();
			// Add default headers to prevent caching
			httpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
			{
				NoCache = true,
				NoStore = true,
				MustRevalidate = true,
				MaxAge = TimeSpan.Zero
			};
			httpClient.DefaultRequestHeaders.Pragma.Add(new System.Net.Http.Headers.NameValueHeaderValue("no-cache"));
		}

		public static async Task<BlogIndex> GetBlogIndexAsync(Division division)
		{
			string url = BlogContentLoader.ConstructGitHubRawUrl(division, BlogIndexFileNames.CompressedJson, true);
			var compressedData = await httpClient.GetByteArrayAsync(url);
			return BlogIndexDeserializer.RestoreBlogIndexFromBinary(compressedData);
		}

		public static async Task<int> GetBlogIndexVersionAsync(Division division)
		{
			string url = BlogContentLoader.ConstructGitHubRawUrl(division, BlogIndexFileNames.Version, true);
			string versionStr = await httpClient.GetStringAsync(url);
			return int.TryParse(versionStr, out int version) ? version : 0;
		}

		public static async Task<string> GetBlogIndexJsonAsync(Division division)
		{
			string url = BlogContentLoader.ConstructGitHubRawUrl(division, BlogIndexFileNames.Json, true);
			return await httpClient.GetStringAsync(url);
		}
	}
}