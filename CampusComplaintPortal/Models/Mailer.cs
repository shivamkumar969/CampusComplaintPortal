using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace CampusComplaintPortal.Models
{
    public class Mailer
    {
        string MyEmail = "shivamji101202@gmail.com";
        string MyMailPassword = "vezp ahya xxpo trgo";

        public bool SendMyEmail(string sendTo, string Subject, string Message)
        {
            try
            {
                // Step 1 : Perform setting of mesage
                MailMessage msg = new MailMessage();
                MailAddress mailFrom = new MailAddress(MyEmail);
                msg.From = mailFrom;
                msg.Subject = Subject;
                msg.To.Add(sendTo);
                msg.Body = Message;

                // Step 2: Create and set smtp protocol
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                NetworkCredential nc = new NetworkCredential(MyEmail, MyMailPassword);
                client.Credentials = nc;

                // Step 3: Send email

                client.Send(msg);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}