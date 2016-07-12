﻿using Certes.AspNet;
using Certes.AspNet.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Certes
{
    public static class CertesOptionsBuilderExtensions
    {
        public static CertesOptionsBuilder AddInMemoryProviders(this CertesOptionsBuilder builder)
        {
            var contextStore = new InMemoryContextStore();
            var httpResponder = new InMemoryHttpChallengeResponder();

            builder.Services.AddSingleton<IContextStore>(contextStore);
            builder.Services.AddSingleton<IChallengeResponder>(httpResponder);
            builder.Services.AddSingleton<IHttpChallengeResponder>(httpResponder);
            return builder;
        }

        public static CertesOptionsBuilder UseConfiguration(this CertesOptionsBuilder builder, IConfiguration config)
        {
            var certesSection = config.GetSection("certes");
            builder.Services.Configure<CertesOptions>(certesSection);
            
            return builder;
        }

        public static CertesOptionsBuilder UseAzureConfiguration(this CertesOptionsBuilder builder, IConfiguration config)
        {
            var certesSection = config.GetSection("certes");
            var azureSection = certesSection?.GetSection("azure");
            if (azureSection != null)
            {
                var section = azureSection.GetSection("servicePrincipal");
                if (section != null)
                {
                    builder.Services.Configure<ServicePrincipalOptions>(section);
                    builder.Services.AddScoped<IClientCredentialProvider, ServicePrincipalCredentialProvider>();

                    section = azureSection.GetSection("webApp");
                    if (section != null)
                    {
                        builder.Services.Configure<WebAppOptions>(section);
                        builder.Services.AddScoped<ISslBindingManager, WebAppSslBindingManager>();
                    }
                }
            }

            return builder;
        }
    }
}