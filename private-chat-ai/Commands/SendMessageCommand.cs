using System.Collections.ObjectModel;
using System.Windows.Input;
using PrivateChatAI.Models;
using PrivateChatAI.Models.Interfaces;
using PrivateChatAI.Services;

namespace PrivateChatAI.Commands
{
    public partial class SendMessageCommand(
        ObservableCollection<IChatMessage> messages,
        Func<ChatMessage> getCurrentMessage,
        Action<ChatMessage> setCurrentMessage,
        Func<bool> getIsLoading,
        Action<bool> setIsLoading,
        OpenRouterService openRouterService
    ) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            var currentMessage = getCurrentMessage();
            var isLoading = getIsLoading();
            return !string.IsNullOrWhiteSpace(currentMessage.Content) && !isLoading;
        }

        public async void Execute(object? parameter)
        {
            await ExecuteAsync();
        }

        public async Task ExecuteAsync()
        {
            var currentMessage = getCurrentMessage();
            if (string.IsNullOrWhiteSpace(currentMessage.Content))
                return;

            messages.Add(currentMessage);
            setCurrentMessage(new ChatMessage());

            setIsLoading(true);

            try
            {
                var response = await openRouterService.SendChatCompletionAsync(messages);

                var botResponse = new ChatMessage
                {
                    Content = response,
                    IsUser = false,
                    Timestamp = DateTime.Now,
                };

                messages.Add(botResponse);
            }
            catch (Exception ex)
            {
                var errorMessage = new ChatMessage
                {
                    Content = $"Sorry, there was an error: {ex.Message}",
                    IsUser = false,
                    Timestamp = DateTime.Now,
                };

                messages.Add(errorMessage);
            }
            finally
            {
                setIsLoading(false);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
