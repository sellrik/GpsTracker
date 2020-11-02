using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GpsTracker
{
    public class EmailClient
    {
        public void SendMail(int port, string host, string username, string password, string from, string to, string subject, string body, Attachment attachment)
        {
            using (var client = new SmtpClient()) // TODO: obsolete
            using (var message = new MailMessage())
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential
                {
                    UserName = username,
                    Password = password
                };
                client.Port = port;
                client.Host = host;
                client.EnableSsl = true;

                message.From = new MailAddress(from);
                message.To.Add(new MailAddress(to));
                message.Subject = subject;
                message.Body = body;

                message.Attachments.Add(attachment);

                client.Send(message);
            }
        }
    }
}