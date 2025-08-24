using System.Text.Json;

namespace PrivateChatAI.Services
{
    public class OpenRouterService
    {
        private readonly HttpClient _httpClient;

        public OpenRouterService()
        {
            _httpClient = new HttpClient();
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
    }
}
