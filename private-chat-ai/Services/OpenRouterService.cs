using System.Text;
using System.Text.Json;
using PrivateChatAI.Models.Interfaces;

namespace PrivateChatAI.Services
{
    public class OpenRouterService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions options;

        public OpenRouterService()
        {
            _httpClient = new HttpClient();
            options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        public async Task<IReadOnlyList<string>> GetModelsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://openrouter.ai/api/v1/models");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();

                using var jsonDocument = JsonDocument.Parse(jsonContent);
                var root = jsonDocument.RootElement;

                if (
                    root.TryGetProperty("data", out var dataArray)
                    && dataArray.ValueKind == JsonValueKind.Array
                )
                {
                    var modelIds = new List<string>();

                    foreach (var model in dataArray.EnumerateArray())
                    {
                        if (
                            model.TryGetProperty("id", out var nameProperty)
                            && nameProperty.ValueKind == JsonValueKind.String
                        )
                        {
                            var name = nameProperty.GetString();
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                modelIds.Add(name);
                            }
                        }
                    }

                    return modelIds.AsReadOnly();
                }

                return new List<string>().AsReadOnly();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching models: {ex.Message}");
                return new List<string>().AsReadOnly();
            }
        }

        public async Task<string> SendChatCompletionAsync(IEnumerable<IChatMessage> messages)
        {
            try
            {
                var apiKey = Config.Instance.ApiKey;
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new InvalidOperationException("API key is not configured");
                }

                var selectedModel = Config.Instance.SelectedModel;
                if (string.IsNullOrWhiteSpace(selectedModel))
                {
                    throw new InvalidOperationException("No model selected");
                }

                var requestBody = new
                {
                    model = selectedModel,
                    messages = messages
                        .Select(msg => new
                        {
                            role = msg.IsUser ? "user" : "assistant",
                            content = msg.Content,
                        })
                        .ToArray(),
                };

                var json = JsonSerializer.Serialize(requestBody, options);

                using var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://openrouter.ai/api/v1/chat/completions"
                );
                request.Headers.Add("Authorization", $"Bearer {apiKey}");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                using var jsonDocument = JsonDocument.Parse(responseContent);
                var root = jsonDocument.RootElement;

                if (
                    root.TryGetProperty("choices", out var choicesArray)
                    && choicesArray.ValueKind == JsonValueKind.Array
                    && choicesArray.GetArrayLength() > 0
                )
                {
                    var firstChoice = choicesArray[0];
                    if (
                        firstChoice.TryGetProperty("message", out var messageObj)
                        && messageObj.TryGetProperty("content", out var contentProperty)
                        && contentProperty.ValueKind == JsonValueKind.String
                    )
                    {
                        return contentProperty.GetString() ?? "No response content";
                    }
                }

                return "No response received";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending chat completion: {ex.Message}");
                throw;
            }
        }
    }
}
