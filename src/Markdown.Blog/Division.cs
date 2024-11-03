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

		// Gets the URL for the index metadata JSON.
		// The URL is in the format: https://raw.githubusercontent.com/{GithubUsername}/{GithubRepository}/refs/heads/main/{DivisionName}/indexMetadata.json
		public string IndexMetadataJsonUrl => $"{RawUrlBase}indexMetadata.json";

		// Gets the URL for the index metadata binary.
		// The URL is in the format: https://raw.githubusercontent.com/{GithubUsername}/{GithubRepository}/refs/heads/main/{DivisionName}/indexMetadata.json.gz
		public string IndexMetadataBinaryUrl => $"{RawUrlBase}indexMetadata.json.gz";

		/// <summary>
		/// Downloads the index metadata JSON file as a string.
		/// </summary>
		/// <returns>The content of the index metadata JSON file as a string.</returns>
		public async Task<string> GetIndexMetadataJsonAsync()
		{
			return await BlogRawContent.GetIndexMetadataJsonAsync(this);
		}

		/// <summary>
		/// Downloads the index metadata binary file as a byte array.
		/// </summary>
		/// <returns>The content of the index metadata binary file as a byte array.</returns>
		public async Task<byte[]> GetIndexMetadataBinaryAsync()
		{
			return await BlogRawContent.GetIndexMetadataBinaryAsync(this);
		}
	}
}
