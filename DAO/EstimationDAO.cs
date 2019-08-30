using DAO.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAO
{
    public class EstimationDAO
    {
        DBConnection myConn = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public EstimationDAO()
        {
            if (myConn == null)
            {
                myConn = new DBConnection();
            }

        }

        /// <summary>
        /// save the estimations
        /// </summary>
        public bool SaveEstimation(Estimation objEstimation, DataTable DT)
        {
            SqlTransaction trans = null;
            SqlParameter[] param_header = null;
            SqlParameter[] param_details = null;

            SqlConnection conn = myConn.OpenConnection();

            string qEstimationHeader = "Sp_Create_EstimationHeader";
            string qEstimationDetails = "Sp_Create_EstimationDetail";

            bool isSaved = false;

            try
            {
                trans = conn.BeginTransaction();

                param_header = new SqlParameter[7];

                param_header[0] = new SqlParameter("@EstimationId", objEstimation.EstimationID);
                param_header[1] = new SqlParameter("@EstimationDate", objEstimation.Applydate);
                param_header[2] = new SqlParameter("@customer_id", objEstimation.CustomerID);
                param_header[3] = new SqlParameter("@job_id", objEstimation.jobID);
                param_header[4] = new SqlParameter("@status", objEstimation.Status);
                param_header[5] = new SqlParameter("@EngineerNo", objEstimation.EngineerId);
                param_header[6] = new SqlParameter("@ManagerNo", objEstimation.ManagerId);

                myConn.ExecuteTransactionProcedure(conn, trans, qEstimationHeader, param_header);

                param_details = new SqlParameter[7];

                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    param_details[0] = new SqlParameter("@EstimationId", objEstimation.EstimationID);
                    param_details[1] = new SqlParameter("@Item_order", i + 1);
                    param_details[2] = new SqlParameter("@ItemId", DT.Rows[i]["ItemCode"].ToString());
                    param_details[3] = new SqlParameter("@EstimatedQty", Convert.ToDecimal(DT.Rows[i]["Qty"].ToString()));
                    param_details[4] = new SqlParameter("@FinalQty", Convert.ToDecimal(DT.Rows[i]["Qty"].ToString()));
                    param_details[5] = new SqlParameter("@EntryDate", DateTime.Now);
                    param_details[6] = new SqlParameter("@Remarks", DT.Rows[i]["Remarks"].ToString());

                    myConn.ExecuteTransactionProcedure(conn, trans, qEstimationDetails, param_details);
                }

                trans.Commit();

                isSaved = true;
                return isSaved;
            }
            catch (Exception ex)
            {
                if (trans != null)
                {
                    trans.Rollback();
                    conn.Close();
                }
                throw ex;
            }
            finally
            {
                trans.Dispose();
                conn.Close();
            }
        }

        /// <summary>
        /// update the estimates
        /// </summary>
        public bool UpdateEstimation(Estimation objEstimation, DataTable DT)
        {
            SqlTransaction trans = null;
            SqlParameter[] param_header = null;
            SqlParameter[] param_deletes = null;
            SqlParameter[] param_details = null;

            SqlConnection conn = myConn.OpenConnection();

            string qEstimationHeader = "sp_web_UpdateEstimationHeader";
            string qDeleteEstimationdetails = "sp_web_DeleteEstimationDetail";
            string qEstimationDetails = "Sp_Create_EstimationDetail";

            bool isSaved = false;

            try
            {
                trans = conn.BeginTransaction();

                param_header = new SqlParameter[6];

                param_header[0] = new SqlParameter("@EstimationId", objEstimation.EstimationID);
                param_header[1] = new SqlParameter("@customer_id", objEstimation.CustomerID);
                param_header[2] = new SqlParameter("@job_code", objEstimation.jobID);
                param_header[3] = new SqlParameter("@status", objEstimation.Status);
                param_header[4] = new SqlParameter("@engineer_id", objEstimation.EngineerId);
                param_header[5] = new SqlParameter("@manager_id", objEstimation.ManagerId);

                myConn.ExecuteTransactionProcedure(conn, trans, qEstimationHeader, param_header);

                param_deletes = new SqlParameter[1];

                param_deletes[0] = new SqlParameter("@EstimationId", objEstimation.EstimationID);

                myConn.ExecuteTransactionProcedure(conn, trans, qDeleteEstimationdetails, param_deletes);

                param_details = new SqlParameter[7];

                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    param_details[0] = new SqlParameter("@EstimationId", objEstimation.EstimationID);
                    param_details[1] = new SqlParameter("@Item_order", i + 1);
                    param_details[2] = new SqlParameter("@ItemId", DT.Rows[i]["ItemCode"].ToString());
                    param_details[3] = new SqlParameter("@EstimatedQty", Convert.ToDecimal(DT.Rows[i]["Qty"].ToString()));
                    param_details[4] = new SqlParameter("@FinalQty", Convert.ToDecimal(DT.Rows[i]["Qty"].ToString()));
                    param_details[5] = new SqlParameter("@EntryDate", DateTime.Now);
                    param_details[6] = new SqlParameter("@Remarks", DT.Rows[i]["Remarks"].ToString());

                    myConn.ExecuteTransactionProcedure(conn, trans, qEstimationDetails, param_details);
                }

                trans.Commit();

                isSaved = true;

                return isSaved;
            }
            catch (Exception ex)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
                throw ex;
            }
            finally
            {
                trans.Dispose();
                conn.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] GetAllItems(string prefix, int type)
        {
            DataTable dtItems = null;
            string qEstimations = "Sp_GetAllItems";
            List<string> objItems = new List<string>();

            try
            {
                SqlParameter[] param = new SqlParameter[2];

                param[0] = new SqlParameter("@term", prefix);
                param[1] = new SqlParameter("@type", type);

                dtItems = myConn.ExecuteProcedure(qEstimations, param);

                foreach (DataRow dr in dtItems.Rows)
                {
                    objItems.Add(string.Format("{0}~{1}~{2}", dr["ItemCode"].ToString(),
                                                                dr["ItemDescription"].ToString(),
                                                                dr["MainMeasure"].ToString()));
                }

                return objItems.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Site Details by job and customer code
        /// </summary>
        public DataTable GetSiteDetailsByJobCustomerCode(int customerCode, int jobCode, int engineerCode)
        {
            DataTable dtSiteDetails = null;
            SqlParameter[] param = null;
            string qSiteDetails = "sp_web_GetSiteDetailsByJobIdAndCustomerId";

            try
            {
                param = new SqlParameter[3];

                param[0] = new SqlParameter("@customer_id", customerCode);
                param[1] = new SqlParameter("@job_id", jobCode);
                param[2] = new SqlParameter("@engineer_id", engineerCode);  //donald

                dtSiteDetails = myConn.ExecuteProcedure(qSiteDetails, param);
                return dtSiteDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get estimation header details
        /// </summary>
        public DataTable GetEstimationHeader(int customerCode, int jobCode, string EngNo)
        {
            DataTable dtSiteDetails = null;
            SqlParameter[] param = null;
            string qSiteDetails = "sp_web_GetEstimationHeaderByCustomerJobAndEngineer";

            try
            {
                param = new SqlParameter[3];

                param[0] = new SqlParameter("@customer_id", customerCode);
                param[1] = new SqlParameter("@job_id", jobCode);
                param[2] = new SqlParameter("@EngineerID", EngNo);           //donald

                dtSiteDetails = myConn.ExecuteProcedure(qSiteDetails, param);
                return dtSiteDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetEstimationHeaderByEngineer(int customerCode, int jobCode, string engNo)
        {
            const string qSiteDetails = "sp_web_GetEstimationHeaderByCustomerAndJob";

            try
            {
                var param = new SqlParameter[3];

                param[0] = new SqlParameter("@customer_id", customerCode);
                param[1] = new SqlParameter("@job_id", jobCode);
                param[2] = new SqlParameter("@EngineerID", engNo);           //donald

                var dtSiteDetails = myConn.ExecuteProcedure(qSiteDetails, param);
                return dtSiteDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get estimate details
        /// </summary>
        public DataTable GetEstimationDetails(int estimationId)
        {
            DataTable dtSiteDetails = null;
            SqlParameter[] param = null;
            string qSiteDetails = "sp_web_GetEstimationDetailByCustomerAndJob";

            try
            {
                param = new SqlParameter[1];

                param[0] = new SqlParameter("@estimation_id", estimationId);

                dtSiteDetails = myConn.ExecuteProcedure(qSiteDetails, param);
                return dtSiteDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetNextEstimationId()
        {
            string estimationId = string.Empty;
            string qEstimationId = "sp_web_GetNextEstimationId";

            try
            {
                estimationId = myConn.ExecuteScalarProcedure(qEstimationId).ToString();
                return estimationId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
