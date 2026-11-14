using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace oris0310.Framework
{
    public abstract class ControllerBase
    {
        private static readonly AsyncLocal<HttpListenerContext?> _ctx = new();

        public static void SetContext(HttpListenerContext context) => _ctx.Value = context;
        public static void ClearContext() => _ctx.Value = null;

        protected HttpListenerContext Context => _ctx.Value!;
        protected HttpListenerRequest Request => Context.Request;
        protected HttpListenerResponse Response => Context.Response;

        protected static string StaticRoot =>
            Path.Combine(Directory.GetCurrentDirectory(), "static");

        protected void ReturnHtml(string html, int statusCode = 200)
        {
            Response.StatusCode = statusCode;
            Response.ContentType = "text/html; charset=utf-8";
            byte[] bytes = Encoding.UTF8.GetBytes(html);
            Response.ContentLength64 = bytes.Length;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        protected void ReturnFile(string relativePath, int statusCode = 200)
        {
            string fullPath = Path.Combine(StaticRoot, relativePath);

            if (!File.Exists(fullPath))
            {
                Response.StatusCode = 404;
                return;
            }

            Response.StatusCode = statusCode;
            Response.ContentType = ContentTypeHelper.GetContentType(fullPath);

            byte[] bytes = File.ReadAllBytes(fullPath);
            Response.ContentLength64 = bytes.Length;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
        }
    }
}
