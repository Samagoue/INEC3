using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Linq;
using System.Web;

namespace INEC3.Helper
{
    public class Email
    {
        public static bool SendMail(string to, string subject, string msg)
        {
            try
            {
                string SenderEmailAddress = System.Configuration.ConfigurationManager.AppSettings["SenderEmailAddress"];
                string FromEmailAddress = System.Configuration.ConfigurationManager.AppSettings["FromEmailAddress"];
                string SenderEmailPassword = System.Configuration.ConfigurationManager.AppSettings["SenderEmailPassword"];
                string SenderSMTPServer = System.Configuration.ConfigurationManager.AppSettings["SenderSMTPServer"];
                int Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);
                bool IsSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsSsl"]);
                string DisplayName = System.Configuration.ConfigurationManager.AppSettings["DisplayName"];

                MailMessage message = new MailMessage();
                string[] addresses = to.Split(';');
                foreach (string address in addresses)
                {
                    message.To.Add(new MailAddress(address));
                }

                message.From = new MailAddress(FromEmailAddress, DisplayName);
                message.Subject = subject;
                message.Body = msg;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                client.Host = SenderSMTPServer;
                if (Port > 0)
                    client.Port = Port;
                client.UseDefaultCredentials = false;
                NetworkCredential nc = new NetworkCredential(FromEmailAddress, SenderEmailPassword);
                client.EnableSsl = IsSsl;
                client.Credentials = nc;
                client.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static void SendMailWithAttachment(string to, string subject, string msg, string filepath)
        {
            try
            {
                string SenderEmailAddress = System.Configuration.ConfigurationManager.AppSettings["SenderEmailAddress"];
                string SenderEmailPassword = System.Configuration.ConfigurationManager.AppSettings["SenderEmailPassword"];
                string SenderSMTPServer = System.Configuration.ConfigurationManager.AppSettings["SenderSMTPServer"];
                int Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);
                bool IsSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsSsl"]);
                string DisplayName = System.Configuration.ConfigurationManager.AppSettings["DisplayName"];
                bool IsLive = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLive"]);

                MailMessage message = new MailMessage();
                string[] addresses = to.Split(';');
                foreach (string address in addresses)
                {
                    message.To.Add(new MailAddress(address));
                }



                if (IsLive == false)
                {
                    //message.Bcc.Add("example@gmail.com");
                    //message.Bcc.Add("example@gmail.com");
                }

                message.From = new MailAddress(SenderEmailAddress, DisplayName);
                message.Subject = subject;
                message.Body = msg;
                message.IsBodyHtml = true;

                message.Attachments.Add(new Attachment(filepath));
                SmtpClient client = new SmtpClient();
                client.Host = SenderSMTPServer;
                if (Port > 0)
                    client.Port = Port;
                System.Net.NetworkCredential nc = new System.Net.NetworkCredential(SenderEmailAddress, SenderEmailPassword);
                client.EnableSsl = IsSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = nc;
                client.Send(message);
            }
            catch (Exception ex)
            {
                //Login.WriteLog(LogType.Error, ex.TargetSite.Name + " - " + ex.Message);
            }
        }
        public enum EmailTemplates
        {
            ForgotPassword = 1,
            WelcomeEmail = 2
        }
        public static string GetTemplateString(int templateCode)
        {
            StreamReader objStreamReader;
            string path = "";
            if (templateCode == (int)EmailTemplates.ForgotPassword)
            {
                path = AppDomain.CurrentDomain.BaseDirectory + @"\Templates\ForgotPassword.htm";
            }
            else if (templateCode == (int)EmailTemplates.WelcomeEmail)
            {
                path = AppDomain.CurrentDomain.BaseDirectory + @"\Templates\WelcomeEmail.htm";
            }


            if (!string.IsNullOrEmpty(path))
            {
                objStreamReader = File.OpenText(path);
                string emailText = objStreamReader.ReadToEnd();
                objStreamReader.Close();
                objStreamReader = null;
                objStreamReader = null;
                return emailText;
            }
            else
            {
                objStreamReader = null;
                return string.Empty;
            }
        }
    }
}