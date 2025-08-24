using System.Windows.Input;

namespace PrivateChatAI.Commands
{
    public partial class OpenUrlCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return parameter is string url && !string.IsNullOrWhiteSpace(url);
        }

        public async void Execute(object? parameter)
        {
            if (parameter is string url)
            {
                await ExecuteAsync(url);
            }
        }

        public static async Task ExecuteAsync(string url)
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

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
