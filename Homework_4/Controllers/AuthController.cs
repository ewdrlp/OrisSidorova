using System;
using System.Net;
using oris0310.Framework;

namespace oris0310.Controllers
{
    [HttpController("auth")]
    public class AuthController : ControllerBase
    {
        [Get("login")]
        public void Login()
        {
            ReturnFile("Steam.html");
        }

        [Post("login")]
        public void Login(string login, string password)
        {
            Console.WriteLine();
            Console.WriteLine("---");
            Console.WriteLine("Данные пользователя");
            Console.WriteLine(" ");
            Console.WriteLine($"Логин:{login}");
            Console.WriteLine($"Пароль:{password}");
            Console.WriteLine($"Время:{DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            Console.WriteLine("---");
            Console.WriteLine();

            bool ok = login == "admin" && password == "123";

            string html = $@"<!DOCTYPE html>
<html lang='ru'><head><meta charset='utf-8'><title>Результат</title></head>
<body style='font-family:Arial;text-align:center;margin-top:80px;'>
  <h1 style='color:{(ok ? "#2e7d32" : "#c62828")}'>
    {(ok ? "Успешный вход!" : "Неверный логин или пароль")}
  </h1>
  <p>Логин: <b>{WebUtility.HtmlEncode(login)}</b></p>
  <p>Пароль: <b>{WebUtility.HtmlEncode(password)}</b></p>
  <p><a href='/login'>← Вернуться</a></p>
</body></html>";

            ReturnHtml(html, ok ? 200 : 401);
        }
    }
}
