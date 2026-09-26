using System.Net;
using System.Text;
using System.Text.Json;

namespace HttpServerdz
{
    public class HttpServer
    {
        HttpListener server = new HttpListener();

        public async Task Start()
        {
            if (!File.Exists("settings.json"))
            {
                Console.WriteLine("Введите путь");
                File.WriteAllText("settings.json", Console.ReadLine());
            }

            string settingsJson = File.ReadAllText("settings.json");
            Settings setting = JsonSerializer.Deserialize<Settings>(settingsJson)
                ?? throw new Exception("Не удалось прочитать settings.json");

            string uriPrefix = setting.Prefixes[0];
            server.Prefixes.Add(uriPrefix);

            server.Start();
            Console.WriteLine("Сервер запущен и слушает: " + uriPrefix);
            await Listen();
        }

        public async Task Listen()
        {
            while (true)
            {
                Console.WriteLine("Введите 'stop', чтобы остановить выполнение");
                Task<string?> consoleTask = Task.Run(() => Console.ReadLine());
                Task<HttpListenerContext> requestTask = server.GetContextAsync();

                Task completedTask = await Task.WhenAny(consoleTask, requestTask);

                if (completedTask == consoleTask)
                {
                    string? command = await consoleTask;
                    if (command == "stop")
                    {
                        Stop();
                        break;
                    }
                }
                else
                {
                    HttpListenerContext context = await requestTask;
                    HttpListenerResponse response = context.Response;


                    string htmlPath = "VsDzSearch.html";
                    string htmlFileText = File.Exists(htmlPath)
                        ? File.ReadAllText(htmlPath, Encoding.UTF8)
                        : "<h1>Файл VsDzSearch.html не найден</h1>";

                    byte[] buffer = Encoding.UTF8.GetBytes(htmlFileText);

                    response.ContentType = "text/html; charset=utf-8"; 
                    response.ContentLength64 = buffer.Length;

                    using Stream output = response.OutputStream;
                    await output.WriteAsync(buffer);
                    await output.FlushAsync();
                    response.Close();

                    Console.WriteLine("Запрос обработан");
                }
            }
        }

        public void Stop()
        {
            if (server.IsListening)
                server.Stop();
            Console.WriteLine("Работа сервера остановлена");
        }
    }
}