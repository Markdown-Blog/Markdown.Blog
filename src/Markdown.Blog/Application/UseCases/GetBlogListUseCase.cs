using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Markdown.Blog.Application.DTOs;
using Markdown.Blog.Application.Services;

namespace Markdown.Blog.Application.UseCases
{
    /// <summary>
    /// 获取博客列表用例
    /// </summary>
    public class GetBlogListUseCase
    {
        private readonly IBlogService _blogService;

        public GetBlogListUseCase(IBlogService blogService)
        {
            _blogService = blogService ?? throw new ArgumentNullException(nameof(blogService));
        }

        /// <summary>
        /// 获取所有博客
        /// </summary>
        /// <param name="includeDrafts">是否包含草稿</param>
        /// <returns>博客列表</returns>
        public async Task<GetBlogListResponse> GetAllBlogsAsync(bool includeDrafts = false)
        {
            try
            {
                var blogs = await _blogService.GetAllBlogsAsync(includeDrafts);
                
                return new GetBlogListResponse
                {
                    Success = true,
                    Blogs = blogs
                };
            }
            catch (Exception ex)
            {
                return new GetBlogListResponse
                {
                    Success = false,
                    ErrorMessage = $"获取博客列表时发生错误: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据标签获取博客
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="includeDrafts">是否包含草稿</param>
        /// <returns>博客列表</returns>
        public async Task<GetBlogListResponse> GetBlogsByTagAsync(string tag, bool includeDrafts = false)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return new GetBlogListResponse
                {
                    Success = false,
                    ErrorMessage = "标签不能为空"
                };
            }

            try
            {
                var blogs = await _blogService.GetBlogsByTagAsync(tag, includeDrafts);
                
                return new GetBlogListResponse
                {
                    Success = true,
                    Blogs = blogs
                };
            }
            catch (Exception ex)
            {
                return new GetBlogListResponse
                {
                    Success = false,
                    ErrorMessage = $"根据标签获取博客时发生错误: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据分类获取博客
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="includeDrafts">是否包含草稿</param>
        /// <returns>博客列表</returns>
        public async Task<GetBlogListResponse> GetBlogsByCategoryAsync(string category, bool includeDrafts = false)
        {
            if (string.IsNullOrEmpty(category))
            {
                return new GetBlogListResponse
                {
                    Success = false,
                    ErrorMessage = "分类不能为空"
                };
            }

            try
            {
                var blogs = await _blogService.GetBlogsByCategoryAsync(category, includeDrafts);
                
                return new GetBlogListResponse
                {
                    Success = true,
                    Blogs = blogs
                };
            }
            catch (Exception ex)
            {
                return new GetBlogListResponse
                {
                    Success = false,
                    ErrorMessage = $"根据分类获取博客时发生错误: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据作者获取博客
        /// </summary>
        /// <param name="author">作者</param>
        /// <param name="includeDrafts">是否包含草稿</param>
        /// <returns>博客列表</returns>
        public async Task<GetBlogListResponse> GetBlogsByAuthorAsync(string author, bool includeDrafts = false)
        {
            if (string.IsNullOrEmpty(author))
            {
                return new GetBlogListResponse
                {
                    Success = false,
                    ErrorMessage = "作者不能为空"
                };
            }

            try
            {
                var blogs = await _blogService.GetBlogsByAuthorAsync(author, includeDrafts);
                
                return new GetBlogListResponse
                {
                    Success = true,
                    Blogs = blogs
                };
            }
            catch (Exception ex)
            {
                return new GetBlogListResponse
                {
                    Success = false,
                    ErrorMessage = $"根据作者获取博客时发生错误: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取最新博客
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="includeDrafts">是否包含草稿</param>
        /// <returns>博客列表</returns>
        public async Task<GetBlogListResponse> GetLatestBlogsAsync(int count = 10, bool includeDrafts = false)
        {
            if (count <= 0)
            {
                count = 10;
            }

            try
            {
                var blogs = await _blogService.GetLatestBlogsAsync(count, includeDrafts);
                
                return new GetBlogListResponse
                {
                    Success = true,
                    Blogs = blogs
                };
            }
            catch (Exception ex)
            {
                return new GetBlogListResponse
                {
                    Success = false,
                    ErrorMessage = $"获取最新博客时发生错误: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取热门博客
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="includeDrafts">是否包含草稿</param>
        /// <returns>博客列表</returns>
        public async Task<GetBlogListResponse> GetPopularBlogsAsync(int count = 10, bool includeDrafts = false)
        {
            if (count <= 0)
            {
                count = 10;
            }

            try
            {
                var blogs = await _blogService.GetPopularBlogsAsync(count, includeDrafts);
                
                return new GetBlogListResponse
                {
                    Success = true,
                    Blogs = blogs
                };
            }
            catch (Exception ex)
            {
                return new GetBlogListResponse
                {
                    Success = false,
                    ErrorMessage = $"获取热门博客时发生错误: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取相关博客
        /// </summary>
        /// <param name="filePath">当前博客文件路径</param>
        /// <param name="count">数量</param>
        /// <returns>博客列表</returns>
        public async Task<GetBlogListResponse> GetRelatedBlogsAsync(string filePath, int count = 5)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return new GetBlogListResponse
                {
                    Success = false,
                    ErrorMessage = "文件路径不能为空"
                };
            }

            if (count <= 0)
            {
                count = 5;
            }

            try
            {
                var blogs = await _blogService.GetRelatedBlogsAsync(filePath, count);
                
                return new GetBlogListResponse
                {
                    Success = true,
                    Blogs = blogs
                };
            }
            catch (Exception ex)
            {
                return new GetBlogListResponse
                {
                    Success = false,
                    ErrorMessage = $"获取相关博客时发生错误: {ex.Message}"
                };
            }
        }
    }

    /// <summary>
    /// 获取博客列表响应
    /// </summary>
    public class GetBlogListResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 博客列表
        /// </summary>
        public List<BlogMetadataDto> Blogs { get; set; } = new List<BlogMetadataDto>();

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}