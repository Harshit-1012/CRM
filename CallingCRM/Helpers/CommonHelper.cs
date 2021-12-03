using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using CallingCRM.Models;
using CallingCRM.Services;

namespace CallingCRM.Helpers
{
    public class CommonHelper
    {
        //static CommonHelper _helper = new CommonHelper();
        static UserServices _userServices = new UserServices();

        public string GenerateRandomCode()
        {
            string guid = Guid.NewGuid().ToString("N");
            List<char> lst = new List<char>();
            int count = 1;

            foreach (char c in guid)
            {
                if (count == 17) break;
                if (count % 2 == 0)
                {
                    lst.Add(Guid.NewGuid().ToString().ToCharArray()[1]);
                }
                else
                {
                    lst.Add(c);
                }
                count++;
            }
            return string.Join("", lst.ToArray()).ToUpper();
        }

        public string GetRandomUsername()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, false));
            builder.Append(RandomNumber(1000, 9999));
            return builder.ToString();
        }

        // Generate a random number between two numbers  
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        // Generate a random string with a given size  
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public string RandomDigits(int size)
        {
            Random random = new Random();
            string r = "";
            int i;
            for (i = 0; i < size; i++)
            {
                r += random.Next(0, 9).ToString();
            }
            return r;
        }

        public Boolean VerifyAdmin(string currentLogin)
        {
            return currentLogin == ConfigurationManager.AppSettings["Admin"];            
        }

        public Decimal Min(Decimal num1, Decimal num2)
        {
            if (num1 < num2) 
            {
                return num1;
            }
            return num2;
        }

        public Decimal Max(Decimal num1, Decimal num2)
        {
            if (num1 > num2)
            {
                return num1;
            }
            return num2;
        }

        public string SendSMTPMail(string fromName, string fromEmail, string toEmail, string subject, string body)
        {
            var ret = "Success";
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(toEmail);
                if (ConfigurationManager.AppSettings["SMTP_AddBCC"] != null && ConfigurationManager.AppSettings["SMTP_AddBCC"] != "")
                {
                    var BccList = ConfigurationManager.AppSettings["SMTP_AddBCC"].Split(',');

                    foreach (string bcc in BccList)
                    {
                        mail.Bcc.Add(bcc);
                    }

                    //mail.Bcc.Add(ConfigurationManager.AppSettings["SMTP_AddBCC"]);
                }

                mail.ReplyToList.Add(new MailAddress(fromEmail, fromName));
                mail.From = new MailAddress(ConfigurationManager.AppSettings["SMTP_User"], fromName);
                mail.Subject = subject;
                mail.Body = HttpUtility.HtmlDecode(body);
                mail.IsBodyHtml = true;

                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTP_IsSSL"]);
                SmtpServer.Host = ConfigurationManager.AppSettings["SMTP_Host"];
                SmtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP_Port"]);

                NetworkCredential credentials = new NetworkCredential(ConfigurationManager.AppSettings["SMTP_User"], ConfigurationManager.AppSettings["SMTP_Password"]);
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = credentials;

                //System.Net.Mail.Attachment attachment;
                //attachment = new System.Net.Mail.Attachment("c:/textfile.txt");
                //mail.Attachments.Add(attachment);

                SmtpServer.Send(mail);
            }
            catch (Exception Ex)
            {
                ret = "Sender:::" + ConfigurationManager.AppSettings["SMTP_Host"] + ":" + ConfigurationManager.AppSettings["SMTP_User"] + ", " + Ex.Message + ", " + Ex.InnerException;
                //ret = "Variables::::" + fromName + ", " + fromEmail + ", " + toEmail + ", " + Ex.InnerException;
            }

            return ret;
        }

        public DateTime GetCurrentDate()
       {
            string timeZoneId = ConfigurationManager.AppSettings["TimeZoneId"];
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo myZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            DateTime thisDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, myZone);
            
            return thisDateTime;
        }

        public string GetNextInvoiceNumber()
        {

            return "This Order";
        }

        public string GetDurationNameByDays(int days)
        {
            var result = "";
            if (days == 1)
            {
                result = "Daily";
            }
            else if (days == 7)
            {
                result = "Weekly";
            }
            else if (days == 15)
            {
                result = "Fortnightly";
            }
            else if (days == 30)
            {
                result = "Monthly";
            }
            else if (days == 90)
            {
                result = "Quaterly";
            }
            else if (days == 180)
            {
                result = "Half Yearly";
            }
            else if (days == 365)
            {
                result = "Yearly";
            }
            return result;
        }

        public string ValidateAdmin()
        {
            string ClientId = ConfigurationManager.AppSettings["ClientId"];
            var result = "";

            if (ClientId == null || ClientId == "")
            {
                result = "Client Id is not declared in project.";
            }
            else
            {
                AspNetUser client = _userServices.getUserById(ClientId);

                if (client == null)
                {
                    result = "Client Id is not declared does not exist.";
                }
                else
                {
                    Boolean IsAdmin = client.AspNetRoles.Any(x => x.Name == "Admin");

                    if (!IsAdmin)
                    {
                        result = "Unauthorized admin access.";
                    }
                }
            }

            return result;
        }

    }
}