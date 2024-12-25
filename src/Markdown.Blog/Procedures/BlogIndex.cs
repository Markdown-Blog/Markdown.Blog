using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace Markdown.Blog.Procedures
{
	public static class BlogIndexProcessor
	{
		#region BlogMetadataList
		public static void BuildBlogMetadataList(List<BlogMetadata> blogMetadataList, out string json, out byte[] binary)
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
		public static List<BlogMetadata> RestoreBlogMetadataListFromJson(string json)
		{
			// Convert JSON string back to blog metadata list
			return JsonConvert.DeserializeObject<List<BlogMetadata>>(json)
				?? new List<BlogMetadata>();
		}

		// Deserialize BlogMetadataList from compressed binary data
		public static List<BlogMetadata> RestoreBlogMetadataListFromBinary(byte[] binary)
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

		#region Tags

		/// <summary>
		/// Organizes tags and their corresponding article counts from a list of blog metadata.
		/// </summary>
		/// <param name="blogMetadataList">The list of blog metadata.</param>
		/// <returns>A dictionary with tags as keys and their article counts as values, ordered by tag name alphabetically.</returns>
		public static Dictionary<string, int> OrganizeTagsAndArticleCounts(List<BlogMetadata> blogMetadataList)
		{
			Dictionary<string, int> result;
			var tagCounts = new Dictionary<string, int>();

			foreach (var metadata in blogMetadataList)
			{
				foreach (var tag in metadata.Tags)
				{
					if (tagCounts.ContainsKey(tag))
					{
						tagCounts[tag]++;
					}
					else
					{
						tagCounts.Add(tag, 1);
					}
				}
			}

			// Order the dictionary by count in descending order
			result = tagCounts.OrderByDescending(pair => pair.Value)
				.ToDictionary(pair => pair.Key, pair => pair.Value);

			return result;
		}


		#endregion

		#region Category

		/// <summary>
		/// Returns a list of categories, subcategories, and the count of blogs under each subcategory from a list of blog metadata.
		/// </summary>
		/// <param name="blogMetadataList">The list of blog metadata.</param>
		/// <returns>A list of tuples, each containing the category, subcategory, and the count of blogs under the subcategory.</returns>
		public static List<(string Category, string SubCategory, int BlogCount)> GetCategoriesAndSubcategoriesWithBlogCount(List<BlogMetadata> blogMetadataList)
		{
			var categories = new List<(string Category, string SubCategory, int BlogCount)>();
			var subCategoryCounts = new Dictionary<string, int>();

			// 统计每个 SubCategory 的博客数量
			foreach (var metadata in blogMetadataList)
			{
				if (!string.IsNullOrEmpty(metadata.Hierarchy?.SubCategory))
				{
					var subCategory = metadata.Hierarchy.SubCategory;
					subCategoryCounts[subCategory] = subCategoryCounts.TryGetValue(subCategory, out var count) ? count + 1 : 1;
				}
			}

			// 构建结果列表
			foreach (var metadata in blogMetadataList)
			{
				if (metadata.Hierarchy != null && !string.IsNullOrEmpty(metadata.Hierarchy.SubCategory))
				{
					categories.Add((metadata.Hierarchy.Category, metadata.Hierarchy.SubCategory, subCategoryCounts[metadata.Hierarchy.SubCategory]));
				}
			}

			return categories.Distinct().ToList();
		}

		#endregion

		#region LatestBlogs

		/// <summary>
		/// Returns the latest blogs for each category or division, sorted in descending order by date, filtered by cover image status, starting from a specified index.
		/// </summary>
		/// <param name="blogMetadataList">The list of blog metadata.</param>
		/// <param name="isPerCategory">Indicates whether to group by category or division.</param>
		/// <param name="numberOfBlogs">The number of latest blogs to return.</param>
		/// <param name="coverImageStatus">Specifies the cover image status to filter by.</param>
		/// <param name="startIndex">The starting index position. Defaults to 0, meaning to start from the first blog.</param>
		/// <returns>A dictionary where each key is a category or division and the value is a list of the latest blogs, filtered by cover image status, starting from the specified index.</returns>
		public static Dictionary<string, List<BlogMetadata>> GetLatestBlogs(List<BlogMetadata> blogMetadataList, bool isPerCategory, int numberOfBlogs, CoverImageStatus coverImageStatus, int startIndex = 0)
		{
			var latestBlogs = new Dictionary<string, List<BlogMetadata>>();
			var keySelector = isPerCategory
				? new Func<BlogMetadata, string>(m => m.Hierarchy?.Category)
				: new Func<BlogMetadata, string>(m => m.Hierarchy?.Division);

			foreach (var metadata in blogMetadataList)
			{
				var key = keySelector(metadata);
				if (key != null && FilterByCoverImageStatus(metadata, coverImageStatus))
				{
					if (!latestBlogs.ContainsKey(key))
					{
						latestBlogs[key] = new List<BlogMetadata>();
					}
					latestBlogs[key].Add(metadata);
				}
			}

			foreach (var key in latestBlogs.Keys)
			{
				latestBlogs[key] = latestBlogs[key]
					.OrderByDescending(metadata => metadata.Date)
					.Skip(startIndex)
					.Take(numberOfBlogs)
					.ToList();
			}

			return latestBlogs;
		}

		private static bool FilterByCoverImageStatus(BlogMetadata metadata, CoverImageStatus status)
		{
			switch (status)
			{
				case CoverImageStatus.All:
					return true;
				case CoverImageStatus.NoCover:
					return metadata.CoverImages.Count == 0;
				case CoverImageStatus.WithCover:
					return metadata.CoverImages.Count > 0;
				default:
					return false;
			}
		}


		#endregion
	}
}
