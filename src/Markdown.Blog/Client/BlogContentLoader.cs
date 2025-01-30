using System.Net.Http;
using System.Threading.Tasks;

namespace Markdown.Blog.Client
{
	/// <summary>
	/// Handles loading of blog content from remote GitHub repository
	/// </summary>
	public static class BlogContentLoader
	{
		private static readonly HttpClient httpClient = new HttpClient();

		public static string ConstructGitHubRawUrl(Division division, string filePath, bool pathStartedInDivision = false)
		{
			return $"{division.RawUrlBase(pathStartedInDivision)}{filePath}";
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