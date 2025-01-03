using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Markdown.Blog.Procedures
{
    public static class BlogRawContentProcessor
    {
        // Add static HttpClient for better performance
        private static readonly HttpClient _httpClient = new HttpClient();

        #region Index Metadata
        /// <summary>
        /// Downloads the index metadata JSON file as a string.
        /// </summary>
        /// <param name="division">The division containing repository information.</param>
        /// <returns>The content of the index metadata JSON file as a string.</returns>
        public static async Task<string> GetIndexMetadataJsonAsync(Division division)
        {
            try
            {
                return await _httpClient.GetStringAsync(division.IndexMetadataJsonUrl);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to download metadata from {division.IndexMetadataJsonUrl}", ex);
            }
        }

        /*
		/// <summary>
		/// Downloads and deserializes the index metadata JSON file into a list of BlogMetadata objects.
		/// </summary>
		/// <param name="division">The division containing repository information.</param>
		/// <returns>A list of BlogMetadata objects.</returns>
		public static async Task<List<BlogMetadata>> GetIndexMetadataAsync(Division division)
		{
			try
			{
				string json = await GetIndexMetadataJsonAsync(division);
				return BlogMetadataProcessor.DeserializeMetadata(json);
			}
			catch (JsonException ex)
			{
				throw new InvalidOperationException("Failed to parse blog metadata JSON", ex);
			}
		}
        */

		/// <summary>
		/// Downloads the index metadata binary file as a byte array with full response headers.
		/// </summary>
		/// <param name="division">The division containing repository information.</param>
		/// <returns>Response containing content, ETag and headers.</returns>
		public static async Task<(byte[] Content, string? ETag, HttpResponseHeaders Headers)> GetIndexMetadataBinaryWithHeadersAsync(Division division)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, division.IndexMetadataBinaryUrl);

                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsByteArrayAsync();
                var etag = response.Headers.ETag?.Tag;

                return (content, etag, response.Headers);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to download binary metadata from {division.IndexMetadataBinaryUrl}", ex);
            }
        }

        /// <summary>
        /// Downloads the index metadata binary file as a byte array.
        /// </summary>
        /// <param name="division">The division containing repository information.</param>
        /// <returns>Tuple of (byte[] content, string etag). etag is the current file's ETag from server</returns>
        public static async Task<(byte[] content, string? etag)> GetIndexMetadataBinaryAsync(Division division)
        {
            var response = await GetIndexMetadataBinaryWithHeadersAsync(division);
            return (response.Content, response.ETag ?? null);
        }

        /// <summary>
        /// Checks if the remote binary metadata file has been modified using HTTP HEAD request
        /// </summary>
        /// <param name="division">The division containing repository information</param>
        /// <param name="etag">Previous ETag value from last download</param>
        /// <returns>Tuple of (bool isModified, string newETag). newETag is the current file's ETag from server</returns>
        public static async Task<(bool isModified, string newETag)> CheckIndexMetadataBinaryAsync(
            Division division,
            string? etag)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Head, division.IndexMetadataBinaryUrl);

                if (!string.IsNullOrEmpty(etag))
                {
                    request.Headers.IfNoneMatch.Add(new System.Net.Http.Headers.EntityTagHeaderValue(etag));
                }

                var response = await _httpClient.SendAsync(request);
                // If 304 is returned, the file has not been changed and the original etag continues to be used.
                if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
                {
                    return (false, etag!);
                }

                // If 200, get new etag
                var newETag = response.Headers.ETag?.Tag;
                if (string.IsNullOrEmpty(newETag))
                {
                    throw new InvalidOperationException("Server did not return an ETag");
                }

                return (true, newETag);
            }
            catch (HttpRequestException)
            {
                // In case of a network error, continue to use the local version assuming that the content has not been changed
                return (false, etag ?? string.Empty);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to check binary metadata status from {division.IndexMetadataBinaryUrl}", ex);
            }
        }

        #endregion

        #region Blog Content
        /// <summary>
        /// Downloads the markdown content of a blog post based on its file path.
        /// </summary>
        /// <param name="division">The division containing repository information.</param>
        /// <param name="blogMetadata">The blog metadata containing the file path.</param>
        /// <returns>The markdown content of the blog post as a string.</returns>
        public static async Task<string> GetContentAsync(Division division, BlogMetadata blogMetadata)
        {
            try
            {
                // Construct the URL for the markdown file based on the division and file path
                // Remove the division name from the file path to avoid duplication
                string markdownUrl = $"{division.RawUrlBase}{blogMetadata.FilePath.Replace(division.DivisionName + "/", "")}";
                return await _httpClient.GetStringAsync(markdownUrl);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to download markdown content from {blogMetadata.FilePath}", ex);
            }
        }
        #endregion
    }
}