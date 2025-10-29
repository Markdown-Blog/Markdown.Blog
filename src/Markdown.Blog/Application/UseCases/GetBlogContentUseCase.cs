using System;
using System.Threading.Tasks;
using Markdown.Blog.Application.DTOs;
using Markdown.Blog.Application.Services;

namespace Markdown.Blog.Application.UseCases
{
    /// <summary>
    /// 获取博客内容用例
    /// </summary>
    public class GetBlogContentUseCase
    {
        private readonly IBlogService _blogService;

        public GetBlogContentUseCase(IBlogService blogService)
        {
            _blogService = blogService ?? throw new ArgumentNullException(nameof(blogService));
        }

        /// <summary>
        /// 执行获取博客内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>博客内容</returns>
        public async Task<GetBlogContentResponse> ExecuteAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return new GetBlogContentResponse
                {
                    Success = false,
                    ErrorMessage = "文件路径不能为空"
                };
            }

            try
            {
                var blogContent = await _blogService.GetBlogByFilePathAsync(filePath);
                
                if (blogContent == null)
                {
                    return new GetBlogContentResponse
                    {
                        Success = false,
                        ErrorMessage = "未找到指定的博客文章"
                    };
                }

                return new GetBlogContentResponse
                {
                    Success = true,
                    BlogContent = blogContent
                };
            }
            catch (Exception ex)
            {
                return new GetBlogContentResponse
                {
                    Success = false,
                    ErrorMessage = $"获取博客内容时发生错误: {ex.Message}"
                };
            }
        }
    }

    /// <summary>
    /// 获取博客内容响应
    /// </summary>
    public class GetBlogContentResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 博客内容
        /// </summary>
        public BlogContentDto? BlogContent { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}