using SerialAssistant.Core.Models;
using SerialAssistant.Core.Services;

namespace SerialAssistant.Tests.Infrastructure
{
    /*
     * Fake AppSettingsService for testing
     */
    public class FakeAppSettingsService : IAppSettingsService
    {
        private AppSettings _savedSettings;
        private bool _shouldFailLoad;
        private bool _shouldFailSave;

        public FakeAppSettingsService()
        {
            _savedSettings = AppSettings.CreateDefault();
        }

        public void SetFailLoad(bool shouldFail)
        {
            _shouldFailLoad = shouldFail;
        }

        public void SetFailSave(bool shouldFail)
        {
            _shouldFailSave = shouldFail;
        }

        public OperationResult<AppSettings> Load()
        {
            if (_shouldFailLoad)
            {
                return OperationResult<AppSettings>.Failure("Load failed.");
            }
            return OperationResult<AppSettings>.Success(_savedSettings);
        }

        public OperationResult Save(AppSettings settings)
        {
            if (_shouldFailSave)
            {
                return OperationResult.Failure("Save failed.");
            }
            _savedSettings = settings;
            LastSavedSettings = settings;
            return OperationResult.Success();
        }

        public AppSettings GetSavedSettings()
        {
            return _savedSettings;
        }

        public AppSettings? LastSavedSettings { get; private set; }

        public void SetSavedSettings(AppSettings settings)
        {
            _savedSettings = settings;
        }
    }
}
