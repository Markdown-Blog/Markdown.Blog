using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Markdown.Blog.Domain.Models;

namespace Markdown.Blog.Domain.Validation
{
	/// <summary>
	/// 验证 <see cref="BlogMetadata"/> 的领域规则（纯规则、无基础设施依赖）。
	/// 设计为易复用，可在发布流水线、应用层或任何工具中直接调用。
	/// </summary>
	public static class BlogMetadataValidator
	{
		/// <summary>
		/// 默认验证选项。
		/// </summary>
		public static readonly BlogMetadataValidationOptions DefaultOptions = new BlogMetadataValidationOptions();

		/// <summary>
		/// 对单个 <see cref="BlogMetadata"/> 执行验证。
		/// </summary>
		/// <param name="metadata">要验证的元数据。</param>
		/// <param name="options">验证选项（可选）。</param>
		/// <returns>验证结果。</returns>
		public static BlogValidationResult Validate(BlogMetadata metadata, BlogMetadataValidationOptions? options = null)
		{
			options ??= DefaultOptions;
			var messages = new List<BlogValidationMessage>();

			if (metadata == null)
			{
				messages.Add(BlogValidationMessage.Error("NullMetadata", "BlogMetadata 不能为空"));
				return BlogValidationResult.From(messages);
			}

			// FilePath
			if (string.IsNullOrWhiteSpace(metadata.FilePath))
				messages.Add(BlogValidationMessage.Error("FilePath.Required", "文件路径 FilePath 不能为空"));
			else
			{
				if (!options.AllowNonMarkdownFile && !(metadata.FilePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase) || metadata.FilePath.EndsWith(".mdx", StringComparison.OrdinalIgnoreCase)))
					messages.Add(BlogValidationMessage.Warning("FilePath.Extension", "推荐使用 .md/.mdx 作为博客文件扩展名"));

				if (metadata.FilePath.IndexOfAny(new[] { '*', '"', '<', '>', '|' }) >= 0)
					messages.Add(BlogValidationMessage.Error("FilePath.IllegalChars", "FilePath 包含非法字符 * \" < > |"));
			}

			// Title
			if (string.IsNullOrWhiteSpace(metadata.Title))
				messages.Add(BlogValidationMessage.Error("Title.Required", "标题 Title 不能为空"));
			else
			{
				if (metadata.Title.Length > options.MaxTitleLength)
					messages.Add(BlogValidationMessage.Error("Title.Length", $"标题长度不可超过 {options.MaxTitleLength} 字符"));

				if (!Regex.IsMatch(metadata.Title, options.AllowedTextPattern))
					messages.Add(BlogValidationMessage.Warning("Title.Pattern", "标题包含不推荐的字符（请使用字母、数字、空格、连字符或下划线）"));
			}

			// Description
			if (string.IsNullOrWhiteSpace(metadata.Description))
			{
				if (options.RequireDescription)
					messages.Add(BlogValidationMessage.Error("Description.Required", "描述 Description 为必填"));
				else
					messages.Add(BlogValidationMessage.Warning("Description.Missing", "描述未填写，可能影响 SEO 与列表展示"));
			}
			else if (metadata.Description.Length > options.MaxDescriptionLength)
			{
				messages.Add(BlogValidationMessage.Warning("Description.Length", $"描述长度建议不超过 {options.MaxDescriptionLength} 字符"));
			}

			// Date
			if (metadata.Date == default)
			{
				messages.Add(BlogValidationMessage.Error("Date.Required", "发布日期 Date 未设置"));
			}
			else
			{
				var now = DateTime.UtcNow;
				var dateUtc = metadata.Date.Kind == DateTimeKind.Unspecified
					? DateTime.SpecifyKind(metadata.Date, DateTimeKind.Utc)
					: metadata.Date.ToUniversalTime();

				if (dateUtc - now > options.AllowedFutureTimeSpan)
					messages.Add(BlogValidationMessage.Error("Date.Future", "发布日期不可晚于当前时间（除非允许未来时间）"));

				if (now - dateUtc > options.MaxAgeTimeSpan)
					messages.Add(BlogValidationMessage.Warning("Date.Age", "发布日期距离当前时间过久，确认是否为预期"));
			}

			// Tags
			var tags = metadata.Tags ?? new List<string>();
			if (tags.Count > options.MaxTagCount)
				messages.Add(BlogValidationMessage.Warning("Tags.Count", $"标签数量建议不超过 {options.MaxTagCount} 个"));

