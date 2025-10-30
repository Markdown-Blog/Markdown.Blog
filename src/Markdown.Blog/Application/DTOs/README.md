# DTOs (Data Transfer Objects) 设计文档

## 概述

本目录包含了 Markdown.Blog 项目的数据传输对象 (DTOs)，这些 DTOs 是为未来可能的 API 层和服务端应用而预留的架构设计。

## 当前状态

目前这些 DTOs 主要用于：
- 应用层内部的数据传输
- 为未来的 Web API 做架构准备
- 提供标准化的数据格式定义

## DTOs 列表及用途

### 1. BlogContentDto
- **用途**: 完整的博客内容传输，包含元数据和内容
- **属性**: 
  - `Metadata`: 博客元数据
  - `MarkdownContent`: 原始 Markdown 内容
  - `HtmlContent`: 预渲染的 HTML 内容（待移除）
- **未来应用**: API 端点返回完整博客内容

### 2. BlogMetadataDto
- **用途**: 博客元数据传输，用于列表展示
- **属性**: 标题、作者、日期、标签、分类等
- **未来应用**: 博客列表 API、搜索结果

### 3. BlogSearchDto
- **用途**: 搜索请求和响应的数据结构
- **包含**: `BlogSearchRequestDto` 和 `BlogSearchResponseDto`
- **未来应用**: 搜索 API 端点

### 4. BlogStatisticsDto
- **用途**: 博客统计信息传输
- **包含**: 总数、分类统计、标签统计等
- **未来应用**: 仪表板 API、统计分析

## 架构设计理念

### 分层架构
```
┌─────────────────┐
│   API Layer    │ ← DTOs 的主要使用场景
├─────────────────┤
│ Application     │ ← DTOs 定义位置
├─────────────────┤
│   Domain        │ ← 领域模型 (BlogMetadata, BlogData)
├─────────────────┤
│ Infrastructure  │ ← 数据访问
└─────────────────┘
```

### 数据流向
1. **当前**: Domain Models → DTOs → Blazor Client
2. **未来**: Domain Models → DTOs → API → External Clients

## 未来开发方案

### 方案一：静态 API 文件 (推荐)
**基础设施**: GitHub Actions + Cloudflare Pages
**实现方式**:
```
GitHub Actions 构建时:
1. 读取 Markdown 文件
2. 生成 JSON API 文件
3. 部署到 Cloudflare Pages
```

**优势**:
- 零运行成本
- 高性能 (CDN 缓存)
- 简单维护
- 符合当前静态架构

**API 端点示例**:
```
/api/blogs/index.json          - 博客列表
/api/blogs/{id}.json          - 单个博客内容
/api/search/{query}.json      - 搜索结果
/api/statistics.json          - 统计信息
```

### 方案二：Cloudflare Workers
**基础设施**: Cloudflare Workers + KV Storage
**实现方式**:
```typescript
// Worker 处理 API 请求
export default {
  async fetch(request: Request): Promise<Response> {
    const url = new URL(request.url);
    
    if (url.pathname.startsWith('/api/blogs')) {
      return handleBlogRequest(request);
    }
    
    return new Response('Not Found', { status: 404 });
  }
}
```

**优势**:
- 动态处理能力
- 边缘计算性能
- 支持复杂查询
- 可扩展性强

**成本**: 免费额度内基本够用

#### C# DTOs 复用策略

**问题**: Cloudflare Workers 使用 TypeScript/JavaScript，无法直接使用 C# DTOs

**解决方案**:

##### 1. 代码生成方案 (推荐)
使用工具自动从 C# DTOs 生成 TypeScript 接口：

```csharp
// 在构建时运行的代码生成器
public class TypeScriptGenerator
{
    public void GenerateFromDTOs()
    {
        var dtoTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.Name.EndsWith("Dto"));
            
        foreach (var type in dtoTypes)
        {
            GenerateTypeScriptInterface(type);
        }
    }
    
    private void GenerateTypeScriptInterface(Type type)
    {
        var tsInterface = $@"
export interface {type.Name} {{
{string.Join("\n", type.GetProperties().Select(p => 
    $"  {ToCamelCase(p.Name)}: {MapCSharpTypeToTypeScript(p.PropertyType)};"))}
}}";
        
        File.WriteAllText($"workers/types/{type.Name}.ts", tsInterface);
    }
}
```

生成的 TypeScript 接口：
```typescript
// BlogContentDto.ts
export interface BlogContentDto {
  metadata: BlogMetadataDto;
  markdownContent: string;
  // htmlContent 已移除
}

// BlogMetadataDto.ts
export interface BlogMetadataDto {
  title: string;
  author: string;
  publishDate: string;
  tags: string[];
  category: string;
}
```

