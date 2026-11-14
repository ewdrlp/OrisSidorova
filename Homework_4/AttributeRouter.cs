using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace oris0310.Framework
{
    public class AttributeRouter
    {
        private class RouteEntry
        {
            public Type ControllerType = null!;
            public MethodInfo Method = null!;
        }

        private readonly Dictionary<(string, string), RouteEntry> _routes = new();

        public AttributeRouter()
        {
            var asm = Assembly.GetExecutingAssembly();

            foreach (var type in asm.GetTypes())
            {
                var ctrl = type.GetCustomAttribute<HttpControllerAttribute>();
                if (ctrl == null || type.IsAbstract) continue;

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    var get = method.GetCustomAttribute<GetAttribute>();
                    var post = method.GetCustomAttribute<PostAttribute>();

                    if (get != null) Register("GET", ctrl.Name, get.Route, type, method);
                    if (post != null) Register("POST", ctrl.Name, post.Route, type, method);
                }
            }

            Console.WriteLine($"[Router] Зарегистрировано маршрутов: {_routes.Count}");
            foreach (var key in _routes.Keys)
                Console.WriteLine($"  {key.Item1} {key.Item2}");
        }

        private void Register(string httpMethod, string controllerName, string route,
                              Type type, MethodInfo method)
        {
            string path = Normalize(route);
            _routes[(httpMethod, path)] = new RouteEntry { ControllerType = type, Method = method };
        }

        private static string Normalize(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "/";
            path = path.Trim();
            if (!path.StartsWith('/')) path = "/" + path;
            return path.TrimEnd('/').ToLowerInvariant() is "" ? "/" : path.TrimEnd('/').ToLowerInvariant();
        }

        public async Task<bool> TryHandleAsync(HttpListenerContext context)
        {
            string method = context.Request.HttpMethod.ToUpperInvariant();
            string path = Normalize(context.Request.Url?.LocalPath ?? "/");

            if (!_routes.TryGetValue((method, path), out var entry))
                return false;

            var form = ReadInput(context.Request);

            var methodParams = entry.Method.GetParameters();
            var args = new object?[methodParams.Length];
            for (int i = 0; i < methodParams.Length; i++)
            {
                var p = methodParams[i];
                if (p.Name != null && form.TryGetValue(p.Name, out var val))
                    args[i] = val;
                else if (p.HasDefaultValue)
                    args[i] = p.DefaultValue;
                else
                    args[i] = p.ParameterType.IsValueType
                        ? Activator.CreateInstance(p.ParameterType)
                        : null;
            }

            ControllerBase.SetContext(context);
            try
            {
                var controller = Activator.CreateInstance(entry.ControllerType);
                object? result = entry.Method.Invoke(controller, args);

                if (result is Task task)
                    await task;
            }
            catch (TargetInvocationException tie)
            {
                Console.WriteLine($"[Controller Error] {tie.InnerException?.Message}");
                try { context.Response.StatusCode = 500; } catch { }
            }
            finally
            {
                ControllerBase.ClearContext();
            }

            return true;
        }

        private static Dictionary<string, string> ReadInput(HttpListenerRequest request)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (request.HasEntityBody &&
                (request.ContentType ?? "").Contains("application/x-www-form-urlencoded",
                    StringComparison.OrdinalIgnoreCase))
            {
                using var reader = new StreamReader(request.InputStream,
                                                    request.ContentEncoding ?? Encoding.UTF8);
                string body = reader.ReadToEnd();

                foreach (var pair in body.Split('&', StringSplitOptions.RemoveEmptyEntries))
                {
                    var kv = pair.Split('=', 2);
                    string k = Uri.UnescapeDataString(kv[0].Replace('+', ' '));
                    string v = kv.Length == 2
                        ? Uri.UnescapeDataString(kv[1].Replace('+', ' '))
                        : "";
                    dict[k] = v;
                }
            }

            foreach (string? key in request.QueryString.AllKeys)
                if (key != null && !dict.ContainsKey(key))
                    dict[key] = request.QueryString[key] ?? "";

            return dict;
        }
    }
}
