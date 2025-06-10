
// Core/Services/AiConnectionTester.cs (YENİ)
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using YetenekPusulasi.Core.Interfaces.AI;
using YetenekPusulasi.Core.Interfaces.Services;

namespace YetenekPusulasi.Core.Services
{
    public class AiConnectionTester : IAiConnectionTester
    {
        private readonly IEnumerable<IAIModelAdapter> _aiAdapters;
        private readonly ILogger<AiConnectionTester> _logger;

        public AiConnectionTester(IEnumerable<IAIModelAdapter> aiAdapters, ILogger<AiConnectionTester> logger)
        {
            _aiAdapters = aiAdapters;
            _logger = logger;
        }

        public async Task<(bool isSuccess, string message)> TestConnectionAsync(string aiModelIdentifier)
        {
            var adapter = _aiAdapters.FirstOrDefault(a =>
                a.ModelIdentifier.Equals(aiModelIdentifier, StringComparison.OrdinalIgnoreCase));

            if (adapter == null)
            {
                _logger.LogWarning("AI Connection Test: Adapter for '{ModelIdentifier}' not found.", aiModelIdentifier);
                return (false, $"Adapter for '{aiModelIdentifier}' not found.");
            }

            _logger.LogInformation("AI Connection Test: Testing adapter '{ModelIdentifier}'.", aiModelIdentifier);
            try
            {
                var testRequest = new AIAdapterRequest
                {
                    SystemPrompt = "You are a helpful assistant.",
                    UserPrompt = "Hello! Is the API connection working? Respond with a short confirmation.",
                    Temperature = 0.1, // Düşük temperature, daha deterministik cevap için
                    MaxTokens = 50
                };
                var response = await adapter.GetCompletionAsync(testRequest);

                if (response.IsSuccess && !string.IsNullOrEmpty(response.Content))
                {
                    _logger.LogInformation("AI Connection Test for '{ModelIdentifier}' SUCCEEDED. Response: {ResponseContent}", aiModelIdentifier, response.Content);
                    return (true, $"Connection to '{aiModelIdentifier}' successful. Response: {response.Content}");
                }
                else
                {
                    _logger.LogError("AI Connection Test for '{ModelIdentifier}' FAILED. Error: {ErrorMessage}, RawContent: {RawContent}",
                        aiModelIdentifier, response.ErrorMessage, response.Content);
                    return (false, $"Connection to '{aiModelIdentifier}' failed. Error: {response.ErrorMessage}. Raw: {response.Content}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI Connection Test for '{ModelIdentifier}' threw an exception.", aiModelIdentifier);
                return (false, $"Exception during connection test to '{aiModelIdentifier}': {ex.Message}");
            }
        }
    }
}