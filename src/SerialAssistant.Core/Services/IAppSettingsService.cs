using SerialAssistant.Core.Models;

namespace SerialAssistant.Core.Services
{
    /*
     * Interface for application settings service
     */
    public interface IAppSettingsService
    {
        OperationResult<AppSettings> Load();

        OperationResult Save(AppSettings settings);
    }
}
