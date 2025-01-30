using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Markdown.Blog.Client
{
	/// <summary>
	/// Handles loading of blog content from remote GitHub repository
	/// </summary>
	public static class BlogContentLoader
	{
		private static readonly HttpClient httpClient;

		static BlogContentLoader()
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

		public static string ConstructGitHubRawUrl(Division division, string filePath, bool pathStartedInDivision = false)
		{
			// Add cache buster to URL
			string url = $"{division.RawUrlBase(pathStartedInDivision)}{filePath}";
			return $"{url}?t={DateTime.UtcNow.Ticks}";
		}

		/// <summary>
		/// Gets the raw content of a blog post from GitHub
		/// </summary>
		public static async Task<string> GetContentAsync(Division division, BlogData blogData)
		{
			string url = ConstructGitHubRawUrl(division, blogData.FilePath);
			return await httpClient.GetStringAsync(url);
		}
	}
}