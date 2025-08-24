using System.Globalization;
using PrivateChatAI.Services;

namespace PrivateChatAI.Converters
{
    public class MarkdownToFormattedStringConverter : IValueConverter
    {
        public object? Convert(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture
        )
        {
            if (value is not string markdown || string.IsNullOrEmpty(markdown))
                return new FormattedString();

            return MarkdownService.ParseMarkdownToFormattedString(markdown);
        }

        public object? ConvertBack(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }
}
