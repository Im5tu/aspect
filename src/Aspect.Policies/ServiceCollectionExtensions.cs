using Aspect.Abstractions;
using Aspect.Policies.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aspect.Policies
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCompilerService(this IServiceCollection services)
        {
            services.TryAddSingleton<IParser, Parser>();
            services.TryAddSingleton<ILexer, Lexer>();
            services.TryAddSingleton<IPolicyCompiler, PolicyCompiler>();
            return services.TryAddCoreServices();
        }
    }
}
