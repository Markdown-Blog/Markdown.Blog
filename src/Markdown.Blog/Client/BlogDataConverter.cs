using System.Collections.Generic;
using System.Linq;
using Markdown.Blog.Shared.Models;
using Markdown.Blog.Shared.Constants;

namespace Markdown.Blog.Client
{
    /// <summary>
    /// Handles conversion between BlogMetadata and BlogData objects
    /// </summary>
    public static class BlogDataConverter
    {
        /// <summary>
        /// Creates a list of BlogData objects from a BlogIndex
        /// </summary>
        public static List<BlogData> CreateBlogDataList(Division division, BlogIndex blogIndex)
        {
            if (blogIndex?.BlogMetadataList == null)
                return new List<BlogData>();

            return blogIndex.BlogMetadataList
                .Select(metadata => new BlogData(division, metadata))
                .ToList();
        }

        /// <summary>
        /// Filters blog data list by category and subcategory
        /// </summary>
        public static List<BlogData> FilterByCategory(
            List<BlogData> blogDataList,
            string category = null,
            string subcategory = null)
        {
            return blogDataList
                .Where(blog => 
                    (string.IsNullOrEmpty(category) || blog.Hierarchy.Category == category) &&
                    (string.IsNullOrEmpty(subcategory) || blog.Hierarchy.SubCategory == subcategory))
                .ToList();
        }

        /// <summary>
        /// Filters blog data list by tag
        /// </summary>
        public static List<BlogData> FilterByTag(List<BlogData> blogDataList, string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return blogDataList;

            return blogDataList
                .Where(blog => blog.Tags?.Contains(tag) == true)
                .ToList();
        }

        /// <summary>
        /// Orders blog data list by date descending
        /// </summary>
        public static List<BlogData> OrderByDateDescending(List<BlogData> blogDataList)
        {
            return blogDataList
                .OrderByDescending(blog => blog.Date)
                .ToList();
        }

        /// <summary>
        /// Gets the latest blogs up to a specified count
        /// </summary>
        public static List<BlogData> GetLatestBlogs(List<BlogData> blogDataList, int count)
        {
            return OrderByDateDescending(blogDataList)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Filters blogs by cover image status
        /// </summary>
        public static List<BlogData> FilterByCoverImageStatus(
            List<BlogData> blogDataList,
            CoverImageStatus status)
        {
            return status switch
            {
                CoverImageStatus.All => blogDataList,
                CoverImageStatus.NoCover => blogDataList.Where(b => !b.CoverImages.Any()).ToList(),
                CoverImageStatus.WithCover => blogDataList.Where(b => b.CoverImages.Any()).ToList(),
                _ => blogDataList
            };
        }
    }
}