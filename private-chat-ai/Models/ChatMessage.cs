using System.ComponentModel;
using System.Runtime.CompilerServices;
using PrivateChatAI.Models.Interfaces;

namespace PrivateChatAI.Models
{
    public partial class ChatMessage : IChatMessage, INotifyPropertyChanged
    {
        private string _content = string.Empty;

        public Guid Id { get; } = Guid.NewGuid();

        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        public bool IsUser { get; set; } = true;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
