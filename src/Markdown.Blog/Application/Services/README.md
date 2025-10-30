# Services 层简要说明

- 角色定位：应用层服务，负责组合领域数据并以 DTOs 形式对外提供。
- 当前用途：为 Blazor 客户端与应用层内部调用，返回标准化 DTOs（如 `BlogContentDto`、`BlogMetadataDto`、`BlogSearchResponseDto`、`BlogStatisticsDto`）。
- 非职责：不进行 Markdown→HTML 渲染；不承载复杂业务规则；不直接绑定 UI。
- 未来用途：作为 Web API 的应用服务被 API 控制器复用；也可被静态 API 生成工具调用。
- 设计原则：轻量编排、无状态、可测试；仅返回 DTOs；保持与 Domain/Infrastructure 的明确边界。
- 注意事项：遵循“实时渲染”设计，服务返回 Markdown 原文与元数据；`BlogContentDto.HtmlContent` 计划移除。
- 关联文件：`IBlogService.cs` 定义契约；`BlogService.cs` 提供实现。
- 参考文档：参见同级 `../DTOs/README.md` 了解 DTOs 的作用与跨平台复用方案。