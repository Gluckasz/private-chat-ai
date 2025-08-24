using System.Windows.Input;

namespace PrivateChatAI.Commands
{
    public partial class ClearSettingsCommand(
        Func<bool> getCanClear,
        Action<bool> setCanClear,
        Action<bool> setCanSave
    ) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return getCanClear();
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

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
