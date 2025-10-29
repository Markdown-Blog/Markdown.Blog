using System;
using System.Collections.Generic;

namespace Markdown.Blog.Application.DTOs
{
    /// <summary>
    /// 博客搜索请求
    /// </summary>
    public class BlogSearchRequestDto
    {
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// 标签过滤
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// 分类过滤
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// 作者过滤
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 是否包含草稿
        /// </summary>
        public bool IncludeDrafts { get; set; } = false;

        /// <summary>
        /// 排序字段
        /// </summary>
        public BlogSortField SortBy { get; set; } = BlogSortField.PublishedDate;

        /// <summary>
        /// 排序方向
        /// </summary>
        public SortDirection SortDirection { get; set; } = SortDirection.Descending;

        /// <summary>
        /// 页码（从1开始）
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// 博客搜索响应
    /// </summary>
    public class BlogSearchResponseDto
    {
        /// <summary>
        /// 搜索结果
        /// </summary>
        public List<BlogMetadataDto> Results { get; set; } = new List<BlogMetadataDto>();

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// 搜索耗时（毫秒）
        /// </summary>
        public long SearchTimeMs { get; set; }

        /// <summary>
        /// 相关标签建议
        /// </summary>
        public List<string> SuggestedTags { get; set; } = new List<string>();

        /// <summary>
        /// 相关分类建议
        /// </summary>
        public List<string> SuggestedCategories { get; set; } = new List<string>();
    }

    /// <summary>
    /// 博客排序字段
    /// </summary>
    public enum BlogSortField
    {
        /// <summary>
        /// 发布日期
        /// </summary>
        PublishedDate,

        /// <summary>
        /// 最后修改时间
        /// </summary>
        LastModified,

        /// <summary>
        /// 标题
        /// </summary>
        Title,

        /// <summary>
        /// 阅读时间
        /// </summary>
        ReadingTime,

        /// <summary>
        /// 字数
        /// </summary>
        WordCount,

        /// <summary>
        /// 优先级
        /// </summary>
        Priority
    }

    /// <summary>
    /// 排序方向
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// 升序
        /// </summary>
        Ascending,

        /// <summary>
        /// 降序
        /// </summary>
        Descending
    }
}