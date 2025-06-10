// Infrastructure/AIAdapters/GoogleGeminiAdapter.cs
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text; // Encoding için
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using YetenekPusulasi.Core.Interfaces.AI;

namespace YetenekPusulasi.Infrastructure.AIAdapters
{
    // Gemini API DTO'ları (değişiklik yok, aynı kalacaklar)
    public class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent? Content { get; set; }
        [JsonPropertyName("finishReason")]
        public string? FinishReason { get; set; }
    }

    public class GeminiContent
    {
        [JsonPropertyName("parts")]
        public GeminiPart[]? Parts { get; set; }
        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }

    public class GeminiPart
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    public class GeminiApiResponse
    {
        [JsonPropertyName("candidates")]
        public GeminiCandidate[]? Candidates { get; set; }
        [JsonPropertyName("promptFeedback")]
        public GeminiPromptFeedback? PromptFeedback { get; set; }
    }

    public class GeminiPromptFeedback
    {
        [JsonPropertyName("blockReason")]
        public string? BlockReason { get; set; }
    }


    public class GoogleGeminiAdapter : IAIModelAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration; // _configuration field'ını eklemiştik
        private readonly string _apiKey;
        private readonly string _modelName;
        private readonly string _apiEndpoint;
        private readonly ILogger<GoogleGeminiAdapter> _logger;

        public string ModelIdentifier => "Google-Gemini-Mock"; // VEYA istediğiniz tutarlı bir ID

        public GoogleGeminiAdapter(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GoogleGeminiAdapter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory?.CreateClient("GoogleGemini") ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration)); // Atama yapılmıştı

            _apiKey = _configuration["GoogleGemini:ApiKey"];
            _modelName = _configuration["GoogleGemini:ModelName"] ?? "gemini-pro";

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogCritical("Google Gemini API Key not configured in appsettings (GoogleGemini:ApiKey). Adapter will not function.");
                throw new InvalidOperationException("Google Gemini API Key not configured in appsettings (GoogleGemini:ApiKey).");
            }
             _logger.LogInformation("GoogleGeminiAdapter initialized. API Key Loaded (Last 4 chars): ...{ApiKeyEnding}, Model: {ModelName}",
                _apiKey.Length > 4 ? _apiKey.Substring(_apiKey.Length - 4) : "****", _modelName);


            _apiEndpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{_modelName}:generateContent?key={_apiKey}";

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AIAdapterResponse> GetCompletionAsync(AIAdapterRequest request)
        {
            _logger.LogInformation("GetCompletionAsync called for model: {ModelIdentifier}", ModelIdentifier);

            if (string.IsNullOrWhiteSpace(request.UserPrompt))
            {
                _logger.LogWarning("User prompt is empty for Gemini request.");
                var emptyPromptResponse = new AIAdapterResponse { IsSuccess = false, ErrorMessage = "User prompt cannot be empty." };
                LogAdapterResponse(emptyPromptResponse, "Empty user prompt."); // Konsola/Log'a çıktı
                return emptyPromptResponse;
            }

            string combinedPrompt = request.UserPrompt;
            if (!string.IsNullOrWhiteSpace(request.SystemPrompt))
            {
                combinedPrompt = $"{request.SystemPrompt}\n\n---\n\nÖğrenci Cevabı/İstek:\n{request.UserPrompt}";
                _logger.LogDebug("System prompt combined with user prompt.");
            }
             _logger.LogDebug("Combined prompt for Gemini: {CombinedPrompt}", combinedPrompt.Substring(0, Math.Min(combinedPrompt.Length, 200)) + (combinedPrompt.Length > 200 ? "..." : "")); // İlk 200 karakteri logla


            var payload = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = combinedPrompt } } }
                },
                generationConfig = new
                {
                    temperature = request.Temperature,
                    // candidateCount = 1, // Genellikle varsayılan 1'dir
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending request to Gemini API. Endpoint: {Endpoint}", _apiEndpoint);
            _logger.LogDebug("Gemini Request Payload JSON: {PayloadJson}", jsonPayload);

            AIAdapterResponse adapterResponse;

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_apiEndpoint, httpContent);
                string responseContentString = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received HTTP response from Gemini API. Status Code: {StatusCode}", response.StatusCode);
                _logger.LogDebug("Gemini Raw Response String: {RawResponse}", responseContentString);

                if (response.IsSuccessStatusCode)
                {
                    GeminiApiResponse? apiResponse = null;
                    try
                    {
                        apiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(responseContentString);
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, "Failed to deserialize Gemini API JSON response.");
                        adapterResponse = new AIAdapterResponse { IsSuccess = false, ErrorMessage = "Failed to parse Gemini API JSON response.", Content = responseContentString };
                        LogAdapterResponse(adapterResponse, "JSON parsing error.");
                        return adapterResponse;
                    }


                    if (apiResponse?.PromptFeedback?.BlockReason != null)
                    {
                        _logger.LogWarning("Gemini API request blocked. Reason: {BlockReason}. Full Response: {FullResponse}", apiResponse.PromptFeedback.BlockReason, responseContentString);
                        adapterResponse = new AIAdapterResponse { IsSuccess = false, ErrorMessage = $"Gemini API request blocked: {apiResponse.PromptFeedback.BlockReason}", Content = responseContentString };
                    }
                    else
                    {
                        var generatedText = apiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
                        if (!string.IsNullOrEmpty(generatedText))
                        {
                            _logger.LogInformation("Successfully parsed content from Gemini API response.");
                            adapterResponse = new AIAdapterResponse { IsSuccess = true, Content = generatedText };
                        }
                        else
                        {
                            _logger.LogWarning("Gemini API returned success but no usable content (text part) was found in the response candidates. Full response: {FullResponse}", responseContentString);
                            adapterResponse = new AIAdapterResponse { IsSuccess = false, ErrorMessage = "Gemini API returned success but no usable content found in candidates.", Content = responseContentString };
                        }
                    }
                }
                else
                {
                    _logger.LogError("Error response from Gemini API. Status: {StatusCode}. Content: {ErrorContent}", response.StatusCode, responseContentString);
                    adapterResponse = new AIAdapterResponse { IsSuccess = false, ErrorMessage = $"Gemini API Error: {response.StatusCode}", Content = responseContentString };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP Request Exception during Gemini API call to {Endpoint}.", _apiEndpoint);
                adapterResponse = new AIAdapterResponse { IsSuccess = false, ErrorMessage = $"HTTP Request Exception: {ex.Message}" };
            }
            catch (TaskCanceledException ex) // Timeout gibi durumlar için
            {
                _logger.LogError(ex, "Task Canceled Exception (possibly timeout) during Gemini API call to {Endpoint}.", _apiEndpoint);
                adapterResponse = new AIAdapterResponse { IsSuccess = false, ErrorMessage = $"Task Canceled (Timeout?): {ex.Message}" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception during Gemini API call to {Endpoint}.", _apiEndpoint);
                adapterResponse = new AIAdapterResponse { IsSuccess = false, ErrorMessage = $"Unexpected Exception: {ex.Message}" };
            }

            LogAdapterResponse(adapterResponse, "Final adapter response."); // Her durumda konsola/log'a çıktı
            return adapterResponse;
        }

        // Helper metot, konsola/log'a daha düzenli çıktı vermek için
        private void LogAdapterResponse(AIAdapterResponse response, string contextMessage)
        {
            Console.WriteLine($"--- {contextMessage} ---"); // Direkt konsola da yazalım
            _logger.LogInformation("Adapter Response Context: {ContextMessage}", contextMessage);
            _logger.LogInformation("Adapter Response IsSuccess: {IsSuccess}", response.IsSuccess);
            if (response.IsSuccess)
            {
                _logger.LogInformation("Adapter Response Content (first 100 chars): {ContentPreview}...", response.Content?.Substring(0, Math.Min(response.Content.Length, 100)));
                Console.WriteLine($"Adapter IsSuccess: true");
                Console.WriteLine($"Adapter Content (preview): {response.Content?.Substring(0, Math.Min(response.Content.Length, 100))}...");
            }
            else
            {
                _logger.LogError("Adapter Response ErrorMessage: {ErrorMessage}", response.ErrorMessage);
                _logger.LogDebug("Adapter Response Full Content (if any from error): {ErrorContent}", response.Content);
                Console.WriteLine($"Adapter IsSuccess: false");
                Console.WriteLine($"Adapter ErrorMessage: {response.ErrorMessage}");
            }
            Console.WriteLine("--- End of Adapter Response ---");
        }
    }
}