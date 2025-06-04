// Infrastructure/AIAdapters/GoogleGeminiAdapter.cs
using System.Threading.Tasks;
using YetenekPusulasi.Core.Interfaces.AI;

namespace YetenekPusulasi.Infrastructure.AIAdapters
{
    public class GoogleGeminiAdapter : IAIModelAdapter
    {
        public string ModelIdentifier => "Google-Gemini-Mock";

        public GoogleGeminiAdapter() { }

        public async Task<AIAdapterResponse> GetCompletionAsync(AIAdapterRequest request)
        {
            // --- GERÇEK GEMINI API ÇAĞRISI BURADA OLACAK ---

            // --- MOCK CEVAP ---
            await Task.Delay(600);
            if (request.UserPrompt.ToLower().Contains("problem"))
            {
                return new AIAdapterResponse { IsSuccess = false, ErrorMessage = "Mock Gemini: Problem kelimesi algılandı, analiz edilemedi." };
            }
            return new AIAdapterResponse
            {
                IsSuccess = true,
                Content = $"Mock Gemini Değerlendirmesi: '{request.UserPrompt}' prompt'u '{request.SystemPrompt.Substring(0, Math.Min(30, request.SystemPrompt.Length))}...' sistem mesajıyla işlendi. Öğrenci potansiyel gösteriyor."
            };
        }
    }
}