using System.Windows.Input;

namespace PrivateChatAI.Commands
{
    public partial class SaveSettingsCommand(
        Func<bool> getCanSave,
        Action<bool> setCanSave,
        Action<bool> setCanClear
    ) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return getCanSave();
        }

        public async void Execute(object? parameter)
        {
            await ExecuteAsync();
        }

        public async Task ExecuteAsync()
        {
            setCanClear(false);
            setCanSave(false);
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
                setCanClear(!string.IsNullOrWhiteSpace(Config.Instance.ApiKey));
                setCanSave(false);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
