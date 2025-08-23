using PrivateChatAI.ViewModels;

namespace PrivateChatAI.Pages
{
    public partial class ChatPage : ContentPage
    {
        public ChatViewModel ViewModel { get; }

        public ChatPage(ChatViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (ViewModel.Messages.Count > 0)
            {
                Dispatcher.Dispatch(async () =>
                {
                    await Task.Delay(100);
                    MessagesCollectionView.ScrollTo(
                        ViewModel.Messages.Last(),
                        position: ScrollToPosition.End
                    );
                });
            }
        }
    }
}
