using HomeCalc.Core.LogService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Services
{
    public class EmailSender
    {
        public static void SendLogFile()
        {
            Task.Factory.StartNew(() => 
            {
                var status = StatusService.GetInstance();
                var logger = new Logger("EmailSender");

                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HomeCalc", "HomeCalc.View.log");
                if (!File.Exists(logPath))
                {
                    status.Post("Помилка! Інформація для відправлення не знайдена.");
                    return;
                }

                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("forestglider@gmail.com", "qnagsgimnrihnwrr");

                MailMessage mm = new MailMessage("forestglider@gmail.com", "forestglider@gmail.com", "HomeCalc log report", "Something going wrong...");
                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                ContentType contentType = new ContentType();
                contentType.MediaType = MediaTypeNames.Text.Plain;
                contentType.Name = "HomeCalc.View.log";

                mm.Attachments.Add(new Attachment(logPath, contentType));

                try
                {
                    client.Send(mm);
                    status.Post("Інформацію відправлено!");
                }
                catch (SmtpException ex)
                {
                    logger.Error(ex.Message);
                    logger.Error(ex.StatusCode.ToString());
                    status.Post("Помилка при з'єднанні з сервером електронної пошти.");
                }
            });
        }
    }
}
