# Markdown.Blog

## YAML Front Matter Rules

The blog post should start with YAML front matter. There are two valid formats:

### 1. Simple Format (Basic metadata only)

yaml
---
title: My Post Title
date: 2024-03-21
tags:
Tutorial
Guide
description: A brief description of the post
---
Content starts here...

### 2. Extended Format (With cover image)

yaml
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
```

