using Microsoft.Extensions.DependencyInjection;
using Markdown.Blog.Infrastructure.Contracts;
using Markdown.Blog.Infrastructure.Services;
using Markdown.Blog.Domain.Services;

namespace Markdown.Blog.Infrastructure
{
    /// <summary>
    /// Infrastructure layer dependency injection configuration
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Add infrastructure services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="basePath">Base path for blog storage</param>
        /// <param name="cacheDirectory">Cache directory path</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            string basePath = null, 
            string cacheDirectory = null)
        {
            // Register storage service with base path
            services.AddSingleton<IBlogStorageService>(provider => 
                new BlogStorageService(basePath ?? System.IO.Directory.GetCurrentDirectory()));

            // Register cache service with cache directory
            services.AddSingleton<IBlogCacheService>(provider => 
                new BlogCacheService(cacheDirectory));

            // Register image path service
            services.AddScoped<IBlogImagePathService, BlogImagePathService>();

            return services;
        }
    }
}