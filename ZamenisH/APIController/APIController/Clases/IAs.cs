using System.Collections.Generic;

namespace APIController.Clases
{
    public class IAs
    {
        public class PerplexityRequest
        {
            public string model { get; set; }
            public List<Message> messages { get; set; }
            public int? max_tokens { get; set; }
            public double? temperature { get; set; }
            public string APIKey { get; set; }
        }

        public class Message
        {
            public string role { get; set; }      // "user", "assistant", "system"
            public string content { get; set; }
        }
        public class ChatRequest
        {
            public string model { get; set; }
            public List<Message> messages { get; set; }
            public int? max_tokens { get; set; }
            public double? temperature { get; set; }
            public double? top_p { get; set; }
        }
        public class ChatResponse
        {
            public string id { get; set; }
            public string @object { get; set; }
            public long created { get; set; }
            public string model { get; set; }
            public List<Choice> choices { get; set; }
            public Usage usage { get; set; }
        }
        public class Choice
        {
            public int index { get; set; }
            public Message message { get; set; }
            public string finish_reason { get; set; }
        }

        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int completion_tokens { get; set; }
            public int total_tokens { get; set; }
        }
    }
}
