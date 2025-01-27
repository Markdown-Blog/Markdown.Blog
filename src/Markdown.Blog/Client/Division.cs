﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace Markdown.Blog.Client
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

        // Constants for file names
        private const string IndexJsonFileName = "index.json";
        private const string IndexVersionFileName = "index.version";

        // Gets the path for the division, which is the same as DivisionName
        public string Path => DivisionName;

        // Gets the base URL for the raw content of the division.
        // The URL is constructed using the GitHub username, repository name, and division name.
        // The URL is in the format: https://raw.githubusercontent.com/{GithubUsername}/{GithubRepository}/refs/heads/main/{DivisionName}/
        public string RawUrlBase => $"https://raw.githubusercontent.com/{GithubUsername}/{GithubRepository}/refs/heads/main/{DivisionName}/";

        /// <summary>
        /// Gets the URL for the blog index JSON file.
        /// </summary>
        /// <param name="version">The version of the index file. Defaults to 0 for the latest version.</param>
        /// <returns>The URL in the format: https://raw.githubusercontent.com/{GithubUsername}/{GithubRepository}/refs/heads/main/{DivisionName}/index[.v{version}].json</returns>
        public string GetIndexJsonUrl() =>
            $"{RawUrlBase}{IndexJsonFileName}";

        /// <summary>
        /// Gets the URL for the blog index binary file.
        /// </summary>
        /// <param name="version">The version of the index file. Defaults to 0 for the latest version.</param>
        /// <returns>The URL in the format: https://raw.githubusercontent.com/{GithubUsername}/{GithubRepository}/refs/heads/main/{DivisionName}/index[.v{version}].json.gz</returns>
        public string GetIndexBinaryUrl() =>
            $"{RawUrlBase}{IndexJsonFileName}.gz";

        /// <summary>
        /// Gets the URL for the blog index version file.
        /// </summary>
        /// <returns>The URL in the format: https://raw.githubusercontent.com/{GithubUsername}/{GithubRepository}/refs/heads/main/{DivisionName}/index.version</returns>
        public string GetIndexVersionUrl() =>
            $"{RawUrlBase}{IndexVersionFileName}";

        /// <summary>
        /// Downloads the index metadata JSON file as a string.
        /// </summary>
        /// <returns>The content of the index metadata JSON file as a string.</returns>
        public async Task<string> GetIndexJsonAsync()
        {
            return await BlogIndexLoader.GetBlogIndexJsonAsync(this);
        }

        /// <summary>
        /// Downloads the blog index binary file as a byte array.
        /// </summary>
        /// <returns>The binary content of the blog index file.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the download fails</exception>
        public async Task<byte[]> GetIndexBinaryAsync()
        {
            string url = GetIndexBinaryUrl();
            using var client = new HttpClient();
            return await client.GetByteArrayAsync(url);
        }

        /// <summary>
        /// Gets the current version number of the blog index.
        /// </summary>
        /// <returns>The current version number from index.version file.</returns>
        public async Task<int> GetBlogIndexVersionAsync()
        {
            return await BlogIndexLoader.GetBlogIndexVersionAsync(this);
        }
    }
}
