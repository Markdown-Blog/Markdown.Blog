# Markdown.Blog

A convenient and flexible .NET-based reusable component for generating blogs using Markdown.

## Quick Start

### Installation

Add the Markdown.Blog package to your project:

```bash
dotnet add package Markdown.Blog
```

### Basic Usage

1. **Configure services in your startup:**

```csharp
using Markdown.Blog;

// In your Program.cs or Startup.cs
services.AddMarkdownBlog(
    basePath: "path/to/your/blog/content",  // Optional: defaults to current directory
    cacheDirectory: "path/to/cache"         // Optional: defaults to temp directory
);
```

2. **Use the blog service:**

```csharp
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBlogs()
    {
        var blogs = await _blogService.GetAllBlogsAsync();
        return Ok(blogs);
    }

    [HttpGet("{filePath}")]
    public async Task<IActionResult> GetBlog(string filePath)
    {
        var blog = await _blogService.GetBlogByFilePathAsync(filePath);
        if (blog == null)
            return NotFound();
        
        return Ok(blog);
    }
}
```

## Architecture

The component follows Clean Architecture principles with the following layers:

- **Domain**: Core business logic and models
- **Application**: Use cases and application services
- **Infrastructure**: Data access and external services
- **Shared**: Common models and constants

## Features

- ✅ Markdown file processing
- ✅ YAML front matter support
- ✅ Hierarchical blog organization
- ✅ Caching for performance
- ✅ Dependency injection ready
- ✅ .NET Standard 2.1 compatible

## Blog Structure

Your blog content should be organized as follows:

```
blog-content/
├── Division1/
│   ├── Category1/
│   │   ├── article1.md
│   │   └── article2.md
│   └── Category2/
│       └── article3.md
└── Division2/
    └── Category3/
        └── article4.md
```

Each markdown file should include YAML front matter:

```markdown
---
title: "My Blog Post"
description: "A brief description"
date: 2024-01-01T00:00:00Z
tags: ["tag1", "tag2"]
isDraft: false
---

# Your blog content here

This is the content of your blog post.
```

## License

MIT License - see LICENSE file for details.