using System.Threading.Tasks;

namespace YetenekPusulasi.Core.Interfaces.AI
{
    public class AIAdapterRequest
    {
        public string SystemPrompt { get; set; }
        public string UserPrompt { get; set; }
        public int MaxTokens { get; set; } = 150; // Varsayılan değer
        public double Temperature { get; set; } = 0.7; // Varsayılan değer
        // Modele özgü diğer parametreler eklenebilir
    }

    public class AIAdapterResponse
    {
        public bool IsSuccess { get; set; }
        public string? Content { get; set; } // AI'dan gelen ham metin cevabı
        public string? ErrorMessage { get; set; }
    }

    public interface IAIModelAdapter
    {
        Task<AIAdapterResponse> GetCompletionAsync(AIAdapterRequest request);
        string ModelIdentifier { get; } // Hangi AI modelini temsil ettiğini belirtir (örn: "OpenAI-GPT3.5", "Gemini-Pro")
    }
}