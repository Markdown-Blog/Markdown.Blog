using System.Collections.Generic;
using System.Linq;
using Markdown.Blog.Domain.Models;

namespace Markdown.Blog.Client
{
    /// <summary>
    /// Provides analysis and statistics for blog data
    /// </summary>
    public static class BlogDataAnalyzer
    {
        /// <summary>
        /// Gets all categories and subcategories with their blog counts
        /// </summary>
        public static Dictionary<string, Dictionary<string, int>> GetCategoriesAndSubcategoriesWithBlogCount(
            List<BlogData> blogDataList)
        {
            return blogDataList
                .GroupBy(blog => blog.Hierarchy.Category)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(blog => blog.Hierarchy.SubCategory)
                          .Where(sg => !string.IsNullOrEmpty(sg.Key))
                          .ToDictionary(sg => sg.Key, sg => sg.Count())
                );
        }

        /// <summary>
        /// Gets all tags with their usage counts
        /// </summary>
        public static Dictionary<string, int> GetTagsWithCount(List<BlogData> blogDataList)
        {
            return blogDataList
                .SelectMany(blog => blog.Tags ?? Enumerable.Empty<string>())
                .GroupBy(tag => tag)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        /// <summary>
        /// Organizes tags by their usage frequency
        /// </summary>
        public static (List<string> CommonTags, List<string> RareTags) OrganizeTagsByFrequency(
            List<BlogData> blogDataList,
            int commonTagThreshold = 3)
        {
            var tagCounts = GetTagsWithCount(blogDataList);
            
            var commonTags = tagCounts
                .Where(kvp => kvp.Value >= commonTagThreshold)
                .Select(kvp => kvp.Key)
                .OrderBy(tag => tag)
                .ToList();

            var rareTags = tagCounts
                .Where(kvp => kvp.Value < commonTagThreshold)
                .Select(kvp => kvp.Key)
                .OrderBy(tag => tag)
                .ToList();

            return (commonTags, rareTags);
        }

        /// <summary>
        /// Gets hierarchy statistics for all blogs
        /// </summary>
        public static Dictionary<string, int> GetHierarchyStatistics(List<BlogData> blogDataList)
        {
            var stats = new Dictionary<string, int>();

            foreach (var blog in blogDataList)
            {
                var hierarchy = blog.Hierarchy;
                if (!string.IsNullOrEmpty(hierarchy.Division))
                    IncrementCount($"Division:{hierarchy.Division}");
                if (!string.IsNullOrEmpty(hierarchy.Category))
                    IncrementCount($"Category:{hierarchy.Category}");
                if (!string.IsNullOrEmpty(hierarchy.SubCategory))
                    IncrementCount($"SubCategory:{hierarchy.SubCategory}");
            }

            return stats;

            void IncrementCount(string key)
            {
                if (!stats.ContainsKey(key))
                    stats[key] = 0;
                stats[key]++;
            }
        }
    }
}