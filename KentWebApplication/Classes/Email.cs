using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace KentWebApplication.Classes
{
	public class Email
	{
		public bool SendMail()
		{
			bool results = false;

			try
			{
				// smtp settings
				var smtp = new System.Net.Mail.SmtpClient();
				{
					smtp.Host           = HostAddress;
					smtp.Port           = 587;
					smtp.EnableSsl      = false;
					smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
					smtp.Credentials    = new NetworkCredential(FromAddress, SmtpPassword);
					smtp.Timeout        = 20000;
				}
				// Passing values to smtp object
				smtp.Send(FromAddress, ToAddress, Subject, Body);
				results = true;
			}
			catch(Exception)
			{
                throw;
			}

			return results;
		}

		public string FromAddress { get; set;}
		public string ToAddress { get; set; }
		public string SmtpPassword { get; set;}
		public string HostAddress { get; set;}
		public string Subject { get; set;}
		public string Body { get; set;}

	}
}