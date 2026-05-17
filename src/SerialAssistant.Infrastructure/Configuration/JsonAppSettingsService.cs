using System.Text;
using System.Text.Json;
using SerialAssistant.Core.Models;
using SerialAssistant.Core.Services;

namespace SerialAssistant.Infrastructure.Configuration
{
    /*
     * Implementation of IAppSettingsService using JSON file
     */
    public class JsonAppSettingsService : IAppSettingsService
    {
        private readonly string _settingsFilePath;

        public JsonAppSettingsService()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string settingsDirectory = Path.Combine(appDataPath, "SerialAssistant.Win");
            _settingsFilePath = Path.Combine(settingsDirectory, "settings.json");
        }

        public JsonAppSettingsService(string settingsFilePath)
        {
            _settingsFilePath = settingsFilePath;
        }

        public OperationResult<AppSettings> Load()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                {
                    return OperationResult<AppSettings>.Success(AppSettings.CreateDefault());
                }

                string json = File.ReadAllText(_settingsFilePath, Encoding.UTF8);
                AppSettings settings = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                }) ?? AppSettings.CreateDefault();

                return OperationResult<AppSettings>.Success(settings);
            }
            catch (JsonException)
            {
                return OperationResult<AppSettings>.Success(AppSettings.CreateDefault());
            }
            catch (Exception)
            {
                return OperationResult<AppSettings>.Success(AppSettings.CreateDefault());
            }
        }

        public OperationResult Save(AppSettings settings)
        {
            if (settings == null)
            {
                return OperationResult.Failure("配置不能为空。");
            }

            try
            {
                string directory = Path.GetDirectoryName(_settingsFilePath) ?? string.Empty;
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_settingsFilePath, json, Encoding.UTF8);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"保存配置失败：{ex.Message}");
            }
        }
    }
}
