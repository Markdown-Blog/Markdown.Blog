// Services 层说明: Markdown.Blog/src/Markdown.Blog/Application/Services/README.md
// DTOs 设计文档: Markdown.Blog/src/Markdown.Blog/Application/DTOs/README.md
// 目的：本服务接口返回标准化 DTOs，供客户端/工具/未来 API 复用。
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Markdown.Blog.Application.DTOs;

namespace Markdown.Blog.Application.Services
{
    /// <summary>
    /// 博客服务接口
    /// </summary>
    public interface IBlogService
    {
        /// <summary>
        /// 获取所有博客元数据
        /// </summary>
        /// <param name="includeDrafts">是否包含草稿</param>
        /// <returns>博客元数据列表</returns>
        Task<List<BlogMetadataDto>> GetAllBlogsAsync(bool includeDrafts = false);

        /// <summary>
        /// 根据文件路径获取博客内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>博客内容</returns>
        Task<BlogContentDto?> GetBlogByFilePathAsync(string filePath);

        /// <summary>
        /// 搜索博客
        /// </summary>
        /// <param name="request">搜索请求</param>
        /// <returns>搜索结果</returns>
        Task<BlogSearchResponseDto> SearchBlogsAsync(BlogSearchRequestDto request);

        /// <summary>
        /// 获取博客标签列表
        /// </summary>
        /// <returns>标签列表</returns>
        Task<List<string>> GetAllTagsAsync();

        /// <summary>
        /// 获取博客分类列表
        /// </summary>
        /// <returns>分类列表</returns>
        Task<List<string>> GetAllCategoriesAsync();

        /// <summary>
        /// 获取博客作者列表
        /// </summary>
        /// <returns>作者列表</returns>
        Task<List<string>> GetAllAuthorsAsync();

        /// <summary>
        /// 根据标签获取博客
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="includeDrafts">是否包含草稿</param>
        /// <returns>博客列表</returns>
        Task<List<BlogMetadataDto>> GetBlogsByTagAsync(string tag, bool includeDrafts = false);

        /// <summary>
        /// 根据分类获取博客
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="includeDrafts">是否包含草稿</param>
        /// <returns>博客列表</returns>
        Task<List<BlogMetadataDto>> GetBlogsByCategoryAsync(string category, bool includeDrafts = false);

        /// <summary>
        /// 根据作者获取博客
        /// </summary>
        /// <param name="author">作者</param>
        /// <param name="includeDrafts">是否包含草稿</param>
        /// <returns>博客列表</returns>
        Task<List<BlogMetadataDto>> GetBlogsByAuthorAsync(string author, bool includeDrafts = false);

        /// <summary>
        /// 获取最新博客
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="includeDrafts">是否包含草稿</param>
        /// <returns>博客列表</returns>
        Task<List<BlogMetadataDto>> GetLatestBlogsAsync(int count = 10, bool includeDrafts = false);

        /// <summary>
        /// 获取热门博客
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="includeDrafts">是否包含草稿</param>
        /// <returns>博客列表</returns>
        Task<List<BlogMetadataDto>> GetPopularBlogsAsync(int count = 10, bool includeDrafts = false);

        /// <summary>
        /// 获取相关博客
        /// </summary>
        /// <param name="filePath">当前博客文件路径</param>
        /// <param name="count">数量</param>
        /// <returns>相关博客列表</returns>
        Task<List<BlogMetadataDto>> GetRelatedBlogsAsync(string filePath, int count = 5);

        /// <summary>
        /// 刷新博客索引
        /// </summary>
        /// <returns>刷新结果</returns>
        Task<bool> RefreshBlogIndexAsync();

        /// <summary>
        /// 获取博客统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        Task<BlogStatisticsDto> GetBlogStatisticsAsync();
    }
}