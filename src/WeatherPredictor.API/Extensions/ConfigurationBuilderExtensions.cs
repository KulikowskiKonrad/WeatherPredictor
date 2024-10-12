using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace WeatherPredictor.API.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static void AddCustomAzureAppConfiguration(this IConfigurationBuilder configurationBuilder,
        IConfiguration config)
    {
        // configurationBuilder.AddAzureAppConfiguration(
        //     options => options.Connect(new Uri(config["AppConfig:Endpoint"]), new DefaultAzureCredential())
        //         .Select(KeyFilter.Any, LabelFilter.Null)
        //         .ConfigureKeyVault(kvOptions =>
        //             kvOptions.SetCredential(new DefaultAzureCredential())
        //                 .SetSecretRefreshInterval(TimeSpan.FromMinutes(30))
        //             ));
    }
}