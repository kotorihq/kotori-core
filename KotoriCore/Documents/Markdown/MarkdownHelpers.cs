namespace KotoriCore.Documents
{
    /// <summary>
    /// Markdown helpers.
    /// </summary>
    public static class MarkdownHelpers
    {
        ///// <summary>
        ///// Converts markdown to html.
        ///// </summary>
        ///// <returns>The html.</returns>
        ///// <param name="markdown">Markdown.</param>
        public static string ToHtml(this string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return markdown;

            var result = CommonMark.CommonMarkConverter.Convert(markdown);

            return result;
        }
    }
}
