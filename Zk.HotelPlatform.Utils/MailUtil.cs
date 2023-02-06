using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public class MailUtil
    {
        public static void SendMail(Mail mail)
        {
            MailMessage message = new MailMessage();
            //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
            message.From = new MailAddress(mail.From);
            //设置邮件标题
            message.Subject = mail.Subject;
            //设置邮件内容
            message.Body = mail.Body;
            message.IsBodyHtml = true;
            //设置收件人,可添加多个,添加方法与下面的一样
            if (!string.IsNullOrEmpty(mail.To))
            {
                mail.To.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                .ForEach(t =>
                {
                    message.To.Add(t);
                });
            }
            //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看
            SmtpClient client = new SmtpClient(mail.Smtp.SmtpHost, mail.Smtp.SmtpPort);
            //启用ssl,也就是安全发送
            client.EnableSsl = false;
            client.Credentials = new NetworkCredential()
            {
                UserName = mail.Smtp.UserName,
                Password = mail.Smtp.UserPwd
            };
            client.Send(message);
        }
    }

    public class Mail
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
        public SMTP Smtp { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SMTP
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
    }
}
