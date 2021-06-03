using System;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Aspect.Dependencies
{
    internal sealed class MicrosoftDiTypeRegistrar : ITypeRegistrar
    {
        private readonly IServiceCollection _builder;

        public MicrosoftDiTypeRegistrar(IServiceCollection builder)
        {
            _builder = builder;
        }

        public void RegisterLazy(Type service, Func<object> factory)
        {
            _builder.AddSingleton(service, factory);
        }

        public ITypeResolver Build()
        {
            return new TypeResolver(_builder.BuildServiceProvider());
        }

        public void Register(Type service, Type implementation)
        {
            _builder.AddSingleton(service, implementation);
        }

        public void RegisterInstance(Type service, object implementation)
        {
            _builder.AddSingleton(service, implementation);
        }

        private sealed class TypeResolver : ITypeResolver
        {
            private readonly IServiceProvider _provider;

            public TypeResolver(IServiceProvider provider)
            {
                _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            }

            public object Resolve(Type? type)
            {
                if (type is null)
                    return null!;

                return _provider.GetRequiredService(type);
            }
        }
    }
}
