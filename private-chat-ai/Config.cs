using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PrivateChatAI
{
    public partial class Config : INotifyPropertyChanged
    {
        private static Config? _instance;
        private static readonly Lock _lock = new();
        private string _apiKey = string.Empty;
        private string _selectedModel = string.Empty;
        private string _systemPrompt = string.Empty;
        private const string ApiKeyStorageKey = "PrivateChatAI_ApiKey";
        private const string SelectedModelStorageKey = "PrivateChatAI_SelectedModel";
        private const string SystemPromptStorageKey = "PrivateChatAI_SystemPrompt";
        public const string ApiKeyLoadFlag = "ApiKeyLoad";

        private Config()
        {
            _ = LoadSettingsAsync();
        }

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new Config();
                    }
                }
                return _instance;
            }
        }

        public string ApiKey
        {
            get => _apiKey;
            set
            {
                if (_apiKey != value)
                {
                    _apiKey = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SelectedModel
        {
            get => _selectedModel;
            set
            {
                if (_selectedModel != value)
                {
                    _selectedModel = value;
                    OnPropertyChanged();
                    SaveSelectedModel();
                }
            }
        }

        public string SystemPrompt
        {
            get => _systemPrompt;
            set
            {
                if (_systemPrompt != value)
                {
                    _systemPrompt = value;
                    OnPropertyChanged();
                    SaveSystemPrompt();
                }
            }
        }

        private async Task LoadSettingsAsync()
        {
            _apiKey = await SecureStorage.GetAsync(ApiKeyStorageKey) ?? string.Empty;
            OnPropertyChanged(ApiKeyLoadFlag);

            _selectedModel = Preferences.Get(SelectedModelStorageKey, string.Empty);
            OnPropertyChanged(nameof(SelectedModel));

            _systemPrompt = Preferences.Get(SystemPromptStorageKey, string.Empty);
            OnPropertyChanged(nameof(SystemPrompt));
        }

        public async Task SaveApiKeyAsync()
        {
            await SecureStorage.SetAsync(ApiKeyStorageKey, _apiKey);
        }

        private void SaveSelectedModel()
        {
            Preferences.Set(SelectedModelStorageKey, _selectedModel);
        }

        private void SaveSystemPrompt()
        {
            Preferences.Set(SystemPromptStorageKey, _systemPrompt);
        }

        public Task ClearApiKey()
        {
            SecureStorage.Remove(ApiKeyStorageKey);
            ApiKey = string.Empty;
            return Task.CompletedTask;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
