using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace KentWebApplication
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null &&
                HttpContext.Current.User.Identity != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                        FormsAuthenticationTicket ticket = id.Ticket;

                        // Get the stored user-data, in this case, our roles
                        string userData = ticket.UserData;
                        string[] roles = userData.Split(',');

                        HttpContext.Current.User = new GenericPrincipal(id, roles);
                    }
                }
            }
        }

        #region Methods

        /// <summary>
        /// Create or Update user context
        /// </summary>
        protected void CreateOrUpdateContext()
        {
            try
            {
                HttpCookie cookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];

                if (cookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

                    if (ticket != null)
                    {
                        FormsAuthentication.RenewTicketIfOld(ticket);
                    }
                }

                string userName = HttpContext.Current.User.Identity.Name;

                Entities.UserAccount account = new DAO.UserAccountDAO().GetUserAccountEnitityByName(userName);
                if (account != null)
                {
                    HttpContext.Current.User = account;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}