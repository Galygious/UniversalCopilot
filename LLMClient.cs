// LLMClient.cs:
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyLLMIntegrationApp
{
    public static class LLMClient
    {
        private static readonly HttpClient httpClient = new HttpClient() { BaseAddress = new System.Uri("http://localhost:1234") };
        private const string ModelName = "qwen2.5-coder-3b-instruct@q4_0";

        public static async Task<string> GetCompletionAsync(string prompt)
        {
            Console.WriteLine("Requesting completion from LLM");
            try
            {
                Console.WriteLine("Crafting request");
                var requestBody = new
                {
                    model = ModelName,
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    }
                };

                Console.WriteLine("Request: " + JsonSerializer.Serialize(requestBody));
                Console.WriteLine("Sending request");

                var response = await httpClient.PostAsJsonAsync("/v1/chat/completions", requestBody);
                Console.WriteLine("Response received");
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Response status code: " + response.StatusCode);
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                Console.WriteLine("Response content: " + json.ToString());
                var content = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                Console.WriteLine("LLM response: " + content);
                return content ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("LLM request failed: " + ex.Message);
                return "";
            }
        }

    }
}
