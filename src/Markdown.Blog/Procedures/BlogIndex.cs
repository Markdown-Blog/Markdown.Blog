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
		#region BlogIndex

		private static string BlogIndexFileNameTemplate = "index.{id}.json";

		public static string GetBlogIndexFileNameUncompressed(int version = default) => 
			(version == default) 
			? BlogIndexFileNameTemplate.Replace("{id}.", "") 
			: BlogIndexFileNameTemplate.Replace("{id}", version.ToString());

		public static string GetBlogIndexFileNameCompressed(int version = default) => GetBlogIndexFileNameUncompressed() + ".gz";

		/// <summary>
		/// Builds a blog index with JSON and binary outputs.
		/// </summary>
		/// <param name="id">The version number to assign to this index.</param>
		/// <param name="blogMetadataList">The list of blog metadata to include in the index.</param>
		/// <param name="json">Output JSON string representation of the blog index.</param>
		/// <param name="binary">Output compressed binary data of the blog index.</param>
		public static void BuildBlogIndex(int id, List<BlogMetadata> blogMetadataList, out string json, out byte[] binary)
		{
			var blogIndex = new BlogIndex
			{
				Id = id,
				DateTime = DateTime.UtcNow,
				BlogMetadataList = blogMetadataList
			};

			// Serialize the entire BlogIndex object to JSON
			json = JsonConvert.SerializeObject(blogIndex, Newtonsoft.Json.Formatting.None);

			// Compress JSON string to binary data
			using (var memoryStream = new MemoryStream())
			{
				using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
				using (var writer = new StreamWriter(gzipStream))
				{
					writer.Write(json);
				}
				binary = memoryStream.ToArray();
			}
		}

		/// <summary>
		/// Restores a BlogIndex object from its JSON representation.
		/// </summary>
		/// <param name="json">The JSON string to deserialize.</param>
		/// <returns>A BlogIndex object, or a new instance if deserialization fails.</returns>
		public static BlogIndex RestoreBlogIndexFromJson(string json)
		{
			return JsonConvert.DeserializeObject<BlogIndex>(json)
				?? new BlogIndex { BlogMetadataList = new List<BlogMetadata>() };
		}

		/// <summary>
		/// Restores a BlogIndex object from its compressed binary representation.
		/// </summary>
		/// <param name="binary">The compressed binary data to deserialize.</param>
		/// <returns>A BlogIndex object, or a new instance if deserialization fails.</returns>
		public static BlogIndex RestoreBlogIndexFromBinary(byte[] binary)
		{
			using (var memoryStream = new MemoryStream(binary))
			using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
			using (var reader = new StreamReader(gzipStream))
			{
				string json = reader.ReadToEnd();
				return JsonConvert.DeserializeObject<BlogIndex>(json)
					?? new BlogIndex { BlogMetadataList = new List<BlogMetadata>() };
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
