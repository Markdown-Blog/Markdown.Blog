using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Markdown.Blog.Domain.Models;
using Markdown.Blog.Infrastructure.Contracts;
using Markdown.Blog.Infrastructure.Utilities;

namespace Markdown.Blog.Infrastructure.Services
{
    /// <summary>
    /// 博客存储服务实现，基于文件系统的存储操作
    /// </summary>
    public class BlogStorageService : IBlogStorageService
    {
        private readonly string _basePath;

        public BlogStorageService(string basePath)
        {
            _basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        }

        // ========== Blog 文件操作 ==========

        public async Task<string> GetBlogContentAsync(string filePath)
        {
            var fullPath = Path.Combine(_basePath, filePath);
            if (!File.Exists(fullPath))
                return string.Empty;

            return await File.ReadAllTextAsync(fullPath);
        }

        /// <summary>
        /// 根据文件路径获取博客文章元数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>博客文章元数据</returns>
        public async Task<BlogMetadata> GetBlogMetadataAsync(string filePath)
        {
            try
            {
                var content = await GetBlogContentAsync(filePath);
                if (string.IsNullOrEmpty(content))
                    return null;

                return BlogMetadataYamlProcessor.ExtractBlogMetadata(content, filePath);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> WriteBlogAsync(string filePath, string content)
        {
            try
            {
                var fullPath = Path.Combine(_basePath, filePath);
                var directory = Path.GetDirectoryName(fullPath);
                
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(fullPath, content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteBlogAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_basePath, filePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BlogExistsAsync(string filePath)
        {
            var fullPath = Path.Combine(_basePath, filePath);
            return File.Exists(fullPath);
        }

        // ========== Division 目录操作 ==========

        public async Task<IEnumerable<string>> GetDivisionsAsync()
        {
            try
            {
                if (!Directory.Exists(_basePath))
                    return Enumerable.Empty<string>();

                return Directory.GetDirectories(_basePath)
                    .Select(Path.GetFileName)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        public async Task<IEnumerable<string>> GetDivisionFilesAsync(string divisionName)
        {
            try
            {
                var divisionPath = Path.Combine(_basePath, divisionName);
                if (!Directory.Exists(divisionPath))
                    return Enumerable.Empty<string>();

                return Directory.GetFiles(divisionPath, "*.md", SearchOption.AllDirectories)
                    .Select(fullPath => Path.GetRelativePath(_basePath, fullPath))
                    .ToList();
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        public async Task<bool> DivisionExistsAsync(string divisionName)
        {
            var divisionPath = Path.Combine(_basePath, divisionName);
            return Directory.Exists(divisionPath);
        }

        // ========== Index 索引操作 ==========

        public async Task<string> GetDivisionIndexAsync(string divisionName)
        {
            var indexPath = Path.Combine(_basePath, divisionName, "index.json");
            if (!File.Exists(indexPath))
                return string.Empty;

            return await File.ReadAllTextAsync(indexPath);
        }

        public async Task<bool> WriteDivisionIndexAsync(string divisionName, string indexContent)
        {
            try
            {
                var indexPath = Path.Combine(_basePath, divisionName, "index.json");
                var directory = Path.GetDirectoryName(indexPath);
                
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(indexPath, indexContent);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DivisionIndexExistsAsync(string divisionName)
        {
            var indexPath = Path.Combine(_basePath, divisionName, "index.json");
            return File.Exists(indexPath);
        }

        // ========== 原始文件操作 ==========

        public async Task<string> ReadFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_basePath, filePath);
            if (!File.Exists(fullPath))
                return string.Empty;

            return await File.ReadAllTextAsync(fullPath);
        }

        public async Task<bool> WriteFileAsync(string filePath, string content)
        {
            try
            {
                var fullPath = Path.Combine(_basePath, filePath);
                var directory = Path.GetDirectoryName(fullPath);
                
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(fullPath, content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_basePath, filePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> FileExistsAsync(string filePath)
        {
            var fullPath = Path.Combine(_basePath, filePath);
            return File.Exists(fullPath);
        }
    }
}