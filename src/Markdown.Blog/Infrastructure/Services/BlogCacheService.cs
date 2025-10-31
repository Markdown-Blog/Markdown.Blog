using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Markdown.Blog.Infrastructure.Contracts;
using Markdown.Blog.Infrastructure.Storage;
using Markdown.Blog.Infrastructure.Utilities;

namespace Markdown.Blog.Infrastructure.Services
{
    /// <summary>
    /// 博客缓存服务实现，基于内存和文件的缓存操作
    /// </summary>
    public class BlogCacheService : IBlogCacheService
    {
        private readonly ConcurrentDictionary<string, BlogCacheDocument> _cache;
        private readonly string _cacheDirectory;
        private readonly object _lockObject = new object();

        public BlogCacheService(string cacheDirectory = null)
        {
            _cache = new ConcurrentDictionary<string, BlogCacheDocument>();
            _cacheDirectory = cacheDirectory ?? Path.Combine(Path.GetTempPath(), "BlogCache");
            
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }
        }

        // ========== 基础缓存操作 ==========

        public async Task<bool> SetCacheAsync(BlogCacheDocument cacheDocument)
        {
            try
            {
                _cache.AddOrUpdate(cacheDocument.CacheKey, cacheDocument, (k, existing) => cacheDocument);
                await SaveCacheDocumentAsync(cacheDocument);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetCacheAsync(string key, string value, int expirationHours = 24, string dataType = "String")
        {
            var document = new BlogCacheDocument
            {
                CacheKey = key,
                DataType = dataType,
                Data = value,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(expirationHours),
                LastAccessedAt = DateTime.UtcNow,
                AccessCount = 0,
                Version = 1,
                Size = value?.Length ?? 0,
                IsCompressed = false,
                Priority = 1
            };

            return await SetCacheAsync(document);
        }

        public async Task<BlogCacheDocument> GetCacheAsync(string key)
        {
            if (_cache.TryGetValue(key, out var document))
            {
                if (document.IsExpired())
                {
                    await RemoveCacheAsync(key);
                    return null;
                }

                document.UpdateAccess();
                return document;
            }

            // 尝试从文件加载
            var loadedDocument = await LoadCacheDocumentAsync(key);
            if (loadedDocument != null && !loadedDocument.IsExpired())
            {
                _cache.TryAdd(key, loadedDocument);
                loadedDocument.UpdateAccess();
                return loadedDocument;
            }

            return null;
        }

        public async Task<string> GetCacheValueAsync(string key)
        {
            var document = await GetCacheAsync(key);
            return document?.Data;
        }

        public async Task<T> GetCacheValueAsync<T>(string key) where T : class
        {
            var document = await GetCacheAsync(key);
            if (document?.Data != null)
            {
                return BlogMetadataDeserializer.DeserializeFromJson<T>(document.Data);
            }
            return null;
        }

        public async Task SetCacheAsync(string key, object data, TimeSpan? expiration = null)
        {
            var document = new BlogCacheDocument
            {
                CacheKey = key,
                DataType = data?.GetType().Name ?? "null",
                Data = BlogMetadataDeserializer.SerializeToJson(data),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : DateTime.UtcNow.AddHours(24),
                LastAccessedAt = DateTime.UtcNow,
                AccessCount = 0,
                Version = 1,
                Size = BlogMetadataDeserializer.SerializeToJson(data)?.Length ?? 0,
                IsCompressed = false,
                Priority = 1
            };

            _cache.AddOrUpdate(key, document, (k, existing) => document);
            await SaveCacheDocumentAsync(document);
        }

        public async Task<T> GetCacheAsync<T>(string key) where T : class
        {
            if (_cache.TryGetValue(key, out var document))
            {
                if (document.IsExpired())
                {
                    await RemoveCacheAsync(key);
                    return default(T);
                }

                document.UpdateAccess();
                return BlogMetadataDeserializer.DeserializeFromJson<T>(document.Data);
            }

            // 尝试从文件加载
            var loadedDocument = await LoadCacheDocumentAsync(key);
            if (loadedDocument != null && !loadedDocument.IsExpired())
            {
                _cache.TryAdd(key, loadedDocument);
                loadedDocument.UpdateAccess();
                return BlogMetadataDeserializer.DeserializeFromJson<T>(loadedDocument.Data);
            }

            return default(T);
        }

        // ========== 批量操作 ==========

        public async Task SetMultipleCacheAsync(Dictionary<string, object> items, TimeSpan? expiration = null)
        {
            var tasks = items.Select(kvp => SetCacheAsync(kvp.Key, kvp.Value, expiration));
            await Task.WhenAll(tasks);
        }

        public async Task<Dictionary<string, T>> GetMultipleCacheAsync<T>(IEnumerable<string> keys) where T : class
        {
            var result = new Dictionary<string, T>();
            var tasks = keys.Select(async key =>
            {
                var value = await GetCacheAsync<T>(key);
                if (value != null)
                {
                    lock (result)
                    {
                        result[key] = value;
                    }
                }
            });

            await Task.WhenAll(tasks);
            return result;
        }

        public async Task<Dictionary<string, BlogCacheDocument>> GetCacheBatchAsync(IEnumerable<string> keys)
        {
            var result = new Dictionary<string, BlogCacheDocument>();
            foreach (var key in keys)
            {
                var document = await GetCacheAsync(key);
                if (document != null)
                {
                    result[key] = document;
                }
            }
            return result;
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (_cache.ContainsKey(key))
            {
                var document = _cache[key];
                if (!document.IsExpired())
                {
                    return true;
                }
                else
                {
                    await RemoveCacheAsync(key);
                    return false;
                }
            }

            // 检查文件是否存在
            var filePath = Path.Combine(_cacheDirectory, $"{key}.cache");
            return File.Exists(filePath);
        }

        // ========== 删除操作 ==========

        public async Task<bool> RemoveCacheBatchAsync(IEnumerable<string> keys)
        {
            var tasks = keys.Select(RemoveCacheAsync);
            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }

        public async Task<int> RemoveCacheByTagAsync(string tag)
        {
            var keysToRemove = _cache
                .Where(kvp => kvp.Value.Tags != null && kvp.Value.Tags.Contains(tag))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                await RemoveCacheAsync(key);
            }

            return keysToRemove.Count;
        }

        public async Task<int> RemoveCacheByDataTypeAsync(string dataType)
        {
            var keysToRemove = _cache
                .Where(kvp => kvp.Value.DataType == dataType)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                await RemoveCacheAsync(key);
            }

            return keysToRemove.Count;
        }

