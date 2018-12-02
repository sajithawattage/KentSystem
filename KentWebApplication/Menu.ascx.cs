using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KentWebApplication
{
	public partial class Menu : System.Web.UI.UserControl
	{
		protected const string ROLE_ADMIN			= "1";
		protected const string ROLE_MANAGER			= "2";
		protected const string ROLE_ENGINEER		= "3";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				//if(User.IsInRole(ROLE_ENGINEER))
				//{
					
				//}
				//else if(User.IsInRole(ROLE_MANAGER))
				//{
					
				//}
			}
		}
	}
}