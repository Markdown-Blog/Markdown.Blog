using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Markdown.Blog.Infrastructure.Storage;

namespace Markdown.Blog.Infrastructure.Contracts
{
    /// <summary>
    /// 博客缓存服务接口，定义缓存数据的存储、检索和管理操作
    /// </summary>
    public interface IBlogCacheService
    {
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheDocument">缓存文档</param>
        /// <returns>设置结果</returns>
        Task<bool> SetCacheAsync(BlogCacheDocument cacheDocument);

        /// <summary>
        /// 设置缓存（简化版本）
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expirationHours">过期小时数</param>
        /// <param name="dataType">数据类型</param>
        /// <returns>设置结果</returns>
        Task<bool> SetCacheAsync(string key, string value, int expirationHours = 24, string dataType = "String");

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存文档</returns>
        Task<BlogCacheDocument> GetCacheAsync(string key);

        /// <summary>
        /// 获取缓存值（简化版本）
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        Task<string> GetCacheValueAsync(string key);

        /// <summary>
        /// 获取缓存值（泛型版本）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        Task<T> GetCacheValueAsync<T>(string key) where T : class;

        /// <summary>
        /// 批量获取缓存
        /// </summary>
        /// <param name="keys">缓存键列表</param>
        /// <returns>缓存文档字典</returns>
        Task<Dictionary<string, BlogCacheDocument>> GetCacheBatchAsync(IEnumerable<string> keys);

        /// <summary>
        /// 检查缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>删除结果</returns>
        Task<bool> RemoveCacheAsync(string key);

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="keys">缓存键列表</param>
        /// <returns>删除结果</returns>
        Task<bool> RemoveCacheBatchAsync(IEnumerable<string> keys);

        /// <summary>
        /// 根据标签删除缓存
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>删除的缓存数量</returns>
        Task<int> RemoveCacheByTagAsync(string tag);

        /// <summary>
        /// 根据数据类型删除缓存
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <returns>删除的缓存数量</returns>
        Task<int> RemoveCacheByDataTypeAsync(string dataType);

        /// <summary>
        /// 根据模式删除缓存
        /// </summary>
        /// <param name="pattern">键模式（支持通配符）</param>
        /// <returns>删除的缓存数量</returns>
        Task<int> RemoveCacheByPatternAsync(string pattern);

        /// <summary>
        /// 延长缓存过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="hours">延长的小时数</param>
        /// <returns>延长结果</returns>
        Task<bool> ExtendCacheExpirationAsync(string key, int hours);

        /// <summary>
        /// 刷新缓存（重置过期时间）
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>刷新结果</returns>
        Task<bool> RefreshCacheAsync(string key);

        /// <summary>
        /// 获取缓存统计信息
        /// </summary>
        /// <returns>缓存统计信息</returns>
        Task<Dictionary<string, object>> GetCacheStatisticsAsync();

        /// <summary>
        /// 获取缓存键列表
        /// </summary>
        /// <param name="pattern">键模式（支持通配符）</param>
        /// <param name="limit">限制数量</param>
        /// <returns>缓存键列表</returns>
        Task<IEnumerable<string>> GetCacheKeysAsync(string pattern = "*", int limit = 1000);

        /// <summary>
        /// 根据标签获取缓存键列表
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="limit">限制数量</param>
        /// <returns>缓存键列表</returns>
        Task<IEnumerable<string>> GetCacheKeysByTagAsync(string tag, int limit = 1000);

        /// <summary>
        /// 根据数据类型获取缓存键列表
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="limit">限制数量</param>
        /// <returns>缓存键列表</returns>
        Task<IEnumerable<string>> GetCacheKeysByDataTypeAsync(string dataType, int limit = 1000);

        /// <summary>
        /// 清理过期缓存
        /// </summary>
        /// <returns>清理的缓存数量</returns>
        Task<int> CleanupExpiredCacheAsync();

        /// <summary>
        /// 清理最少使用的缓存（LRU策略）
        /// </summary>
        /// <param name="count">清理数量</param>
        /// <returns>清理的缓存数量</returns>
        Task<int> CleanupLeastUsedCacheAsync(int count);

        /// <summary>
        /// 清理低优先级缓存
        /// </summary>
        /// <param name="maxPriority">最大优先级</param>
        /// <returns>清理的缓存数量</returns>
        Task<int> CleanupLowPriorityCacheAsync(int maxPriority = 1);

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        /// <returns>清空结果</returns>
        Task<bool> ClearAllCacheAsync();

        /// <summary>
        /// 获取缓存大小（字节）
        /// </summary>
        /// <returns>缓存总大小</returns>
        Task<long> GetCacheSizeAsync();

        /// <summary>
        /// 获取缓存数量
        /// </summary>
        /// <returns>缓存总数量</returns>
        Task<long> GetCacheCountAsync();

        /// <summary>
        /// 预热缓存
        /// </summary>
        /// <param name="keys">需要预热的缓存键列表</param>
        /// <returns>预热结果</returns>
        Task<bool> WarmupCacheAsync(IEnumerable<string> keys);

        /// <summary>
        /// 导出缓存数据
        /// </summary>
        /// <param name="exportPath">导出路径</param>
        /// <param name="pattern">键模式（支持通配符）</param>
        /// <returns>导出结果</returns>
        Task<bool> ExportCacheAsync(string exportPath, string pattern = "*");

        /// <summary>
        /// 导入缓存数据
        /// </summary>
        /// <param name="importPath">导入路径</param>
        /// <param name="overwrite">是否覆盖现有缓存</param>
        /// <returns>导入结果</returns>
        Task<bool> ImportCacheAsync(string importPath, bool overwrite = false);

        /// <summary>
        /// 验证缓存完整性
        /// </summary>
        /// <returns>验证结果</returns>
        Task<bool> ValidateCacheIntegrityAsync();

        /// <summary>
        /// 压缩缓存存储
        /// </summary>
        /// <returns>压缩结果</returns>
        Task<bool> CompressCacheStorageAsync();
    }
}