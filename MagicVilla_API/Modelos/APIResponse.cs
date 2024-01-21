using System.Net;

namespace MagicVilla_API.Modelos
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessage = new List<string>();
        }
        public HttpStatusCode statusCode { get; set; }
        public bool IsExitoso { get; set; } = true;
        public List<string> ErrorMessage { get; set; }
        public object Resultado { get; set; }
    }
}
