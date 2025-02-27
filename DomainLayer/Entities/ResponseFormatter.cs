
namespace DomainLayer.Entities
{
    public class ResponseFormatter<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
