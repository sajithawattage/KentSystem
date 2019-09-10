using DAO;
using KentWebApplication.Classes;
using System;

namespace KentWebApplication
{
    public partial class EmailTest : System.Web.UI.Page
    {
        protected void Unnamed_Click(object sender, EventArgs e)
        {
            Email email = new Email()
            {
                HostAddress = Config.SmtpHostName,
                FromAddress = Config.SmtpUserName,
                SmtpPassword = Config.SmtpPassword,
                ToAddress = "sajitha.wattage@gmail.com",
                Subject = "New  MR Received ",
                Body = "Test Email"
            };

            email.SendMail();
        }

    }
}