using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace KentWebApplication
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{

		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
			if (HttpContext.Current.User != null &&
				HttpContext.Current.User.Identity != null)
			{
				if (HttpContext.Current.User.Identity.IsAuthenticated)
				{
					if (HttpContext.Current.User.Identity is FormsIdentity)
					{
						FormsIdentity				id		= (FormsIdentity)HttpContext.Current.User.Identity;
						FormsAuthenticationTicket	ticket	= id.Ticket;

						// Get the stored user-data, in this case, our roles
						string					userData	= ticket.UserData;
						string[]				roles		= userData.Split(',');

						HttpContext.Current.User			= new GenericPrincipal(id, roles);
					}
				}
			}
		}

		protected void Application_Error(object sender, EventArgs e) {	}

		protected void Session_End(object sender, EventArgs e)	{	}

		protected void Application_End(object sender, EventArgs e)	{	}

		#region Methods

		/// <summary>
		/// Create or Update user context
		/// </summary>
		protected void CreateOrUpdateContext()
		{
			try
			{
				HttpCookie	cookie							= Context.Request.Cookies[FormsAuthentication.FormsCookieName];

				if (cookie != null)
				{
					FormsAuthenticationTicket ticket		= FormsAuthentication.Decrypt(cookie.Value);

					if (ticket != null)
					{
						FormsAuthentication.RenewTicketIfOld(ticket);
					}
				}

				string					userName				= HttpContext.Current.User.Identity.Name;

				Entities.UserAccount	account					= new DAO.UserAccountDAO().GetUserAccountEnitityByName(userName);
				if (account != null)
				{
					// Assign the user
					HttpContext.Current.User = account;
				}


			}
			catch (Exception ex)
			{

			}
		}

		#endregion

	}
}