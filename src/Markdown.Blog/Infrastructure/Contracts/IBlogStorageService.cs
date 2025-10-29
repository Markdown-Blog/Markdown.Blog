using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Markdown.Blog.Shared.Models;

namespace Markdown.Blog.Infrastructure.Contracts
{
	/// <summary>
	/// 博客存储服务接口，基于 GitHub 技术栈的实际能力设计
	/// </summary>
	public interface IBlogStorageService
	{
		// ========== Blog 文件操作 ==========

		/// <summary>
		/// 根据文件路径获取博客文章内容
		/// </summary>
		/// <param name="filePath">文件路径 (如: "Div1/Cat1/article.md")</param>
		/// <returns>博客文章的 Markdown 内容</returns>
		Task<string> GetBlogContentAsync(string filePath);

		/// <summary>
		/// 根据文件路径获取博客文章元数据
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns>博客文章元数据</returns>
		Task<BlogMetadata> GetBlogMetadataAsync(string filePath);

		/// <summary>
		/// 写入博客文章
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <param name="content">Markdown 内容</param>
		/// <returns>写入结果</returns>
		Task<bool> WriteBlogAsync(string filePath, string content);

		/// <summary>
		/// 删除博客文章
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns>删除结果</returns>
		Task<bool> DeleteBlogAsync(string filePath);

		/// <summary>
		/// 检查博客文章是否存在
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns>是否存在</returns>
		Task<bool> BlogExistsAsync(string filePath);

		// ========== Division 目录操作 ==========

		/// <summary>
		/// 获取所有分区名称
		/// </summary>
		/// <returns>分区名称列表</returns>
		Task<IEnumerable<string>> GetDivisionsAsync();

		/// <summary>
		/// 获取分区下的所有文件路径
		/// </summary>
		/// <param name="divisionName">分区名称</param>
		/// <returns>文件路径列表</returns>
		Task<IEnumerable<string>> GetDivisionFilesAsync(string divisionName);

		/// <summary>
		/// 检查分区是否存在
		/// </summary>
		/// <param name="divisionName">分区名称</param>
		/// <returns>是否存在</returns>
		Task<bool> DivisionExistsAsync(string divisionName);

		// ========== Index 索引操作 ==========

		/// <summary>
		/// 获取分区的索引文件内容
		/// </summary>
		/// <param name="divisionName">分区名称</param>
		/// <returns>索引 JSON 内容</returns>
		Task<string> GetDivisionIndexAsync(string divisionName);

		/// <summary>
		/// 写入分区的索引文件
		/// </summary>
		/// <param name="divisionName">分区名称</param>
		/// <param name="indexContent">索引 JSON 内容</param>
		/// <returns>写入结果</returns>
		Task<bool> WriteDivisionIndexAsync(string divisionName, string indexContent);

		/// <summary>
		/// 检查分区索引是否存在
		/// </summary>
		/// <param name="divisionName">分区名称</param>
		/// <returns>是否存在</returns>
		Task<bool> DivisionIndexExistsAsync(string divisionName);

		// ========== 原始文件操作 ==========

		/// <summary>
		/// 读取任意文件内容
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns>文件内容</returns>
		Task<string> ReadFileAsync(string filePath);

		/// <summary>
		/// 写入任意文件内容
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <param name="content">文件内容</param>
		/// <returns>写入结果</returns>
		Task<bool> WriteFileAsync(string filePath, string content);

		/// <summary>
		/// 删除任意文件
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns>删除结果</returns>
		Task<bool> DeleteFileAsync(string filePath);

		/// <summary>
		/// 检查任意文件是否存在
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns>是否存在</returns>
		Task<bool> FileExistsAsync(string filePath);
	}
}