			var normalizedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (var tag in tags)
			{
				var t = (tag ?? string.Empty).Trim();
				if (string.IsNullOrWhiteSpace(t))
				{
					messages.Add(BlogValidationMessage.Error("Tag.Empty", "存在为空的标签项"));
					continue;
				}

				if (t.Length > options.MaxTagLength)
					messages.Add(BlogValidationMessage.Warning("Tag.Length", $"标签 \"{t}\" 长度建议不超过 {options.MaxTagLength}"));

				if (!Regex.IsMatch(t, options.AllowedTagPattern))
					messages.Add(BlogValidationMessage.Warning("Tag.Pattern", $"标签 \"{t}\" 含不推荐字符（仅允许字母、数字、空格、连字符）"));

				if (!normalizedTags.Add(t))
					messages.Add(BlogValidationMessage.Warning("Tag.Duplicate", $"标签 \"{t}\" 重复"));
			}

			// CoverImages
			var images = metadata.CoverImages ?? new List<string>();
			foreach (var img in images)
			{
				var path = (img ?? string.Empty).Trim();
				if (string.IsNullOrWhiteSpace(path))
				{
					messages.Add(BlogValidationMessage.Warning("Cover.Empty", "存在为空的封面图片路径"));
					continue;
				}

				if (!Regex.IsMatch(path, options.AllowedImagePathPattern))
					messages.Add(BlogValidationMessage.Warning("Cover.Path", $"封面路径 \"{path}\" 格式不规范（仅允许相对路径、字母数字、连字符、点和斜杠）"));

				var hasKnownExt = options.AllowedImageExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
				if (!hasKnownExt)
					messages.Add(BlogValidationMessage.Warning("Cover.Extension", $"封面路径 \"{path}\" 未使用常见图片扩展名：{string.Join(", ", options.AllowedImageExtensions)}"));
			}

			// Hierarchy
			if (metadata.Hierarchy == null)
			{
				messages.Add(BlogValidationMessage.Error("Hierarchy.Required", "层级信息 Hierarchy 不能为空"));
			}
			else
			{
				ValidateHierarchy(metadata.Hierarchy, messages, options);
			}

			// PathSegments（仅在启用路径模式时校验）
			if (metadata.PathSegments != null)
			{
				var segments = metadata.PathSegments;
				if (segments.Count == 0)
					messages.Add(BlogValidationMessage.Error("PathSegments.Empty", "PathSegments 至少需要一个段"));

				foreach (var seg in segments)
				{
					var s = (seg ?? string.Empty).Trim();
					if (string.IsNullOrWhiteSpace(s))
						messages.Add(BlogValidationMessage.Error("PathSegments.Blank", "PathSegments 包含空白段"));

					if (!Regex.IsMatch(s, options.AllowedPathSegmentPattern))
						messages.Add(BlogValidationMessage.Warning("PathSegments.Pattern", $"PathSegments 段 \"{s}\" 包含不推荐字符（仅允许字母、数字、连字符和空格）"));

					if (s.Contains('/') || s.Contains('\\'))
						messages.Add(BlogValidationMessage.Error("PathSegments.Separator", "PathSegments 段不可包含路径分隔符 / 或 \\"));
				}
			}

			return BlogValidationResult.From(messages);
		}

		/// <summary>
		/// 对层级信息执行验证。
		/// </summary>
		private static void ValidateHierarchy(BlogHierarchy hierarchy, List<BlogValidationMessage> messages, BlogMetadataValidationOptions options)
		{
			// Division
			if (string.IsNullOrWhiteSpace(hierarchy.Division))
				messages.Add(BlogValidationMessage.Error("Hierarchy.Division.Required", "Hierarchy.Division 不能为空"));
			else
			{
				if (!Regex.IsMatch(hierarchy.Division, options.AllowedTextPattern))
					messages.Add(BlogValidationMessage.Warning("Hierarchy.Division.Pattern", "Division 包含不推荐字符"));

				if (options.AllowedDivisions.Count > 0 && !options.AllowedDivisions.Contains(hierarchy.Division))
					messages.Add(BlogValidationMessage.Warning("Hierarchy.Division.NotInAllowed", $"Division \"{hierarchy.Division}\" 不在允许集合中"));
			}

			// Category
			if (string.IsNullOrWhiteSpace(hierarchy.Category))
				messages.Add(BlogValidationMessage.Error("Hierarchy.Category.Required", "Hierarchy.Category 不能为空"));
			else
			{
				if (!Regex.IsMatch(hierarchy.Category, options.AllowedTextPattern))
					messages.Add(BlogValidationMessage.Warning("Hierarchy.Category.Pattern", "Category 包含不推荐字符"));

				if (options.AllowedCategories.Count > 0 && !options.AllowedCategories.Contains(hierarchy.Category))
					messages.Add(BlogValidationMessage.Warning("Hierarchy.Category.NotInAllowed", $"Category \"{hierarchy.Category}\" 不在允许集合中"));
			}

			// SubCategory（可选）
			if (!string.IsNullOrWhiteSpace(hierarchy.SubCategory))
			{
				if (!Regex.IsMatch(hierarchy.SubCategory, options.AllowedTextPattern))
					messages.Add(BlogValidationMessage.Warning("Hierarchy.SubCategory.Pattern", "SubCategory 包含不推荐字符"));
			}
		}

