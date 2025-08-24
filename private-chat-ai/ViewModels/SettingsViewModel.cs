using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PrivateChatAI.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private bool _canClear = !string.IsNullOrWhiteSpace(Config.Instance.ApiKey);
        private bool _canSave = false;

        public string ApiKey
        {
            get => Config.Instance.ApiKey;
            set
            {
                Config.Instance.ApiKey = value;
                OnPropertyChanged();
            }
        }

        public bool CanClear
        {
            get => _canClear;
            set
            {
                _canClear = value;
                OnPropertyChanged();
                ((Command)ClearCommand).ChangeCanExecute();
            }
        }

        public bool CanSave
        {
            get => _canSave;
            set
            {
                _canSave = value;
                OnPropertyChanged();
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand OpenUrlCommand { get; }

        public SettingsViewModel()
        {
            Config.Instance.PropertyChanged += OnConfigPropertyChanged;

            SaveCommand = new Command(async () => await SaveSettingsAsync(), () => CanSave);
            ClearCommand = new Command(async () => await ClearSettingsAsync(), () => CanClear);
            OpenUrlCommand = new Command<string>(async (url) => await OpenUrlAsync(url));
        }

        private async Task OpenUrlAsync(string url)
        {
            try
            {
                await Launcher.OpenAsync(url);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"Unable to open URL: {ex.Message}",
                    "OK"
                );
            }
        }

        private void OnConfigPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Config.ApiKey))
            {
                OnPropertyChanged(nameof(ApiKey));
                CanSave = !string.IsNullOrWhiteSpace(Config.Instance.ApiKey);
            }
            else if (e.PropertyName == Config.ApiKeyLoadFlag)
            {
                OnPropertyChanged(nameof(ApiKey));
                CanClear = !string.IsNullOrWhiteSpace(Config.Instance.ApiKey);
            }
        }

        private async Task SaveSettingsAsync()
        {
            CanClear = false;
            CanSave = false;
            try
            {
                await Config.Instance.SaveApiKeyAsync();

                await Shell.Current.DisplayAlert("Settings", "API key saved successfully!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"Failed to save settings: {ex.Message}",
                    "OK"
                );
            }
            finally
            {
                CanClear = !string.IsNullOrWhiteSpace(Config.Instance.ApiKey);
                CanSave = false;
            }
        }

        private async Task ClearSettingsAsync()
        {
            CanClear = false;
            CanSave = false;
            try
            {
                bool confirmed = await Shell.Current.DisplayAlert(
                    "Clear API Key",
                    "Are you sure you want to clear the API key?",
                    "Yes",
                    "No"
                );

                if (confirmed)
                {
                    await Config.Instance.ClearApiKey();
                    await Shell.Current.DisplayAlert(
                        "Settings",
                        "API key cleared successfully!",
                        "OK"
                    );
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"Failed to clear settings: {ex.Message}",
                    "OK"
                );
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
