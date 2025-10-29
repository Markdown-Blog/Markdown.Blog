using System;
using System.Threading.Tasks;
using Markdown.Blog.Application.DTOs;
using Markdown.Blog.Application.Services;

namespace Markdown.Blog.Application.UseCases
{
    /// <summary>
    /// 搜索博客用例
    /// </summary>
    public class SearchBlogsUseCase
    {
        private readonly IBlogService _blogService;

        public SearchBlogsUseCase(IBlogService blogService)
        {
            _blogService = blogService ?? throw new ArgumentNullException(nameof(blogService));
        }

        /// <summary>
        /// 执行博客搜索
        /// </summary>
        /// <param name="request">搜索请求</param>
        /// <returns>搜索结果</returns>
        public async Task<SearchBlogsResponse> ExecuteAsync(BlogSearchRequestDto request)
        {
            if (request == null)
            {
                return new SearchBlogsResponse
                {
                    Success = false,
                    ErrorMessage = "搜索请求不能为空"
                };
            }

            // 验证分页参数
            if (request.PageNumber < 1)
            {
                request.PageNumber = 1;
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                request.PageSize = 10;
            }

            try
            {
                var searchResult = await _blogService.SearchBlogsAsync(request);
                
                return new SearchBlogsResponse
                {
                    Success = true,
                    SearchResult = searchResult
                };
            }
            catch (Exception ex)
            {
                return new SearchBlogsResponse
                {
                    Success = false,
                    ErrorMessage = $"搜索博客时发生错误: {ex.Message}"
                };
            }
        }
    }

    /// <summary>
    /// 搜索博客响应
    /// </summary>
    public class SearchBlogsResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 搜索结果
        /// </summary>
        public BlogSearchResponseDto? SearchResult { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}