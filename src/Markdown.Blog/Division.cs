using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Markdown.Blog.Procedures;

namespace Markdown.Blog
{
	// This class represents a division in the blog system.
	// Each division can be rendered independently or combined with others into a website.
	// Each division maintains its own index.
	public class Division
	{
		// Gets or sets the GitHub username associated with the division.
		public string GithubUsername { get; set; }
		// Gets or sets the GitHub repository name associated with the division.
		public string GithubRepository { get; set; }
		// Gets or sets the name of the division.
		public string DivisionName { get; set; }

		// Gets the base URL for the raw content of the division.
		// The URL is constructed using the GitHub username, repository name, and division name.
		// The URL is in the format: https://raw.githubusercontent.com/{GithubUsername}/{GithubRepository}/refs/heads/main/{DivisionName}/
		public string RawUrlBase => $"https://raw.githubusercontent.com/{GithubUsername}/{GithubRepository}/refs/heads/main/{DivisionName}/";

		/// <summary>
		/// Gets the URL for the blog index JSON file.
		/// </summary>
		/// <param name="version">The version of the index file. Defaults to 0 for the latest version.</param>
		/// <returns>The URL in the format: https://raw.githubusercontent.com/{GithubUsername}/{GithubRepository}/refs/heads/main/{DivisionName}/index[.v{version}].json</returns>
		public string GetIndexJsonUrl(int version = 0) => 
			$"{RawUrlBase}{BlogIndexProcessor.GetBlogIndexFileNameUncompressed(version)}";

		/// <summary>
		/// Gets the URL for the blog index binary file.
		/// </summary>
		/// <param name="version">The version of the index file. Defaults to 0 for the latest version.</param>
		/// <returns>The URL in the format: https://raw.githubusercontent.com/{GithubUsername}/{GithubRepository}/refs/heads/main/{DivisionName}/index[.v{version}].json.gz</returns>
		public string GetIndexBinaryUrl(int version = 0) => 
			$"{RawUrlBase}{BlogIndexProcessor.GetBlogIndexFileNameCompressed(version)}";

		/// <summary>
		/// Downloads the index metadata JSON file as a string.
		/// </summary>
		/// <returns>The content of the index metadata JSON file as a string.</returns>
		public async Task<string> GetIndexJsonAsync()
		{
			return await BlogRawContentProcessor.GetIndexJsonAsync(this);
		}

		/// <summary>
		/// Downloads the blog index binary file as a byte array.
		/// </summary>
		/// <returns>The binary content of the blog index file.</returns>
		/// <exception cref="InvalidOperationException">Thrown when the download fails</exception>
		public async Task<byte[]> GetIndexBinaryAsync()
		{
			return await BlogRawContentProcessor.GetBlogIndexBinaryAsync(this);
		}

		/// <summary>
		/// Checks if the specified version of blog index binary file exists.
		/// </summary>
		/// <param name="version">The version number to check</param>
		/// <returns>True if the specified version exists, false if it returns 404</returns>
		public async Task<bool> CheckBlogIndexBinaryAsync(int version)
		{
			return await BlogRawContentProcessor.CheckBlogIndexBinaryAsync(this, version);
		}
	}
}