##### 2. JSON Schema 方案
使用 JSON Schema 作为中间格式：

```csharp
// 生成 JSON Schema
public class JsonSchemaGenerator
{
    public void GenerateSchemas()
    {
        var schema = JsonSchema.FromType<BlogContentDto>();
        File.WriteAllText("schemas/BlogContentDto.json", schema.ToJson());
    }
}
```

然后在 Workers 中使用：
```typescript
// 从 JSON Schema 生成 TypeScript 类型
import { BlogContentDto } from './generated/types';

// 运行时验证
import Ajv from 'ajv';
const ajv = new Ajv();
const validate = ajv.compile(blogContentSchema);

function validateBlogContent(data: unknown): data is BlogContentDto {
  return validate(data);
}
```

##### 3. 共享数据契约方案
创建独立的数据契约项目：

```
Markdown.Blog.Contracts/
├── DTOs/
│   ├── BlogContentDto.cs
│   └── BlogMetadataDto.cs
├── TypeScript/
│   ├── BlogContentDto.ts
│   └── BlogMetadataDto.ts
└── Schemas/
    ├── BlogContentDto.json
    └── BlogMetadataDto.json
```

##### 4. 构建流程集成
在 GitHub Actions 中自动同步：

```yaml
name: Sync DTOs to Workers
on:
  push:
    paths:
      - 'src/Markdown.Blog/Application/DTOs/**'

jobs:
  sync-dtos:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Generate TypeScript Types
        run: |
          dotnet run --project Tools/TypeScriptGenerator
          
      - name: Deploy to Workers
        run: |
          cd workers
          npm install
          npx wrangler deploy
```

### 方案三：混合架构 (最佳实践)
**阶段一**: 实现静态 API 文件
- 立即可用
- 零成本
- 满足基本需求

**阶段二**: 扩展 Cloudflare Workers
- 添加动态功能
- 高级搜索
- 实时统计

## 实现路径

### 第一步：完善 DTOs
1. 移除 `BlogContentDto.HtmlContent` 属性
2. 添加 API 版本控制支持
3. 完善数据验证
4. 添加 TypeScript 生成支持

### 第二步：创建跨平台工具
```csharp
// Tools/TypeScriptGenerator/Program.cs
public class Program
{
    public static void Main(string[] args)
    {
        var generator = new TypeScriptGenerator();
        generator.GenerateFromAssembly(typeof(BlogContentDto).Assembly);
        
        var schemaGenerator = new JsonSchemaGenerator();
        schemaGenerator.GenerateSchemas();
        
        Console.WriteLine("TypeScript 接口和 JSON Schema 生成完成");
    }
}

// 支持的类型映射
private static readonly Dictionary<Type, string> TypeMap = new()
{
    { typeof(string), "string" },
    { typeof(int), "number" },
    { typeof(bool), "boolean" },
    { typeof(DateTime), "string" }, // ISO 8601
    { typeof(string[]), "string[]" },
    { typeof(List<string>), "string[]" }
};
```

### 第三步：创建 API 生成器
```csharp
public class ApiGenerator
{
    public async Task GenerateStaticApiFiles()
    {
        // 生成博客列表 API
        var blogs = await _blogService.GetAllBlogsAsync();
        var blogDtos = blogs.Select(MapToDto);
        await WriteJsonFile("api/blogs/index.json", blogDtos);
        
        // 生成单个博客 API
        foreach (var blog in blogs)
        {
            var contentDto = await _blogService.GetBlogContentAsync(blog.FilePath);
            await WriteJsonFile($"api/blogs/{blog.Id}.json", contentDto);
        }
        
        // 生成 TypeScript 类型定义
        await GenerateTypeScriptTypes();
    }
    
    private async Task GenerateTypeScriptTypes()
    {
        var generator = new TypeScriptGenerator();
        await generator.GenerateAllTypes("wwwroot/types/");
    }
}
```

### 第四步：集成到构建流程
在 GitHub Actions 中添加 API 生成步骤：
```yaml
- name: Generate API Files and Types
  run: |
    dotnet run --project Tools/TypeScriptGenerator
    dotnet run --project Tools/ApiGenerator
    cp -r api-output/* publish/api/
    cp -r types-output/* publish/types/
```

## DTOs 跨平台复用最佳实践

### 1. 保持数据契约一致性
```csharp
// 使用属性标注来控制序列化
public class BlogContentDto
{
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; } = "1.0";
    
    [JsonPropertyName("metadata")]
    public BlogMetadataDto Metadata { get; set; } = new();
    
    [JsonPropertyName("markdownContent")]
    public string MarkdownContent { get; set; } = string.Empty;
    
    // HtmlContent 已移除 - 符合实时渲染架构
}
```

