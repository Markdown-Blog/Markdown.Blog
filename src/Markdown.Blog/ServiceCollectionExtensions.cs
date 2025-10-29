using Microsoft.Extensions.DependencyInjection;
using Markdown.Blog.Application;
using Markdown.Blog.Domain;
using Markdown.Blog.Infrastructure;

namespace Markdown.Blog
{
    /// <summary>
    /// Extension methods for configuring Markdown.Blog services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all Markdown.Blog services to the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="basePath">Base path for blog storage (optional, defaults to current directory)</param>
        /// <param name="cacheDirectory">Cache directory path (optional, defaults to temp directory)</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMarkdownBlog(
            this IServiceCollection services, 
            string basePath = null, 
            string cacheDirectory = null)
        {
            // Add all layers
            services.AddDomainServices();
            services.AddInfrastructureServices(basePath, cacheDirectory);
            services.AddApplicationServices();

            return services;
        }
    }
}