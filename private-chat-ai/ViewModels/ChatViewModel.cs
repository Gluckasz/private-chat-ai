using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PrivateChatAI.Models;
using PrivateChatAI.Models.Interfaces;
using PrivateChatAI.Services;

namespace PrivateChatAI.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private ChatMessage _currentMessage = new();
        private bool _isLoading = false;
        private bool _isLoadingModels = false;
        private readonly OpenRouterService _openRouterService;

        public ObservableCollection<IChatMessage> Messages { get; } = new();
        public ObservableCollection<string> AvailableModels { get; } = new();

        public ChatMessage CurrentMessage
        {
            get => _currentMessage;
            set
            {
                if (_currentMessage != null)
                    _currentMessage.PropertyChanged -= OnCurrentMessagePropertyChanged;

                _currentMessage = value;

                if (_currentMessage != null)
                    _currentMessage.PropertyChanged += OnCurrentMessagePropertyChanged;

                OnPropertyChanged();
                ((Command)SendMessageCommand).ChangeCanExecute();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                ((Command)SendMessageCommand).ChangeCanExecute();
            }
        }

        public bool IsLoadingModels
        {
            get => _isLoadingModels;
            set
            {
                _isLoadingModels = value;
                OnPropertyChanged();
            }
        }

        public string SelectedModelName
        {
            get =>
                string.IsNullOrEmpty(Config.Instance.SelectedModel)
                    ? "Select Model"
                    : Config.Instance.SelectedModel;
        }

        public ICommand SendMessageCommand { get; }
        public ICommand SelectModelCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ChatViewModel(OpenRouterService openRouterService)
        {
            _openRouterService = openRouterService;

            SendMessageCommand = new Command(async () => await SendMessageAsync(), CanSendMessage);
            SelectModelCommand = new Command(async () => await ShowModelSelectionAsync());

            _currentMessage.PropertyChanged += OnCurrentMessagePropertyChanged;
            Config.Instance.PropertyChanged += OnConfigPropertyChanged;

            Messages.Add(
                new ChatMessage
                {
                    Content = "Hello! How can I help you today?",
                    IsUser = false,
                    Timestamp = DateTime.Now,
                }
            );
        }

        private void OnConfigPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Config.SelectedModel))
            {
                OnPropertyChanged(nameof(SelectedModelName));
            }
        }

        private async Task ShowModelSelectionAsync()
        {
            IsLoadingModels = true;
            try
            {
                var models = await _openRouterService.GetModelsAsync();
                AvailableModels.Clear();

                foreach (var model in models)
                {
                    AvailableModels.Add(model);
                }

                if (AvailableModels.Count > 0)
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
            finally
            {
                IsLoadingModels = false;
            }
        }

        private async Task ShowModelPickerAsync()
        {
            var modelNames = AvailableModels.ToArray();

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

        private void OnCurrentMessagePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChatMessage.Content))
            {
                ((Command)SendMessageCommand).ChangeCanExecute();
            }
        }

        private bool CanSendMessage()
        {
            return !string.IsNullOrWhiteSpace(CurrentMessage.Content) && !IsLoading;
        }

        private async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentMessage.Content))
                return;

            Messages.Add(CurrentMessage);
            string content = CurrentMessage.Content;
            CurrentMessage = new();

            IsLoading = true;

            try
            {
                // TODO: Make API call
                await Task.Delay(1500);

                var botResponse = new ChatMessage
                {
                    Content =
                        $"I received your message: \"{content}\". This is a placeholder response.",
                    IsUser = false,
                    Timestamp = DateTime.Now,
                };

                Messages.Add(botResponse);
            }
            catch (Exception ex)
            {
                var errorMessage = new ChatMessage
                {
                    Content = $"Sorry, there was an error: {ex.Message}",
                    IsUser = false,
                    Timestamp = DateTime.Now,
                };

                Messages.Add(errorMessage);
            }
            finally
            {
                CurrentMessage = new();
                IsLoading = false;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
