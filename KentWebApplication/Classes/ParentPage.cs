using Entities;
using System;

namespace SLII_Web.Classes
{
    public class ParentPage : System.Web.UI.Page
    {
        #region Events

        protected void Page_Init(object sender, System.EventArgs e)
        {
            if (Context.User != null && Context.User.Identity.IsAuthenticated)
            {
                Account = new UserAccount(Context.User.Identity.Name);
            }
        }

        #endregion

        #region Properties

        public UserAccount Account { get; private set; } = null;

        public string UserName
        {
            get { return Context.User.Identity.Name; }
        }

        #endregion
    }
}