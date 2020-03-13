using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSE.SecureWebServerHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PlatformAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // The KeyVault can be specified as parameter or in ENV 'KeyVaultName'
                    // main Idea is to use Managed Identity to access the secrets in KeyVault, for non Azure Dev Environments you can specify
                    // the ENV 'AzureConnectionString' with your Application ID and Secret 'RunAs=App;AppId={AppId};TenantId={TenantId};AppKey={ClientSecret}'
                    webBuilder.ConfigurationFromKeyVault();
                    // The KeyVault and Certificate Name can be specified as parameters or in ENV 'KeyVaultName' and ENV 'CertificateName'
                    // Remark, if your RootCA differs from the SSL cert you should specify certificate name as a parameter
                    webBuilder.ConfigureKestrelSSLFromKeyVault();

                    webBuilder.UseStartup<Startup>();
                })
            ;
    }
}
