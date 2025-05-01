using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace particlex.Core.Configuration;

public class ConfigurationLoader(string configurationPath = "Content/configuration.json")
{
    public async Task<Model.Configuration> LoadConfigurationAsync()
    {
        Console.WriteLine(Directory.GetCurrentDirectory());
        if (!File.Exists(configurationPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {configurationPath}");
        }

        var jsonContent = await File.ReadAllTextAsync(configurationPath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var configuration = JsonSerializer.Deserialize<Model.Configuration>(jsonContent, options);
        
        if (configuration is null)
        {
            throw new InvalidOperationException("Could not deserialize configuration");
        }

        return configuration;
    }

    public Model.Configuration LoadConfiguration()
    {
        return LoadConfigurationAsync().GetAwaiter().GetResult();
    }
}