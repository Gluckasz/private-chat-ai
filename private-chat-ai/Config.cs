using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PrivateChatAI
{
    public class Config : INotifyPropertyChanged
    {
        private static Config? _instance;
        private static readonly object _lock = new object();
        private string _apiKey;
        private const string ApiKeyStorageKey = "ChatAI_ApiKey";
        public const string ApiKeyLoadFlag = "ApiKeyLoad";

        private Config()
        {
            _ = LoadApiKeyAsync();
        }

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Config();
                        }
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

        private async Task LoadApiKeyAsync()
        {
            _apiKey = await SecureStorage.GetAsync(ApiKeyStorageKey) ?? string.Empty;
            OnPropertyChanged(ApiKeyLoadFlag);
        }

        public async Task SaveApiKeyAsync()
        {
            await SecureStorage.SetAsync(ApiKeyStorageKey, _apiKey);
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
