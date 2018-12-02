using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAO;
using KentWebApplication.Classes;

namespace KentWebApplication
{
	public partial class EmailTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void Unnamed_Click(object sender, EventArgs e)
		{
			try
			{
				Email		email					= new Email(){	HostAddress  = Config.SmtpHostName,
																	FromAddress	 = Config.SmtpUserName,
																	SmtpPassword = Config.SmtpPassword,
																	ToAddress = "sajitha.wattage@gmail.com",
																	Subject = "New  MR Received ",
																	Body = "Test Email" };

				if (email.SendMail())
				{
				
				}
			}
			catch (Exception ex)
			{
				
				throw ex;
			}
			
		}
	}
}