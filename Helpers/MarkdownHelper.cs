// Helpers/MarkdownHelper.cs
using Markdig;
using Microsoft.AspNetCore.Html; // HtmlString için

namespace YetenekPusulasi.WebApp.Helpers // Namespace'inizi projenize göre ayarlayın
{
    public static class MarkdownHelper
    {
        private static readonly MarkdownPipeline _pipeline;

        static MarkdownHelper()
        {
            // Markdig pipeline'ını yapılandırabilirsiniz.
            // Örneğin, ekstra eklentilerle (tablolar, görev listeleri, emojiler vb.)
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions() // Çoğu yaygın markdown özelliğini etkinleştirir
                // .UsePipeTables() // Tablolar için
                // .UseEmphasisExtras() // Üstü çizili, altı çizili vb. için
                // .UseAutoLinks() // URL'leri otomatik link yapar
                .Build();
        }

        public static HtmlString Parse(string? markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return HtmlString.Empty;
            }
            return new HtmlString(Markdig.Markdown.ToHtml(markdown, _pipeline));
        }
    }
}