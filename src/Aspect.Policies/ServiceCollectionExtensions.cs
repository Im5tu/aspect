using Aspect.Abstractions;
using Aspect.Policies.BuiltIn;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.Suite;
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
            services.TryAddSingleton<IPolicySuiteValidator, PolicySuiteValidator>();
            services.TryAddSingleton<IBuiltInPolicyProvider, BuiltInPolicyProvider>();
            services.TryAddSingleton<IPolicySuiteSerializer, PolicySuiteSerializer>();
            return services.TryAddCoreServices();
        }
    }
}
