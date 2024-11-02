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
### 2. Extended Format (With cover image)

```yaml

---
title: My Post Title
date: 2024-03-21
tags:
  - Tutorial
  - Guide
description: A brief description of the post
coverImage: images/original-cover.jpg    # Optional, will be overwritten if image exists in cover section
---
![cover image](images/my-cover.jpg)
---

Content starts here...
```

### Cover Image Priority Rules
1. If an image exists between the second and third `---` markers, it will be used as the cover image
2. Otherwise, if `coverImage` is specified in the YAML front matter, it will be used
3. If neither exists, no cover image will be assigned

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
coverImage: images/2024/guide-cover.jpg
pathSegments:
  - Documentation
  - Platform
  - Guides
  - Getting Started
---
![cover](images/2024/actual-guide-cover.jpg)
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