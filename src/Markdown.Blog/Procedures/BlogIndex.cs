using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace Markdown.Blog.Procedures
{
	public class BlogIndex
	{
		#region BlogMetadataList
		public void BuildBlogMetadataList(List<BlogMetadata> blogMetadataList, out string json, out byte[] binary)
		{
			// Serialize the blog metadata list to JSON string
			json = JsonConvert.SerializeObject(blogMetadataList, Newtonsoft.Json.Formatting.None);

			// Compress the JSON string to binary:
			// 1. MemoryStream: Temporary container for compressed bytes
			// 2. GZipStream: Performs compression with optimal level
			// 3. StreamWriter: Converts string to bytes for compression
			using (var memoryStream = new MemoryStream())
			{
				using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
				using (var writer = new StreamWriter(gzipStream))
				{
					// Write the JSON string through the compression pipeline
					writer.Write(json);
				}

				// Get the raw compressed bytes
				binary = memoryStream.ToArray();
			}
		}

		// Deserialize BlogMetadataList from JSON string
		public List<BlogMetadata> RestoreBlogMetadataListFromJson(string json)
		{
			// Convert JSON string back to blog metadata list
			return JsonConvert.DeserializeObject<List<BlogMetadata>>(json)
				?? new List<BlogMetadata>();
		}

		// Deserialize BlogMetadataList from compressed binary data
		public List<BlogMetadata> RestoreBlogMetadataListFromBinary(byte[] binary)
		{
			// Decompress binary data and convert back to blog metadata list:
			// 1. MemoryStream: Read compressed bytes
			// 2. GZipStream: Performs decompression
			// 3. StreamReader: Reads decompressed bytes as string
			using (var memoryStream = new MemoryStream(binary))
			using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
			using (var reader = new StreamReader(gzipStream))
			{
				// Read the decompressed JSON string
				string json = reader.ReadToEnd();

				// Convert JSON string back to blog metadata list
				return JsonConvert.DeserializeObject<List<BlogMetadata>>(json)
					?? new List<BlogMetadata>();
			}
		}
		#endregion
	}
}
