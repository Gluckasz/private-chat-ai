using PrivateChatAI.Models.Interfaces;

namespace PrivateChatAI.Selectors
{
    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? UserMessageTemplate { get; set; }
        public DataTemplate? BotMessageTemplate { get; set; }

        protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
        {
            if (item is IChatMessage message)
            {
                return message.IsUser ? UserMessageTemplate : BotMessageTemplate;
            }
            return null;
        }
    }
}