        public async Task<int> RemoveCacheByPatternAsync(string pattern)
        {
            var keysToRemove = _cache.Keys
                .Where(key => MatchesPattern(key, pattern))
                .ToList();

            foreach (var key in keysToRemove)
            {
                await RemoveCacheAsync(key);
            }

            return keysToRemove.Count;
        }

        // ========== 过期时间操作 ==========

        public async Task<bool> ExtendCacheExpirationAsync(string key, int hours)
        {
            if (_cache.TryGetValue(key, out var document))
            {
                document.ExpiresAt = document.ExpiresAt.AddHours(hours);
                await SaveCacheDocumentAsync(document);
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveCacheAsync(string key)
        {
            var removed = _cache.TryRemove(key, out _);
            if (removed)
            {
                await DeleteCacheFileAsync(key);
            }
            return removed;
        }

        public async Task RemoveMultipleCacheAsync(IEnumerable<string> keys)
        {
            var tasks = keys.Select(RemoveCacheAsync);
            await Task.WhenAll(tasks);
        }

        // ========== 过期时间操作 ==========

        public async Task<bool> ExtendExpirationAsync(string key, TimeSpan extension)
        {
            if (_cache.TryGetValue(key, out var document))
            {
                document.ExpiresAt = document.ExpiresAt.Add(extension);
                await SaveCacheDocumentAsync(document);
                return true;
            }
            return false;
        }

        public async Task<bool> RefreshCacheAsync(string key)
        {
            if (_cache.TryGetValue(key, out var document))
            {
                document.LastAccessedAt = DateTime.UtcNow;
                document.AccessCount++;
                await SaveCacheDocumentAsync(document);
                return true;
            }
            return false;
        }

        // ========== 统计和信息 ==========

        public async Task<Dictionary<string, object>> GetCacheStatisticsAsync()
        {
            var totalSize = _cache.Values.Sum(doc => doc.Size);
            var expiredCount = _cache.Values.Count(doc => doc.IsExpired());
            
            return new Dictionary<string, object>
            {
                ["TotalItems"] = _cache.Count,
                ["TotalSize"] = totalSize,
                ["ExpiredItems"] = expiredCount,
                ["HitRate"] = CalculateHitRate(),
                ["AverageAccessCount"] = _cache.Values.Any() ? _cache.Values.Average(doc => doc.AccessCount) : 0
            };
        }

        public async Task<IEnumerable<string>> GetCacheKeysAsync(string pattern = "*", int limit = 1000)
        {
            var keys = _cache.Keys.AsEnumerable();
            if (!string.IsNullOrEmpty(pattern) && pattern != "*")
            {
                keys = keys.Where(key => MatchesPattern(key, pattern));
            }
            return keys.Take(limit).ToList();
        }

        public async Task<IEnumerable<string>> GetCacheKeysByTagAsync(string tag, int limit = 1000)
        {
            var keys = _cache
                .Where(kvp => kvp.Value.Tags != null && kvp.Value.Tags.Contains(tag))
                .Select(kvp => kvp.Key)
                .Take(limit);
            return keys.ToList();
        }

        public async Task<IEnumerable<string>> GetCacheKeysByDataTypeAsync(string dataType, int limit = 1000)
        {
            var keys = _cache
                .Where(kvp => kvp.Value.DataType == dataType)
                .Select(kvp => kvp.Key)
                .Take(limit);
            return keys.ToList();
        }

        // ========== 清理操作 ==========

        public async Task<int> CleanupExpiredCacheAsync()
        {
            var expiredKeys = _cache
                .Where(kvp => kvp.Value.IsExpired())
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                await RemoveCacheAsync(key);
            }
            
            return expiredKeys.Count;
        }

        public async Task<int> CleanupLeastUsedCacheAsync(int count)
        {
            var leastUsedKeys = _cache
                .OrderBy(kvp => kvp.Value.LastAccessedAt)
                .Take(count)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in leastUsedKeys)
            {
                await RemoveCacheAsync(key);
            }
            
            return leastUsedKeys.Count;
        }

