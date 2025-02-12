using Castle.Components.DictionaryAdapter.Xml;
using SurrealDb.Net.Models;
using System.Text.Json.Serialization;

namespace Popoupa.Core.DomainModels
{
    public class User
    {
        [JsonPropertyName("user_id")]
        public Guid Id { get; }        
        [JsonPropertyName("name")]
        public string Name { get; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email { get; } = string.Empty;
        [JsonPropertyName("password")]
        public string Password { get; } = string.Empty;
    }
}
