using System.Text.Json.Serialization;

namespace ArticleProject.Application.Common
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }
        public Response(string message)
        {
            Succeeded = false;
            Message = message;
        }
        [JsonPropertyName("succeeded")]
        public bool Succeeded { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; }
        [JsonPropertyName("data")]
        public T Data { get; set; }

        public static Response<T> Success(T data, string message = null)
        {
            return new Response<T>(data, message);
        }
        public static Response<T> Failure(string message)
        {
            return new Response<T>(message);
        }
    }

    public class Response
    {
        private Response()
        {
        }
        public Response(bool succeeded, string message)
        {
            Succeeded = succeeded;
            Message = message;
        }
        [JsonPropertyName("succeeded")]
        public bool Succeeded { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; }

        public static Response Success(string message = null)
        {
            return new Response(true, message);
        }
        public static Response Failure(string message)
        {
            return new Response(false, message);
        }
    }
}