### 2. TypeScript 类型安全
```typescript
// 在 Cloudflare Workers 中使用生成的类型
import { BlogContentDto, BlogMetadataDto } from './types/generated';

export default {
  async fetch(request: Request): Promise<Response> {
    const blogContent: BlogContentDto = {
      apiVersion: "1.0",
      metadata: {
        title: "示例博客",
        author: "作者",
        publishDate: new Date().toISOString(),
        tags: ["技术", "博客"],
        category: "技术"
      },
      markdownContent: "# 标题\n\n内容..."
    };
    
    return new Response(JSON.stringify(blogContent), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
};
```

### 3. 运行时验证
```typescript
// 使用 Zod 进行运行时类型验证
import { z } from 'zod';

const BlogMetadataSchema = z.object({
  title: z.string(),
  author: z.string(),
  publishDate: z.string(),
  tags: z.array(z.string()),
  category: z.string()
});

const BlogContentSchema = z.object({
  apiVersion: z.string(),
  metadata: BlogMetadataSchema,
  markdownContent: z.string()
});

// 在 Worker 中验证数据
function validateBlogContent(data: unknown): BlogContentDto {
  return BlogContentSchema.parse(data);
}
```

### 4. 版本兼容性策略
```csharp
// C# 端支持多版本
public class BlogContentDtoV1
{
    public string ApiVersion { get; set; } = "1.0";
    public BlogMetadataDto Metadata { get; set; } = new();
    public string MarkdownContent { get; set; } = string.Empty;
}

public class BlogContentDtoV2 : BlogContentDtoV1
{
    public string ApiVersion { get; set; } = "2.0";
    public DateTime LastModified { get; set; }
    public string[] RelatedPosts { get; set; } = Array.Empty<string>();
}
```

```typescript
// TypeScript 端处理版本差异
type BlogContentDto = BlogContentDtoV1 | BlogContentDtoV2;

function handleBlogContent(dto: BlogContentDto) {
  switch (dto.apiVersion) {
    case "1.0":
      return handleV1(dto as BlogContentDtoV1);
    case "2.0":
      return handleV2(dto as BlogContentDtoV2);
    default:
      throw new Error(`Unsupported API version: ${dto.apiVersion}`);
  }
}
```

## 推荐的项目结构

```
Markdown.Blog/
├── src/
│   ├── Markdown.Blog/
│   │   └── Application/
│   │       └── DTOs/           # C# DTOs 定义
│   ├── Markdown.Blog.Blazor/   # Blazor 客户端
│   └── Tools/
│       ├── TypeScriptGenerator/ # DTO → TypeScript 转换工具
│       └── ApiGenerator/       # 静态 API 生成工具
├── workers/                    # Cloudflare Workers
│   ├── src/
│   │   ├── types/             # 生成的 TypeScript 类型
│   │   └── handlers/          # API 处理逻辑
│   └── schemas/               # JSON Schema 文件
└── publish/
    ├── api/                   # 生成的静态 API 文件
    └── types/                 # 公开的 TypeScript 类型定义
```

### API 版本化
```csharp
public class BlogContentDto
{
    public string ApiVersion { get; set; } = "1.0";
    // ... 其他属性
}
```

### 向后兼容
- 新增属性使用可选字段
- 废弃属性保留但标记为过时
- 主版本号变更时提供迁移指南

## 性能考虑

### 缓存策略
- 静态文件: CDN 缓存 (长期)
- 动态 API: 边缘缓存 (短期)
- 客户端: 浏览器缓存

### 数据优化
- 分页支持
- 字段选择 (GraphQL 风格)
- 压缩传输

## 安全考虑

### 访问控制
- 公开内容: 无限制访问
- 管理功能: 需要认证
- 敏感操作: 需要授权

### 数据保护
- 输入验证
- 输出编码
- 防止信息泄露

## 监控和分析

### 使用统计
- API 调用次数
- 热门内容
- 用户行为分析

### 性能监控
- 响应时间
- 错误率
- 缓存命中率

## 总结

DTOs 为 Markdown.Blog 项目提供了面向未来的架构基础，支持从当前的静态 Blazor 应用平滑过渡到功能完整的 API 驱动架构。通过分阶段实施，可以在保持当前简洁性的同时，为未来的扩展需求做好准备。

推荐采用混合架构方案，先实现静态 API 文件，再根据需要扩展动态功能，这样既能满足当前需求，又为未来发展留下了充足的空间。