        public async Task<int> CleanupLowPriorityCacheAsync(int maxPriority = 1)
        {
            var lowPriorityKeys = _cache
                .Where(kvp => kvp.Value.Priority <= maxPriority)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in lowPriorityKeys)
            {
                await RemoveCacheAsync(key);
            }
            
            return lowPriorityKeys.Count;
        }

        public async Task<bool> ClearAllCacheAsync()
        {
            try
            {
                _cache.Clear();
                if (Directory.Exists(_cacheDirectory))
                {
                    Directory.Delete(_cacheDirectory, true);
                    Directory.CreateDirectory(_cacheDirectory);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ========== 大小和数量 ==========

        public async Task<long> GetCacheSizeAsync()
        {
            return _cache.Values.Sum(doc => doc.Size);
        }

        public async Task<long> GetCacheCountAsync()
        {
            return _cache.Count;
        }

        // ========== 预热操作 ==========

        public async Task<bool> WarmupCacheAsync(IEnumerable<string> keys)
        {
            try
            {
                foreach (var key in keys)
                {
                    // 预热逻辑：检查缓存是否存在，如果不存在则创建默认缓存
                    if (!await ExistsAsync(key))
                    {
                        await SetCacheAsync(key, $"Warmed up cache for {key}");
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ========== 导入导出 ==========

        public async Task<bool> ExportCacheAsync(string exportPath, string pattern = "*")
        {
            try
            {
                var cacheData = new List<BlogCacheDocument>();
                
                foreach (var kvp in _cache)
                {
                    if (!kvp.Value.IsExpired() && MatchesPattern(kvp.Key, pattern))
                    {
                        cacheData.Add(kvp.Value);
                    }
                }

                var json = JsonSerializer.Serialize(cacheData, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(exportPath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task ImportCacheAsync(Dictionary<string, object> data)
        {
            await SetMultipleCacheAsync(data);
        }

        public async Task<bool> ImportCacheAsync(string importPath, bool overwrite = false)
        {
            try
            {
                if (!File.Exists(importPath))
                    return false;

                var importData = await File.ReadAllTextAsync(importPath);
                var cacheData = BlogMetadataDeserializer.DeserializeFromJson<Dictionary<string, object>>(importData);
                
                if (cacheData == null)
                    return false;

                foreach (var kvp in cacheData)
                {
                    if (!overwrite && _cache.ContainsKey(kvp.Key))
                        continue;
                        
                    await SetCacheAsync(kvp.Key, kvp.Value);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // ========== 验证和压缩 ==========

        public async Task<bool> ValidateCacheIntegrityAsync()
        {
            try
            {
                foreach (var document in _cache.Values)
                {
                    if (string.IsNullOrEmpty(document.CacheKey) || string.IsNullOrEmpty(document.Data))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CompressCacheStorageAsync()
        {
            try
            {
                // 清理过期缓存
                await CleanupExpiredCacheAsync();
                
                // 压缩文件存储（简单实现）
                var allDocuments = _cache.Values.ToList();
                foreach (var document in allDocuments)
                {
                    if (!document.IsCompressed && document.Size > 1024) // 大于1KB的数据进行压缩标记
                    {
                        document.IsCompressed = true;
                        await SaveCacheDocumentAsync(document);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查键是否匹配模式（支持通配符）
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="pattern">模式</param>
        /// <returns>是否匹配</returns>
        private bool MatchesPattern(string key, string pattern)
        {
            if (pattern == "*") return true;
            
            // 简单的通配符匹配实现
            var regex = pattern.Replace("*", ".*").Replace("?", ".");
            return System.Text.RegularExpressions.Regex.IsMatch(key, $"^{regex}$");
        }

        private async Task SaveCacheDocumentAsync(BlogCacheDocument document)
        {
            try
            {
                var filePath = GetCacheFilePath(document.CacheKey);
                var json = BlogMetadataDeserializer.SerializeToJson(document);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch
            {
                // 忽略文件保存错误
            }
        }


        private async Task<BlogCacheDocument> LoadCacheDocumentAsync(string key)
        {
            try
            {
                var filePath = GetCacheFilePath(key);
                if (!File.Exists(filePath))
                    return null;

                var json = await File.ReadAllTextAsync(filePath);
                return BlogMetadataDeserializer.DeserializeFromJson<BlogCacheDocument>(json);
            }
            catch
            {
                return null;
            }
        }

        private async Task DeleteCacheFileAsync(string key)
        {
            try
            {
                var filePath = GetCacheFilePath(key);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // 忽略文件删除错误
            }
        }

        private string GetCacheFilePath(string key)
        {
            var safeKey = string.Join("_", key.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(_cacheDirectory, $"{safeKey}.cache");
        }

        private double CalculateHitRate()
        {
            var totalAccess = _cache.Values.Sum(doc => doc.AccessCount);
            return totalAccess > 0 ? (double)_cache.Count / totalAccess : 0;
        }
    }
}