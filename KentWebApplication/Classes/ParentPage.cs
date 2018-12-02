using System;
using System.Collections.Generic;
using System.Web;
using Entities;

namespace SLII_Web.Classes
{
	public class ParentPage  : System.Web.UI.Page
	{

		#region Members

		private UserAccount account		= null;

		#endregion

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		private void GetAccountFromSession()
		{
			
		}
		
		#endregion

		#region Events

		/// <summary>
		/// 
		/// </summary>
		protected void Page_Init(object sender, System.EventArgs e)
		{
			if (this.Context.User != null && this.Context.User.Identity.IsAuthenticated)
			{
				// Get account
				account					= new UserAccount(this.Context.User.Identity.Name);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		override protected void OnPreInit(EventArgs e)
		{

		}

		#endregion

		#region Properties

		public UserAccount Account
		{
			get { return this.account; }
		}

		public string UserName
		{
			get { return this.Context.User.Identity.Name; }
		}
		
		#endregion

	}
}