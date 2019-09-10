using System;
using System.Data;
using System.Data.SqlClient;

namespace DAO
{
    public class ProfileDAO
    {
        #region Member

        private readonly DBConnection myConn = null;

        #endregion

        /// <summary>
        /// Default Constructor for class
        /// </summary>
        public ProfileDAO()
        {
            if (myConn == null)
            {
                myConn = new DBConnection();
            }
        }

        /// <summary>
        /// Update password according to the user name
        /// </summary>
        public void UpdatePasswordByUserName(string userName, string newPassword)
        {
            string qUpdatePassword = "sp_web_UpdatePassword";
            SqlParameter[] param = null;

            try
            {
                param = new SqlParameter[2];

                param[0] = new SqlParameter("@user_name", userName);
                param[1] = new SqlParameter("@new_password", newPassword);

                myConn.ExecuteProcedure(qUpdatePassword, param);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateProfileDetails(string userName, string emailAddress, string telephone)
        {
            string qUpdatePassword = "sp_web_UpdateProfile";
            SqlParameter[] param = null;

            try
            {
                param = new SqlParameter[3];

                param[0] = new SqlParameter("@user_name", userName);
                param[1] = new SqlParameter("@telephone", telephone);
                param[2] = new SqlParameter("@emailAddress", emailAddress);

                myConn.ExecuteProcedure(qUpdatePassword, param);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetProfileDetails(string userName)
        {
            string qUpdatePassword = "sp_web_ProfileDetails";
            DataTable dtProfileDetails = null;
            SqlParameter[] param = null;

            try
            {
                param = new SqlParameter[1];

                param[0] = new SqlParameter("@user_name", userName);

                dtProfileDetails = myConn.ExecuteProcedure(qUpdatePassword, param);

                return dtProfileDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
