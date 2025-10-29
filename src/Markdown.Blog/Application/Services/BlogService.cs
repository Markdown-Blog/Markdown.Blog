using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markdown.Blog.Application.DTOs;
using Markdown.Blog.Infrastructure.Contracts;
using Markdown.Blog.Shared.Models;

namespace Markdown.Blog.Application.Services
{
    /// <summary>
    /// 博客服务实现
    /// </summary>
    public class BlogService : IBlogService
    {
        private readonly IBlogStorageService _blogStorageService;
        private readonly IBlogCacheService _blogCacheService;

        public BlogService(
            IBlogStorageService blogStorageService,
            IBlogCacheService blogCacheService)
        {
            _blogStorageService = blogStorageService ?? throw new ArgumentNullException(nameof(blogStorageService));
            _blogCacheService = blogCacheService ?? throw new ArgumentNullException(nameof(blogCacheService));
        }

        public async Task<List<BlogMetadataDto>> GetAllBlogsAsync(bool includeDrafts = false)
        {
            // 简化实现 - 返回空列表
            return new List<BlogMetadataDto>();
        }

        public async Task<BlogContentDto?> GetBlogByFilePathAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            try
            {
                var content = await _blogStorageService.GetBlogContentAsync(filePath);
                var metadata = await _blogStorageService.GetBlogMetadataAsync(filePath);

                if (metadata == null || string.IsNullOrEmpty(content))
                    return null;

                return new BlogContentDto
                {
                    Metadata = MapToDto(metadata),
                    MarkdownContent = content,
                    HtmlContent = content, // 简化实现
                    TableOfContents = new List<TableOfContentItem>(),
                    Images = new List<string>(),
                    Links = new List<string>(),
                    RelatedBlogs = new List<BlogMetadataDto>()
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<BlogSearchResponseDto> SearchBlogsAsync(BlogSearchRequestDto request)
        {
            return new BlogSearchResponseDto
            {
                Results = new List<BlogMetadataDto>(),
                TotalCount = 0,
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = 0
            };
        }

        public async Task<List<string>> GetAllTagsAsync()
        {
            return new List<string>();
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            return new List<string>();
        }

        public async Task<List<string>> GetAllAuthorsAsync()
        {
            return new List<string>();
        }

        public async Task<List<BlogMetadataDto>> GetBlogsByTagAsync(string tag, bool includeDrafts = false)
        {
            return new List<BlogMetadataDto>();
        }

        public async Task<List<BlogMetadataDto>> GetBlogsByCategoryAsync(string category, bool includeDrafts = false)
        {
            return new List<BlogMetadataDto>();
        }

        public async Task<List<BlogMetadataDto>> GetBlogsByAuthorAsync(string author, bool includeDrafts = false)
        {
            return new List<BlogMetadataDto>();
        }

        public async Task<List<BlogMetadataDto>> GetLatestBlogsAsync(int count = 10, bool includeDrafts = false)
        {
            return new List<BlogMetadataDto>();
        }

        public async Task<List<BlogMetadataDto>> GetPopularBlogsAsync(int count = 10, bool includeDrafts = false)
        {
            return new List<BlogMetadataDto>();
        }

        public async Task<List<BlogMetadataDto>> GetRelatedBlogsAsync(string filePath, int count = 5)
        {
            return new List<BlogMetadataDto>();
        }

        public async Task<bool> RefreshBlogIndexAsync()
        {
            return true;
        }

        public async Task<BlogStatisticsDto> GetBlogStatisticsAsync()
        {
            return new BlogStatisticsDto
            {
                TotalBlogs = 0,
                PublishedBlogs = 0,
                DraftBlogs = 0,
                TotalWordCount = 0,
                AverageWordCount = 0,
                TotalReadingTimeMinutes = 0,
                AverageReadingTimeMinutes = 0,
                TagStatistics = new List<TagStatistic>(),
                CategoryStatistics = new List<CategoryStatistic>(),
                AuthorStatistics = new List<AuthorStatistic>(),
                MonthlyStatistics = new List<MonthlyStatistic>()
            };
        }

        private static BlogMetadataDto MapToDto(BlogMetadata metadata)
        {
            return new BlogMetadataDto
            {
                FilePath = metadata.FilePath,
                Title = metadata.Title,
                Description = metadata.Description,
                Summary = metadata.Description, // 简化映射
                PublishedDate = metadata.Date,
                Tags = metadata.Tags ?? new List<string>(),
                Category = metadata.Hierarchy?.Category ?? string.Empty,
                Author = string.Empty, // 需要从其他地方获取
                ReadingTimeMinutes = 5, // 默认阅读时间
                WordCount = 0, // 需要计算
                IsDraft = metadata.IsDraft,
                CoverImageUrl = metadata.CoverImages?.FirstOrDefault() ?? string.Empty,
                LastModified = metadata.Date
            };
        }
    }
}