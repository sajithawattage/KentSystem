
using System;
using System.Data.SqlClient;

namespace DAO
{
    public class LoginDAO
    {
        #region Member
        private readonly DBConnection myConn = null;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public LoginDAO()
        {
            if (myConn == null)
            {
                myConn = new DBConnection();
            }
        }

        #endregion

        #region Methods

        public string GetPasswordByUserName(string userName)
        {
            string status = string.Empty;
            string qCandidateStatus = "sp_web_GetPasswordByUserName";
            SqlParameter[] param = null;

            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@username", userName);

                status = Convert.ToString(myConn.ExecuteScalarProcedure(qCandidateStatus, param));

                return status;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
