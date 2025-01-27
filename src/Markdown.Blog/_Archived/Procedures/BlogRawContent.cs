//using System;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using System.Net.Http.Headers;
//using Markdown.Blog.Shared;
//using Markdown.Blog.Client;

//namespace Markdown.Blog.Procedures
//{
//    public static class BlogRawContentProcessor
//    {
//        // Add static HttpClient for better performance
//        private static readonly HttpClient _httpClient = new HttpClient();

//        #region Index Metadata
//        /// <summary>
//        /// Downloads the index metadata JSON file as a string.
//        /// </summary>
//        /// <param name="division">The division containing repository information.</param>
//        /// <returns>The content of the index metadata JSON file as a string.</returns>
//        public static async Task<string> GetIndexJsonAsync(Division division)
//        {
//            try
//            {
//                return await _httpClient.GetStringAsync(division.GetIndexJsonUrl());
//            }
//            catch (HttpRequestException ex)
//            {
//                throw new InvalidOperationException($"Failed to download metadata from {division.GetIndexJsonUrl()}", ex);
//            }
//        }

//        /// <summary>
//        /// Downloads the blog index binary file as a byte array.
//        /// </summary>
//        /// <param name="division">The division containing repository information.</param>
//        /// <returns>The binary content of the blog index file.</returns>
//        public static async Task<byte[]> GetBlogIndexBinaryAsync(Division division)
//        {
//            try
//            {
//                var request = new HttpRequestMessage(HttpMethod.Get, division.GetIndexBinaryUrl());
//                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };

//                using var response = await _httpClient.SendAsync(request);
//                response.EnsureSuccessStatusCode();

//                return await response.Content.ReadAsByteArrayAsync();
//            }
//            catch (HttpRequestException ex)
//            {
//                throw new InvalidOperationException($"Failed to download binary blog index from {division.GetIndexBinaryUrl()}", ex);
//            }
//        }

//        /// <summary>
//        /// Checks if the next version of blog index binary file exists.
//        /// </summary>
//        /// <param name="division">The division containing repository information</param>
//        /// <param name="version">The version to check (typically current version + 1)</param>
//        /// <returns>True if the specified version exists, false if it returns 404</returns>
//        public static async Task<bool> CheckBlogIndexBinaryAsync(
//            Division division,
//            int version)
//        {
//            try
//            {
//                var request = new HttpRequestMessage(HttpMethod.Head, division.GetIndexBinaryUrl(version));
//                var response = await _httpClient.SendAsync(request);
                
//                return response.StatusCode == HttpStatusCode.OK;
//            }
//            catch (HttpRequestException ex)
//            {
//                if (ex.Message.Contains("404"))
//                {
//                    return false;
//                }
//                throw new InvalidOperationException($"Failed to check blog index version {version} at {division.GetIndexBinaryUrl(version)}", ex);
//            }
//        }

//        /// <summary>
//        /// Downloads and parses the blog index version number.
//        /// </summary>
//        /// <param name="division">The division containing repository information.</param>
//        /// <returns>The current version number of the blog index.</returns>
//        public static async Task<int> GetBlogIndexVersionAsync(Division division)
//        {
//            try
//            {
//                var request = new HttpRequestMessage(HttpMethod.Get, division.GetIndexVersionUrl());
//                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };

//                using var response = await _httpClient.SendAsync(request);
//                response.EnsureSuccessStatusCode();

//                var content = await response.Content.ReadAsStringAsync();
//                if (int.TryParse(content.Trim(), out int version))
//                {
//                    return version;
//                }
//                throw new InvalidOperationException("Invalid version number format");
//            }
//            catch (Exception ex)
//            {
//                throw new InvalidOperationException($"Failed to get blog index version from {division.GetIndexVersionUrl()}", ex);
//            }
//        }

//        #endregion

//        #region Blog Content
//        /// <summary>
//        /// Downloads the markdown content of a blog post based on its file path.
//        /// </summary>
//        /// <param name="division">The division containing repository information.</param>
//        /// <param name="blogMetadata">The blog metadata containing the file path.</param>
//        /// <returns>The markdown content of the blog post as a string.</returns>
//        public static async Task<string> GetContentAsync(Division division, BlogMetadata blogMetadata)
//        {
//            try
//            {
//                // Construct the URL for the markdown file based on the division and file path
//                // Remove the division name from the file path to avoid duplication
//                string markdownUrl = $"{division.RawUrlBase}{blogMetadata.FilePath.Replace(division.DivisionName + "/", "")}";
//                return await _httpClient.GetStringAsync(markdownUrl);
//            }
//            catch (HttpRequestException ex)
//            {
//                throw new InvalidOperationException($"Failed to download markdown content from {blogMetadata.FilePath}", ex);
//            }
//        }
//        #endregion
//    }
//}