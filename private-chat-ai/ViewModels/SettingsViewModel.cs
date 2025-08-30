using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PrivateChatAI.Commands;

namespace PrivateChatAI.ViewModels
{
    public partial class SettingsViewModel : INotifyPropertyChanged
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

        public string SystemPrompt
        {
            get => Config.Instance.SystemPrompt;
            set
            {
                Config.Instance.SystemPrompt = value;
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
                ((SaveApiKeyCommand)SaveCommand).RaiseCanExecuteChanged();
                ((ClearSettingsCommand)ClearCommand).RaiseCanExecuteChanged();
            }
        }

        public bool CanSave
        {
            get => _canSave;
            set
            {
                _canSave = value;
                OnPropertyChanged();
                ((SaveApiKeyCommand)SaveCommand).RaiseCanExecuteChanged();
                ((ClearSettingsCommand)ClearCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand OpenUrlCommand { get; }

        public SettingsViewModel()
        {
            Config.Instance.PropertyChanged += OnConfigPropertyChanged;

            SaveCommand = new SaveApiKeyCommand(
                () => CanSave,
                (canSave) => CanSave = canSave,
                (canClear) => CanClear = canClear
            );

            ClearCommand = new ClearSettingsCommand(
                () => CanClear,
                (canClear) => CanClear = canClear,
                (canSave) => CanSave = canSave
            );

            OpenUrlCommand = new OpenUrlCommand();
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
            else if (e.PropertyName == nameof(Config.SystemPrompt))
            {
                OnPropertyChanged(nameof(SystemPrompt));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
