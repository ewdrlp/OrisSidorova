using oris0310.Framework;
using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Mail;
//using System.Text;
//using System.Threading.Tasks;

//namespace oris0310 
//{
//    public class SteamController : ControllerBase
//    {
//        [Get ("send")]
//        public void Send( string to, string hero)
//        {
//            // отправитель - устанавливаем адрес и отображаемое в письме имя
//            MailAddress from = new MailAddress("eevvkkaa07@mail.ru", "Eva");
//            // кому отправляем
//            MailAddress toAdress = new MailAddress("vip.koroleva2007@mail.ru");
//            // создаем объект сообщения
//            MailMessage m = new MailMessage(from, toAdress);
//            // тема письма
//            m.Subject = "Тест";
//            // текст письма
//            m.Body = "<h2>Письмо-тест работы smtp-клиента</h2>";
//            // письмо представляет код html
//            m.IsBodyHtml = true;
//            // адрес smtp-сервера и порт, с которого будем отправлять письмо
//            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
//            // логин и пароль
//            smtp.Credentials = new NetworkCredential("somemail@gmail.com", "mypassword");
//            smtp.EnableSsl = true;
//            smtp.Send(m);
//            Console.Read();
//        }
//    }
//}
