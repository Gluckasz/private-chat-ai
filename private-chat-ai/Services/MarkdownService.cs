using System.Text.RegularExpressions;

namespace PrivateChatAI.Services
{
    public static class MarkdownService
    {
        private static readonly Regex MarkdownRegex;

        static MarkdownService()
        {
            var pattern = @"(\*\*[^*]*\*\*|\*[^*]*\*|_[^_]*_|#{1,6}\s[^\n]*|\n)";
            MarkdownRegex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public static FormattedString ParseMarkdownToFormattedString(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return new FormattedString();

            var formattedString = new FormattedString();
            var segments = ParseMarkdownSegments(markdown);

            foreach (var segment in segments)
            {
                formattedString.Spans.Add(CreateSpan(segment));
            }

            return formattedString;
        }

        private static List<MarkdownSegment> ParseMarkdownSegments(string markdown)
        {
            var segments = new List<MarkdownSegment>();

            var matches = MarkdownRegex.Matches(markdown);

            var lastIndex = 0;

            foreach (Match match in matches)
            {
                if (match.Index > lastIndex)
                {
                    var normalText = markdown[lastIndex..match.Index];
                    if (!string.IsNullOrEmpty(normalText))
                    {
                        segments.Add(
                            new MarkdownSegment { Text = normalText, Type = MarkdownType.Normal }
                        );
                    }
                }

                var matchText = match.Value;
                segments.Add(ClassifyMarkdownSegment(matchText));

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < markdown.Length)
            {
                var remainingText = markdown[lastIndex..];
                if (!string.IsNullOrEmpty(remainingText))
                {
                    segments.Add(
                        new MarkdownSegment { Text = remainingText, Type = MarkdownType.Normal }
                    );
                }
            }

            return segments;
        }

        private static MarkdownSegment ClassifyMarkdownSegment(string text)
        {
            if (text.Length >= 4 && text.StartsWith("**") && text.EndsWith("**"))
                return new MarkdownSegment { Text = text, Type = MarkdownType.Bold };

            if (
                text.Length >= 2
                    && (text.StartsWith('*') && text.EndsWith('*') && !text.StartsWith("**"))
                || (text.StartsWith('_') && text.EndsWith('_') && !text.StartsWith("__"))
            )
                return new MarkdownSegment { Text = text, Type = MarkdownType.Italic };

            if (text.StartsWith('#'))
                return new MarkdownSegment
                {
                    Text = text.Replace("*", "").Replace("_", ""),
                    Type = MarkdownType.Header,
                };

            if (text == "\n")
                return new MarkdownSegment { Text = text, Type = MarkdownType.LineBreak };

            return new MarkdownSegment { Text = text, Type = MarkdownType.Normal };
        }

        private static Span CreateSpan(MarkdownSegment segment)
        {
            var span = new Span();

            switch (segment.Type)
            {
                case MarkdownType.Bold:
                    span.Text = segment.Text[2..^2];
                    span.FontAttributes = FontAttributes.Bold;
                    break;

                case MarkdownType.Italic:
                    span.Text = segment.Text[1..^1];
                    span.FontAttributes = FontAttributes.Italic;
                    break;

                case MarkdownType.Header:
                    var headerLevel = segment.Text.TakeWhile(c => c == '#').Count();
                    span.Text = segment.Text[headerLevel..].Trim();
                    span.FontAttributes = FontAttributes.Bold;
                    span.FontSize = Math.Max(20 - headerLevel, 14);
                    break;

                case MarkdownType.LineBreak:
                    span.Text = "\n";
                    break;

                default:
                    span.Text = segment.Text;
                    break;
            }

            return span;
        }
    }

    public class MarkdownSegment
    {
        public string Text { get; set; } = string.Empty;
        public MarkdownType Type { get; set; }
    }

    public enum MarkdownType
    {
        Normal,
        Bold,
        Italic,
        Header,
        LineBreak,
    }
}
