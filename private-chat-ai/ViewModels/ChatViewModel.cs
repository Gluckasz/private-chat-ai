using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PrivateChatAI.Models;
using PrivateChatAI.Models.Interfaces;

namespace PrivateChatAI.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private ChatMessage _currentMessage = new();
        private bool _isLoading = false;

        public ObservableCollection<IChatMessage> Messages { get; } = new();

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

        public ICommand SendMessageCommand { get; }

        public ChatViewModel()
        {
            SendMessageCommand = new Command(async () => await SendMessageAsync(), CanSendMessage);
            _currentMessage.PropertyChanged += OnCurrentMessagePropertyChanged;

            Messages.Add(
                new ChatMessage
                {
                    Content = "Hello! How can I help you today?",
                    IsUser = false,
                    Timestamp = DateTime.Now,
                }
            );
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
