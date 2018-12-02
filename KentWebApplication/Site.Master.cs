using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KentWebApplication
{
	public partial class Site : System.Web.UI.MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void btnLogout_Click(object sender, EventArgs e)
		{
			FormsAuthentication.SignOut();
			Response.Redirect("Login.aspx");
		}

		protected void hlProfile_Click(object sender, EventArgs e)
		{
			Response.Redirect("Profile.aspx");
		}

		protected void hlSignOut_Click(object sender, EventArgs e)
		{
			HttpCookie dummyCookie = new HttpCookie(FormsAuthentication.FormsCookieName, String.Empty);

			// Add cookie to response, and do not cache it.
			Response.Cookies.Add(dummyCookie);
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			// Sign out of forms authentication.
			FormsAuthentication.SignOut();

			// Abandon the session
			Session.Abandon();

			// Redirect to the home page (which FormsAuthentication should pick up, and redirect to the login).
			Response.Redirect("~/Login.aspx");
		}
	}
}