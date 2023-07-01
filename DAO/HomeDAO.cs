using System;
using System.Data;
using System.Data.SqlClient;

namespace DAO
{
    public class HomeDAO
    {
        #region Member

        private readonly DBConnection myConn = null;

        #endregion

        #region Constructor

        public HomeDAO()
        {
            if (myConn == null)
            {
                myConn = new DBConnection();
            }
        }

        #endregion

        #region Curd Operation

        /// <summary>
        /// Get site details by engineer employee code
        /// </summary>
        public DataTable GetSitesByEngineer(string empCode)
        {
            DataTable dtSites = null;
            SqlParameter[] param = null;
            string qCandidateStatus = "sp_web_GetSitesByEngineerId";

            try
            {
                param = new SqlParameter[1];

                param[0] = new SqlParameter("@emp_code", empCode);

                dtSites = myConn.ExecuteProcedure(qCandidateStatus, param);

                return dtSites;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetSitesByManager(string empCode)
        {
            DataTable dtSites = null;
            SqlParameter[] param = null;
            string qCandidateStatus = "sp_web_GetSitesByManagerId";

            try
            {
                param = new SqlParameter[1];

                param[0] = new SqlParameter("@emp_code", empCode);

                dtSites = myConn.ExecuteProcedure(qCandidateStatus, param);

                return dtSites;

            }
            catch (Exception )
            {
                throw ;
            }
        }

        public string GetEmployeeCodeByUserName(string userName)
        {
            string status = string.Empty;
            string qCandidateStatus = "sp_web_GetEmployeeCodeByUserName";
            SqlParameter[] param = null;

            try
            {
                param = new SqlParameter[1];

                param[0] = new SqlParameter("@user_name", userName);

                status = Convert.ToString(myConn.ExecuteScalarProcedure(qCandidateStatus, param));

                return status;

            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Get site details by Customer Id and Job Id
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public DataTable GetSiteByCustomerAndJob(int customerId, int jobId, int engineerCode)
        {
            DataTable dtSites = null;
            SqlParameter[] param = null;
            string qCandidateStatus = "sp_web_SiteByCustomerAndJob";

            try
            {
                param = new SqlParameter[3];

                param[0] = new SqlParameter("@customer_id", customerId);
                param[1] = new SqlParameter("@job_id", jobId);
                param[2] = new SqlParameter("@engineer_id", engineerCode);

                dtSites = myConn.ExecuteProcedure(qCandidateStatus, param);

                return dtSites;

            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Get user statistics according to the user name.
        /// </summary>
        public DataTable GetUserStatistics(string userName)
        {
            DataTable dtInfomation = null;
            SqlParameter[] param = null;
            string qCandidateStatus = "sp_web_GetUserDetails";

            try
            {
                param = new SqlParameter[1];

                param[0] = new SqlParameter("@user_name", userName);

                dtInfomation = myConn.ExecuteProcedure(qCandidateStatus, param);

                return dtInfomation;

            }
            catch (Exception )
            {
                throw ;
            }
        }

        public int GetSEstimateCount(int customerId, int jobId)
        {
            string qGetSEstimateCount = "kentuser.sp_GetSEstimateCount";
            SqlParameter[] param = null;
            try
            {
                param = new SqlParameter[2];

                param[0] = new SqlParameter("@CustomerCode", customerId);
                param[1] = new SqlParameter("@JobCode", jobId);

                return Convert.ToInt32(myConn.ExecuteScalarProcedure(qGetSEstimateCount, param));
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }

}
