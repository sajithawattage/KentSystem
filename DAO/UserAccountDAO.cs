using Entities;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DAO
{
    public class UserAccountDAO
    {
        #region Member

        DBConnection myConn = null;

        #endregion

        #region Constructor

        public UserAccountDAO()
        {
            if (myConn == null)
            {
                myConn = new DBConnection();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get user account by user name
        /// </summary>
        public DataTable GetUserAccountByName(string userName)
        {
            DataTable dtUserAccount = null;
            string qUserAccount = "sp_web_GetAccountByUserName";
            SqlParameter[] param = null;

            try
            {
                param = new SqlParameter[1];

                param[0] = new SqlParameter("@username", userName);

                dtUserAccount = myConn.ExecuteProcedure(qUserAccount, param);
                return dtUserAccount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get a user account entity by user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserAccount GetUserAccountEnitityByName(string userName)
        {
            UserAccount userAccount = null;
            DataTable dtUser = this.GetUserAccountByName(userName);
            if (dtUser != null && dtUser.Rows.Count > 0)
            {
                userAccount = this.MapUserAccountentites(dtUser);
            }
            return userAccount;
        }

        public UserAccount MapUserAccountentites(DataTable dtUserAccount)
        {
            UserAccount account = null;

            if (dtUserAccount != null && dtUserAccount.Rows.Count > 0)
            {
                account = new UserAccount(dtUserAccount.Rows[0]["UserName"].ToString());

                account.UserName = dtUserAccount.Rows[0]["UserName"].ToString();
                account.DisplayName = dtUserAccount.Rows[0]["DisplayName"].ToString();
                account.Email = dtUserAccount.Rows[0]["Email"].ToString();
                account.Role = Convert.ToInt32(dtUserAccount.Rows[0]["Role"]);
                account.Status = Convert.ToInt32(dtUserAccount.Rows[0]["Status"]);
                account.EncryptedPassword = dtUserAccount.Rows[0]["Password"].ToString();
                account.EmployeeCode = dtUserAccount.Rows[0]["EmpCode"].ToString();
            }

            return account;
        }

        #endregion

        #region enum

        public enum Role
        {
            ADMIN = 1,
            MANAGER,
            ENGINEER

        }

        #endregion

    }
}
