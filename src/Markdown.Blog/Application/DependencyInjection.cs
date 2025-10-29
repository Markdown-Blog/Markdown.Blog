using Microsoft.Extensions.DependencyInjection;
using Markdown.Blog.Application.Services;
using Markdown.Blog.Application.UseCases;

namespace Markdown.Blog.Application
{
    /// <summary>
    /// Application 层依赖注入配置
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// 注册 Application 层服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // 注册应用服务
            services.AddScoped<IBlogService, BlogService>();

            // 注册用例
            services.AddScoped<GetBlogContentUseCase>();
            services.AddScoped<SearchBlogsUseCase>();
            services.AddScoped<GetBlogStatisticsUseCase>();
            services.AddScoped<GetBlogListUseCase>();

            return services;
        }
    }
}