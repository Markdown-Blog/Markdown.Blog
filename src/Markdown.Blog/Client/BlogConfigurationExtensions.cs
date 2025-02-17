using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Markdown.Blog.Client
{
    public static class BlogConfigurationExtensions
    {
        public static IServiceCollection AddMarkdownBlogConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var blogConfig = configuration.GetSection("Blog")
                .Get<BlogConfiguration>()
                ?? throw new InvalidOperationException("Blog configuration is missing");

            services.AddSingleton(blogConfig);
            return services;
        }
    }

    public class BlogConfiguration
    {
        public List<Division> Divisions { get; set; }
    }
}