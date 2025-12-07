using Microsoft.Extensions.DependencyInjection;
using Markdown.Blog.Client;
//using Markdown.Blog.Server;

namespace Markdown.Blog.Domain
{
    /// <summary>
    /// Domain 层依赖注入配置
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// 注册 Domain 层服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            // 注册客户端服务
            services.AddScoped<BlogContentRenderer>();
            
            // 注意：BlogIndexBuilder 和 BlogImageExtractor 是静态类，不需要注册
            // BlogIndexLoader 也是静态类，不需要注册

            return services;
        }
    }
}