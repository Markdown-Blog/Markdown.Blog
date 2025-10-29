using System;
using System.Threading.Tasks;
using Markdown.Blog.Application.DTOs;
using Markdown.Blog.Application.Services;

namespace Markdown.Blog.Application.UseCases
{
    /// <summary>
    /// 获取博客统计信息用例
    /// </summary>
    public class GetBlogStatisticsUseCase
    {
        private readonly IBlogService _blogService;

        public GetBlogStatisticsUseCase(IBlogService blogService)
        {
            _blogService = blogService ?? throw new ArgumentNullException(nameof(blogService));
        }

        /// <summary>
        /// 执行获取博客统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        public async Task<GetBlogStatisticsResponse> ExecuteAsync()
        {
            try
            {
                var statistics = await _blogService.GetBlogStatisticsAsync();
                
                return new GetBlogStatisticsResponse
                {
                    Success = true,
                    Statistics = statistics
                };
            }
            catch (Exception ex)
            {
                return new GetBlogStatisticsResponse
                {
                    Success = false,
                    ErrorMessage = $"获取博客统计信息时发生错误: {ex.Message}"
                };
            }
        }
    }

    /// <summary>
    /// 获取博客统计信息响应
    /// </summary>
    public class GetBlogStatisticsResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 统计信息
        /// </summary>
        public BlogStatisticsDto? Statistics { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}