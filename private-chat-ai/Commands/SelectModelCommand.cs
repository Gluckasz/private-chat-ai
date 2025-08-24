using System.Collections.ObjectModel;
using System.Windows.Input;
using PrivateChatAI.Services;

namespace PrivateChatAI.Commands
{
    public partial class SelectModelCommand(
        OpenRouterService openRouterService,
        ObservableCollection<string> availableModels
    ) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            await ExecuteAsync();
        }

        public async Task ExecuteAsync()
        {
            try
            {
                var models = await openRouterService.GetModelsAsync();
                availableModels.Clear();

                foreach (var model in models)
                {
                    availableModels.Add(model);
                }

                if (availableModels.Count > 0)
                {
                    await ShowModelPickerAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No models available", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"Failed to load models: {ex.Message}",
                    "OK"
                );
            }
        }

        private async Task ShowModelPickerAsync()
        {
            var modelNames = availableModels.ToArray();

            var selectedModel = await Shell.Current.DisplayActionSheet(
                "Select Model",
                "Cancel",
                null,
                modelNames
            );

            if (!string.IsNullOrEmpty(selectedModel) && selectedModel != "Cancel")
            {
                Config.Instance.SelectedModel = selectedModel;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
