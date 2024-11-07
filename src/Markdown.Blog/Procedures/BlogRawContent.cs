using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Markdown.Blog.Procedures
{
    public static class BlogRawContent
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

        /// <summary>
        /// Downloads the index metadata binary file as a byte array.
        /// </summary>
        /// <param name="division">The division containing repository information.</param>
        /// <returns>Tuple of (byte[] content, DateTime lastModifiedUtc)</returns>
        public static async Task<(byte[] content, DateTime lastModifiedUtc)> GetIndexMetadataBinaryAsync(Division division)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, division.IndexMetadataBinaryUrl);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsByteArrayAsync();
                var lastModified = response.Content.Headers.LastModified?.UtcDateTime ?? DateTime.UtcNow;

                return (content, lastModified);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to download binary metadata from {division.IndexMetadataBinaryUrl}", ex);
            }
        }

        /// <summary>
        /// Checks if the remote binary metadata file has been modified using HTTP HEAD request
        /// </summary>
        /// <param name="division">The division containing repository information</param>
        /// <param name="lastModified">Last known modification time in UTC</param>
        /// <returns>Tuple of (bool isModified, DateTime newLastModified). newLastModified is always in UTC</returns>
        public static async Task<(bool isModified, DateTime newLastModified)> CheckIndexMetadataBinaryAsync(
            Division division,
            DateTime? lastModified = null)
        {
            // Ensure input DateTime is UTC
            if (lastModified.HasValue && lastModified.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("lastModified must be in UTC", nameof(lastModified));
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Head, division.IndexMetadataBinaryUrl);

                if (lastModified.HasValue)
                {
                    request.Headers.IfModifiedSince = lastModified.Value;
                }

                var response = await _httpClient.SendAsync(request);
                var newLastModified = response.Content.Headers.LastModified?.UtcDateTime ?? DateTime.UtcNow;

                return (response.StatusCode != System.Net.HttpStatusCode.NotModified, newLastModified);
            }
            catch (HttpRequestException ex)
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