		/// <summary>
		/// 验证选项，用于调整领域规则的严格程度和允许范围。
		/// </summary>
		public class BlogMetadataValidationOptions
		{
			/// <summary>
			/// 标题最大长度（默认 200）。
			/// </summary>
			public int MaxTitleLength { get; set; } = 200;

			/// <summary>
			/// 描述最大长度（默认 500）。
			/// </summary>
			public int MaxDescriptionLength { get; set; } = 500;

			/// <summary>
			/// 标签最大数量（默认 10）。
			/// </summary>
			public int MaxTagCount { get; set; } = 10;

			/// <summary>
			/// 单个标签最大长度（默认 50）。
			/// </summary>
			public int MaxTagLength { get; set; } = 50;

			/// <summary>
			/// 是否必须填写描述（默认 false）。
			/// </summary>
			public bool RequireDescription { get; set; } = false;

			/// <summary>
			/// 允许发布日期相对当前时间的最大未来跨度（默认不允许未来时间）。
			/// </summary>
			public TimeSpan AllowedFutureTimeSpan { get; set; } = TimeSpan.Zero;

			/// <summary>
			/// 认为“过久”的时间跨度，用于产生警告（默认 5 年）。
			/// </summary>
			public TimeSpan MaxAgeTimeSpan { get; set; } = TimeSpan.FromDays(365 * 5);

			/// <summary>
			/// 允许的图片扩展名（用于封面图片路径校验）。
			/// </summary>
			public IReadOnlyList<string> AllowedImageExtensions { get; set; } = new[] { ".png", ".jpg", ".jpeg", ".webp", ".gif" };

			/// <summary>
			/// 允许的通用文本模式（字母、数字、空格、连字符、下划线）。
			/// </summary>
			public string AllowedTextPattern { get; set; } = @"^[A-Za-z0-9\-_\s]+$";

			/// <summary>
			/// 允许的标签模式（字母、数字、空格、连字符）。
			/// </summary>
			public string AllowedTagPattern { get; set; } = @"^[A-Za-z0-9\-\s]+$";

			/// <summary>
			/// 允许的 PathSegments 模式（字母、数字、空格、连字符）。
			/// </summary>
			public string AllowedPathSegmentPattern { get; set; } = @"^[A-Za-z0-9\-\s]+$";

			/// <summary>
			/// 封面图片路径允许的模式（建议相对路径）。
			/// </summary>
			public string AllowedImagePathPattern { get; set; } = @"^[A-Za-z0-9_\-./]+$";

			/// <summary>
			/// 当为 true 时，如果 FilePath 扩展名不是 .md/.mdx，将给出警告（默认 true）。
			/// </summary>
			public bool AllowNonMarkdownFile { get; set; } = true;

			/// <summary>
        /// 允许的 Division 集合（为空表示不限制）。
        /// </summary>
        public HashSet<string> AllowedDivisions { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 允许的 Category 集合（为空表示不限制）。
        /// </summary>
        public HashSet<string> AllowedCategories { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// 验证结果。
		/// </summary>
		public class BlogValidationResult
		{
			public IReadOnlyList<BlogValidationMessage> Messages { get; }

			public bool IsValid => Messages.All(m => m.Severity != ValidationSeverity.Error);

			public IEnumerable<BlogValidationMessage> Errors => Messages.Where(m => m.Severity == ValidationSeverity.Error);
			public IEnumerable<BlogValidationMessage> Warnings => Messages.Where(m => m.Severity == ValidationSeverity.Warning);

			private BlogValidationResult(IReadOnlyList<BlogValidationMessage> messages)
			{
				Messages = messages;
			}

			public static BlogValidationResult From(IEnumerable<BlogValidationMessage> messages)
				=> new BlogValidationResult(messages.ToList());
		}

		/// <summary>
		/// 验证消息。
		/// </summary>
		public class BlogValidationMessage
		{
			public string Code { get; }
			public string Message { get; }
			public ValidationSeverity Severity { get; }

			public BlogValidationMessage(string code, string message, ValidationSeverity severity)
			{
				Code = code;
				Message = message;
				Severity = severity;
			}

			public static BlogValidationMessage Error(string code, string message) => new BlogValidationMessage(code, message, ValidationSeverity.Error);
			public static BlogValidationMessage Warning(string code, string message) => new BlogValidationMessage(code, message, ValidationSeverity.Warning);
		}

		/// <summary>
		/// 验证消息严重性。
		/// </summary>
		public enum ValidationSeverity
		{
			Error,
			Warning
		}
	}
}