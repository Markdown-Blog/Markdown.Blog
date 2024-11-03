# Markdown.Blog

## Directory Structure Rules

The blog content follows a hierarchical organization:

1. First-level directories under the root are **Divisions**
   - Each Division can be rendered independently or combined with others into a website
   - Each Division maintains its own index

2. The structure follows this pattern:
   ```
   Division/Category/SubCategory/Posts
   ```
   - Second level: Categories
   - Third level: SubCategories
   - Posts are placed under SubCategories

## YAML Front Matter Rules

The blog post should start with YAML front matter. There are two valid formats:

### 1. Simple Format (Basic metadata only)

```yaml
---
title: My Post Title
date: 2024-03-21
tags:
  - Tutorial
  - Guide
description: A brief description of the post
---

Content starts here...
```
### 2. Extended Format (With cover images)

```yaml
---
title: My Post Title
date: 2024-03-21
tags:
  - Tutorial
  - Guide
description: A brief description of the post
coverImages:    # Optional, will be overwritten if images exist in cover section
  - images/cover1.jpg
  - images/cover2.jpg
---
![cover image 1](images/my-cover1.jpg)
![cover image 2](images/my-cover2.jpg)
---

Content starts here...
```

### Cover Images Priority Rules
1. If images exist between the second and third `---` markers, they will be used as the cover images
2. Otherwise, if `coverImages` is specified in the YAML front matter, they will be used
3. If neither exists, no cover images will be assigned

### Examples

1. Minimal Post:
```yaml
---
title: Quick Start
date: 2024-03-21
---

Content here...
```

2. Standard Post with Tags:
```yaml
---
title: Advanced Features
date: 2024-03-21
description: Exploring advanced features and best practices
tags:
  - Advanced
  - Tutorial
---

Content here...
```

3. Post with Cover Image:
```yaml
---
title: Visual Guide
date: 2024-03-21
description: A visual guide to our platform
tags:
  - Guide
  - Visual
---
![cover](images/2024/visual-guide-cover.jpg)
---

Content here...
```

4. Post with All Available Fields:
```yaml
---
title: Complete Guide to Our Platform
date: 2024-03-21
description: A comprehensive guide covering all aspects
tags:
  - Guide
  - Tutorial
  - Advanced
coverImages:
  - images/2024/guide-cover1.jpg
  - images/2024/guide-cover2.jpg
pathSegments:
  - Documentation
  - Platform
  - Guides
  - Getting Started
---
![cover](images/2024/actual-guide-cover1.jpg)
![cover](images/2024/actual-guide-cover2.jpg)
---

Content here...
```

## Metadata Fields Usage

All metadata fields are extracted from the YAML front matter at the beginning of each markdown file. These fields are parsed into the `BlogMetadata` object for processing.

### Hierarchy
- Automatically generated from the physical file structure
- Not specified in YAML front matter
- Reflects the actual directory organization: `Division/Category/SubCategory`
- Directory placement determines the blog post's hierarchical position

### Relationship between PathSegments and SubCategory
- PathSegments typically belong to a specific SubCategory
- Each SubCategory can have multiple PathSegments hierarchies
- Example:
  ```
  Division/
    Category/
      Software-A/                     # SubCategory
        getting-started.md            # PathSegments: [Docs, Getting Started]
        advanced-usage.md             # PathSegments: [Docs, Advanced]
        api-overview.md               # PathSegments: [Docs, API, Overview]
        api-authentication.md         # PathSegments: [Docs, API, Authentication]
        api-endpoints.md              # PathSegments: [Docs, API, Endpoints]
        deployment-aws.md             # PathSegments: [Docs, Deployment, Cloud, AWS]
        deployment-azure.md           # PathSegments: [Docs, Deployment, Cloud, Azure]
      Software-B/                     # Another SubCategory
        setup.md                      # PathSegments: [Documentation, Setup]
        api.md                        # PathSegments: [Documentation, API]
        troubleshooting-basic.md      # PathSegments: [Documentation, Support, Troubleshooting, Basic]
        troubleshooting-advanced.md   # PathSegments: [Documentation, Support, Troubleshooting, Advanced]
        migration-guide-v1.md         # PathSegments: [Documentation, Migration, V1]
        migration-guide-v2.md         # PathSegments: [Documentation, Migration, V2]
- This organization allows for flexible documentation structures within each software's own space

### Tags
- Used for categorizing posts by topics
- Helps readers find related content
- Enables topic-based navigation and content discovery
- Supports statistical analysis of content distribution

### Path Segments
- Provides custom navigation path for posts
- Allows flexible organization independent of physical file structure
- Used for displaying post location in navigation menus
- Example:
  ```yaml
  pathSegments:
    - Documentation
    - Platform
    - Guides
    - Getting Started
  ```
  Will be displayed in navigation as: Documentation > Platform > Guides > Getting Started

## Index Features

The blog system provides several indexing methods through the `BlogIndex` class to organize and retrieve blog posts efficiently:

### Blog Metadata List Management
- Supports serialization and deserialization of blog metadata in both JSON and compressed binary formats
- Binary compression uses GZip for optimal storage size
- Example usage:
```csharp
var blogIndex = new BlogIndex();
string jsonData;
byte[] binaryData;

// Build index
blogIndex.BuildBlogMetadataList(metadataList, out jsonData, out binaryData);

// Restore from JSON
var restoredFromJson = blogIndex.RestoreBlogMetadataListFromJson(jsonData);

// Restore from binary
var restoredFromBinary = blogIndex.RestoreBlogMetadataListFromBinary(binaryData);
```

### Tag Organization
- Provides tag-based article counting and organization
- Returns tags sorted by article count in descending order
- Example usage:
```csharp
var tagCounts = blogIndex.OrganizeTagsAndArticleCounts(metadataList);
// Returns: Dictionary<string, int> where
// - Key: Tag name
// - Value: Number of articles using this tag
```

### Category and Subcategory Organization
- Groups articles by their category and subcategory hierarchy
- Provides article counts for each subcategory
- Example usage:
```csharp
var categories = blogIndex.GetCategoriesAndSubcategoriesWithBlogCount(metadataList);
// Returns: List of (Category, SubCategory, BlogCount) tuples
```

### Latest Blogs Retrieval
- Supports retrieving latest blogs by division or category
- Allows filtering by cover image status
- Provides pagination through start index parameter
- Example usage:
```csharp
var latestBlogs = blogIndex.GetLatestBlogs(
    blogMetadataList: metadataList,
    isPerCategory: true,        // true for category-based, false for division-based
    numberOfBlogs: 5,           // number of blogs to retrieve
    coverImageStatus: CoverImageStatus.WithCover,  // filter by cover image
    startIndex: 0               // pagination start index
);
```

Cover image filtering options:
- `CoverImageStatus.All`: Include all posts
- `CoverImageStatus.WithCover`: Only posts with cover images
- `CoverImageStatus.NoCover`: Only posts without cover images