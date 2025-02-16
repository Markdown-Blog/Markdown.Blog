using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Markdown.Blog.Client.Configuration
{
    public static class BlogConfigurationExtensions
    {
        public static IServiceCollection AddMarkdownBlog(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var blogConfig = configuration.GetSection("Blog")
                .Get<BlogConfigurationOptions>()
                ?? throw new InvalidOperationException("Blog configuration is missing");

            services.AddSingleton(blogConfig);
            services.AddSingleton<IBlogService, BlogService>();
            services.AddSingleton<IBlogUpdateService, BlogUpdateService>();

            return services;
        }

        public static Division ToDivision(this DivisionOptions options)
        {
            return new Division
            {
                GithubUsername = options.GithubUsername,
                GithubRepository = options.GithubRepository,
                DivisionName = options.DivisionName
            };
        }
    }
}