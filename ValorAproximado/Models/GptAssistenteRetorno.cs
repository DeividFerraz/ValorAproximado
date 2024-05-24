namespace ValorAproximado.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class GptAssistenteRetorno
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("data")]
        public List<MessageData> Data { get; set; }

        [JsonPropertyName("first_id")]
        public string FirstId { get; set; }

        [JsonPropertyName("last_id")]
        public string LastId { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }

    public class MessageData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("assistant_id")]
        public string AssistantId { get; set; }

        [JsonPropertyName("thread_id")]
        public string ThreadId { get; set; }

        [JsonPropertyName("run_id")]
        public string RunId { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public List<Content> Content { get; set; }

        [JsonPropertyName("attachments")]
        public List<object> Attachments { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        public TextContent Text { get; set; }
    }

    public class TextContent
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("annotations")]
        public List<object> Annotations { get; set; }
    }
}
