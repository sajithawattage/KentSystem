 using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAO;
using Entities;
using SLII_Web.Classes;

namespace KentWebApplication
{
	public partial class Login : System.Web.UI.Page
	{

		#region Constant

		#endregion
		
		#region Member

		LoginDAO		login						= null;
		
		#endregion

		#region Methods

		private bool Authenticate(string username, string userPassword, UserAccount account)
		{
			bool status			= false;

			if (account != null)
			{
				if (General.GetDecryptedValue(account.EncryptedPassword, "AAECAwQFBgcICQoLDA0ODw==") == userPassword)
				{
					status		= true;
				}
			}

			return status;
		}

		private bool Validation()
		{
			return true;
		}

		private void InitPage()
		{
			txtUserName.Text = string.Empty;
			txtPassword.Text = string.Empty;
			
			dvErrorMessages.Visible = false;
		}

		private void CreateTicket(string username, UserAccount account)
		{
			FormsAuthentication.SetAuthCookie(username, false);
			
			FormsAuthenticationTicket	loginTicket = new FormsAuthenticationTicket(1,username, DateTime.Now,
																				DateTime.Now.AddMinutes(480), true, account.Role.ToString());
			HttpCookie					cookie		= new HttpCookie(FormsAuthentication.FormsCookieName,
																	FormsAuthentication.Encrypt(loginTicket));

			Response.Cookies.Add(cookie);
		}

		public DataTable UserAccountGetByUserName(string userName)
		{
			DataTable			dtUserAccount	= null;
			UserAccountDAO		userAccountDao	= new UserAccountDAO();

			if(userName != string.Empty)
			{
				dtUserAccount					= userAccountDao.GetUserAccountByName(userName);
			}

			return dtUserAccount;

		}

		#endregion

		#region Events

		protected void Page_Load(object sender, EventArgs e)
		{
			if (login == null)
			{
				login							= new LoginDAO();
			}

			if (!IsPostBack)
			{
				this.InitPage();
			}
		}

		protected void btnLogin_Click(object sender, EventArgs e)
		{
			if (Validation())
			{
				string username				= txtUserName.Text.Trim();
				string password				= txtPassword.Text.Trim();

				string statusMessage = string.Empty;

				DataTable	dtUserAccount	= this.UserAccountGetByUserName(username);
				UserAccount userAccount		= new UserAccountDAO().MapUserAccountentites(dtUserAccount);

				if (this.Authenticate(username, password, userAccount))
				{
					this.CreateTicket(username,  userAccount);

					Response.Redirect("Home.aspx", false);
					Context.ApplicationInstance.CompleteRequest();		
				}
				else
				{
					litErrorMessage.Text	= "Invalid user name and password or inactive account";
					dvErrorMessages.Visible = true;
				}
			}
			
		}

		#endregion

	}
	
}