using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Markdown.Blog.Shared.Models;

namespace Markdown.Blog.Infrastructure.Storage
{
    /// <summary>
    /// 博客缓存文档，用于提高查询性能和减少重复计算
    /// </summary>
    public class BlogCacheDocument
    {
        /// <summary>
        /// 缓存键
        /// </summary>
        public string CacheKey { get; set; } = string.Empty;

        /// <summary>
        /// 缓存数据类型
        /// </summary>
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// 缓存的数据内容（JSON格式）
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// 缓存创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 缓存过期时间
        /// </summary>
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(24);

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 访问次数
        /// </summary>
        public int AccessCount { get; set; } = 0;

        /// <summary>
        /// 缓存版本号
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// 缓存标签（用于批量清理）
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// 缓存大小（字节）
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 是否压缩存储
        /// </summary>
        public bool IsCompressed { get; set; } = false;

        /// <summary>
        /// 缓存优先级（用于内存不足时的清理策略）
        /// </summary>
        public int Priority { get; set; } = 1;

        /// <summary>
        /// 依赖的文件路径（当这些文件变更时，缓存失效）
        /// </summary>
        public List<string> Dependencies { get; set; } = new List<string>();

        /// <summary>
        /// 缓存元数据（扩展信息）
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BlogCacheDocument()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="dataType">数据类型</param>
        /// <param name="data">缓存数据</param>
        /// <param name="expirationHours">过期小时数</param>
        public BlogCacheDocument(string cacheKey, string dataType, string data, int expirationHours = 24)
        {
            CacheKey = cacheKey ?? throw new ArgumentNullException(nameof(cacheKey));
            DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
            Data = data ?? string.Empty;
            ExpiresAt = DateTime.UtcNow.AddHours(expirationHours);
            Size = System.Text.Encoding.UTF8.GetByteCount(Data);
        }

        /// <summary>
        /// 检查缓存是否已过期
        /// </summary>
        /// <returns>是否过期</returns>
        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiresAt;
        }

        /// <summary>
        /// 更新最后访问时间和访问次数
        /// </summary>
        public void UpdateAccess()
        {
            LastAccessedAt = DateTime.UtcNow;
            AccessCount++;
        }

        /// <summary>
        /// 延长缓存过期时间
        /// </summary>
        /// <param name="hours">延长的小时数</param>
        public void ExtendExpiration(int hours)
        {
            ExpiresAt = ExpiresAt.AddHours(hours);
        }

        /// <summary>
        /// 添加依赖文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void AddDependency(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && !Dependencies.Contains(filePath))
            {
                Dependencies.Add(filePath);
            }
        }

        /// <summary>
        /// 添加缓存标签
        /// </summary>
        /// <param name="tag">标签</param>
        public void AddTag(string tag)
        {
            if (!string.IsNullOrEmpty(tag) && !Tags.Contains(tag))
            {
                Tags.Add(tag);
            }
        }

        /// <summary>
        /// 设置元数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetMetadata(string key, string value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                Metadata[key] = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 获取元数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public string GetMetadata(string key)
        {
            return Metadata.TryGetValue(key, out var value) ? value : string.Empty;
        }

        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <param name="parameters">参数</param>
        /// <returns>缓存键</returns>
        public static string GenerateCacheKey(string prefix, params string[] parameters)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentNullException(nameof(prefix));

            var key = prefix;
            if (parameters != null && parameters.Length > 0)
            {
                key += ":" + string.Join(":", parameters);
            }

            return key;
        }

        /// <summary>
        /// 创建博客索引缓存
        /// </summary>
        /// <param name="division">分区</param>
        /// <param name="blogIndex">博客索引</param>
        /// <returns>缓存文档</returns>
        public static BlogCacheDocument CreateBlogIndexCache(string division, BlogIndex blogIndex)
        {
            var cacheKey = GenerateCacheKey("blog_index", division);
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(blogIndex);
            
            var cache = new BlogCacheDocument(cacheKey, "BlogIndex", data, 12); // 12小时过期
            cache.AddTag("blog_index");
            cache.AddTag($"division:{division}");
            cache.Priority = 2; // 高优先级
            
            return cache;
        }

        /// <summary>
        /// 创建博客层级缓存
        /// </summary>
        /// <param name="hierarchy">层级结构</param>
        /// <returns>缓存文档</returns>
        public static BlogCacheDocument CreateHierarchyCache(BlogHierarchy hierarchy)
        {
            var cacheKey = GenerateCacheKey("hierarchy", hierarchy.Division, hierarchy.Category, hierarchy.SubCategory);
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(hierarchy);
            
            var cache = new BlogCacheDocument(cacheKey, "BlogHierarchy", data, 24); // 24小时过期
            cache.AddTag("hierarchy");
            cache.AddTag($"division:{hierarchy.Division}");
            cache.Priority = 1; // 中等优先级
            
            return cache;
        }
    }
}