using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PrivateChatAI.Commands;
using PrivateChatAI.Models;
using PrivateChatAI.Models.Interfaces;
using PrivateChatAI.Services;

namespace PrivateChatAI.ViewModels
{
    public partial class ChatViewModel : INotifyPropertyChanged
    {
        private ChatMessage _currentMessage = new();
        private bool _isLoading = false;
        private readonly OpenRouterService _openRouterService;

        public ObservableCollection<IChatMessage> Messages { get; } = [];
        public ObservableCollection<string> AvailableModels { get; } = [];

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
                ((SendMessageCommand)SendMessageCommand).RaiseCanExecuteChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                ((SendMessageCommand)SendMessageCommand).RaiseCanExecuteChanged();
            }
        }

        public string SelectedModelName
        {
            get =>
                string.IsNullOrWhiteSpace(Config.Instance.SelectedModel)
                    ? "Select Model"
                    : Config.Instance.SelectedModel;
        }

        public ICommand SendMessageCommand { get; }
        public ICommand SelectModelCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ChatViewModel(OpenRouterService openRouterService)
        {
            _openRouterService = openRouterService;

            SendMessageCommand = new SendMessageCommand(
                Messages,
                () => CurrentMessage,
                (message) => CurrentMessage = message,
                () => IsLoading,
                (loading) => IsLoading = loading
            );

            SelectModelCommand = new SelectModelCommand(_openRouterService, AvailableModels);

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

        private void OnCurrentMessagePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChatMessage.Content))
            {
                ((SendMessageCommand)SendMessageCommand).RaiseCanExecuteChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
