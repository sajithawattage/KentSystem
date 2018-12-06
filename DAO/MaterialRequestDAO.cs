using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DAO.Entities;

namespace DAO
{
    public class MaterialRequestDAO
    {
        #region Member

        DBConnection myConn = null; 

        #endregion

        #region Constructor

        public MaterialRequestDAO()
        {
            myConn = new DBConnection();
        }

        #endregion

        /// <summary>
        /// Get All Sites
        /// </summary>
        /// <returns>dtSites</returns>
        public DataTable GetAllSites()
        {
            DataTable dtSites = null;
            string qSites = "Sp_GetAllSites";

            try
            {
                dtSites = myConn.ExecuteProcedure(qSites);
                return dtSites;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		/// <summary>
		/// Get estimation items from the database
		/// </summary>
		/// <returns></returns>
		public string[] GetEstimationItems(string prefix, int estimationId)
		{
			DataTable		dtItems				= null;
            string			qEstimations		= "sp_web_GetEstimationDetails";
			string			qAllItems			= "Sp_GetAllItems";

            List<string>	objItems			= new List<string>();
           
            try
            { 
				if(estimationId != -1)
				{
					SqlParameter[] param			= new SqlParameter[2];
 
					param[0]						= new SqlParameter("@term", prefix);
					param[1]						= new SqlParameter("@estimation_id", estimationId);
                   

					dtItems = myConn.ExecuteProcedure(qEstimations, param);
				}
				else
				{
					SqlParameter[] param			= new SqlParameter[2];
 
					param[0]						= new SqlParameter("@term", prefix);
					param[1]						= new SqlParameter("@type", 1);

					dtItems                         = myConn.ExecuteProcedure(qAllItems, param);

				}
				

                foreach (DataRow dr in dtItems.Rows)
                {
					objItems.Add(string.Format("{0}~{1}~{2}~{3}~{4}~{5}~{6}", dr["ItemCode"].ToString(), 
																	dr["ItemDescription"].ToString(),
																	dr["MainMeasure"].ToString(),
																	dr["Qty"].ToString(),											  
																	dr["FinalQty"].ToString(),
																	dr["IssuedQty"].ToString(),
                                                                    dr["RequestedQty"].ToString()));
                }

                return objItems.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
			}
            
		}

		public string GetEstimationItemById(string itemCode)
		{
			DataTable		dtItems				= null;
            string			qEstimations		= "sp_web_GetItemDetailsByCode";
			
            string	objItem			= string.Empty;
           
            try
            { 

					SqlParameter[] param			= new SqlParameter[1];
 
					param[0]						= new SqlParameter("@term", itemCode);
					
					dtItems = myConn.ExecuteProcedure(qEstimations, param);
				

                foreach (DataRow dr in dtItems.Rows)
                {
					objItem = string.Format("{0}~{1}~{2}~{3}", dr["ItemCode"].ToString(), 
																  dr["ItemDescription"].ToString(),
																  dr["MainMeasure"].ToString(),
																  dr["ReqestedQty"].ToString());
                }

                return objItem;
            }
            catch (Exception ex)
            {
                throw ex;
			}
		}

        /// <summary>
        /// Get Estimations By SiteID
        /// </summary>
        /// <returns>dtSites</returns>
        public DataTable GetEstimationsBySiteID(int SiteID)
        {
            DataTable dtEstimations = null;
            SqlParameter[] param = null;
            string qEstimations = "Sp_GetEstimationsBySiteID";

            try
            {
                param = new SqlParameter[1];

                param[0] = new SqlParameter("@iSiteID", SiteID);
                dtEstimations = myConn.ExecuteProcedure(qEstimations, param);
                return dtEstimations;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		/// <summary>
		/// Get next material request ID
		/// </summary>
		public string GetNextMaterailRequestId()
		{
            string materialRequestId		= string.Empty;
			string qMaterialRequestId		= "sp_web_GetNextMaterailRequestId";

            try
            {
                materialRequestId			= myConn.ExecuteScalarProcedure(qMaterialRequestId).ToString();
                return materialRequestId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
		}

        /// <summary>
        /// Get Estimate Details By SiteID And EstimationID
        /// </summary>
        /// <param name="iSiteID"></param>
        /// <param name="iEstimationID"></param>
        /// <returns></returns>
        public DataTable GetEstimateDetailsBySiteIDAndEstimationID(int iSiteID, int iEstimationID)
        {
            DataTable		dtEstimationDetails	= null;
            SqlParameter[]	param				= null;

            string			qEstimationDetails	= "Sp_GetEstimateDetailsBySiteIDAndEstimationID";

            try
            {
                param = new SqlParameter[2];

                param[0]						= new SqlParameter("@iSiteID", iSiteID);
                param[1]						= new SqlParameter("@iEstimationID", iEstimationID);

                dtEstimationDetails				= myConn.ExecuteProcedure(qEstimationDetails, param);
                return dtEstimationDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		/// <summary>
		/// Get material request list
		/// </summary>
		public DataTable GetMaterialRequestList(int customerId, int jobId, int engineerId)
		{
			DataTable		dtEstimations		= null;
            SqlParameter[]	param				= null;

            string			qMaterialRequest	= "sp_web_GetMaterialRequestList";

            try
            {
                param							= new SqlParameter[3];

                param[0]						= new SqlParameter("@customer_id", customerId);
				param[1]						= new SqlParameter("@job_id", jobId);
				param[2]						= new SqlParameter("@engineer_id", engineerId);

                dtEstimations					= myConn.ExecuteProcedure(qMaterialRequest, param);
                return dtEstimations;
            }
            catch (Exception ex)
            {
                throw ex;
            }
		}

		/// <summary>
		/// 
		/// </summary>
        public DataTable GetMaterialRequestByID(int customerId, int jobId, string EngNo)
		{
			DataTable		dtEstimations		= null;
            SqlParameter[]	param				= null;

            string			qMaterialRequest	= "sp_web_GetMaterialRequestByID";

            try
            {
                param							= new SqlParameter[3];

                param[0]						= new SqlParameter("@customer_id", customerId);
				param[1]						= new SqlParameter("@job_id", jobId);
                param[2]                        = new SqlParameter("@EngineerID", EngNo);           //donald

                dtEstimations					= myConn.ExecuteProcedure(qMaterialRequest, param);
                return dtEstimations;
            }
            catch (Exception ex)
            {
                throw ex;
            }
		}

		public DataTable GetMaterialRequestByMRNumber(int mrNumber)
		{
		    const string qMaterialRequest = "sp_web_GetMaterialRequestByMRNumber";

		    try
            {
                var	param				= new SqlParameter[1];

                param[0]						= new SqlParameter("@MRNumber", mrNumber);

                var		dtEstimations		= myConn.ExecuteProcedure(qMaterialRequest, param);
                return dtEstimations;
            }
            catch (Exception ex)
            {
                throw ex;
            }
		}

        /// <summary>
		/// get item list of the material request
		/// </summary>
		/// <returns></returns>
		public DataTable GetMaterialRequestItemByMaterialRequestId(int materialRequestId)
		{
			DataTable dtSiteDetails			= null;
            SqlParameter[] param			= null;
            string qSiteDetails				= "sp_web_GetMaterialRequestItemList";

            try
            {
                param = new SqlParameter[1];

                param[0] = new SqlParameter("@material_request_id", materialRequestId);

                dtSiteDetails				= myConn.ExecuteProcedure(qSiteDetails, param);
                return dtSiteDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
		}

		/// <summary>
		/// Save material request details
		/// </summary>
		public bool SaveMaterialRequest(MaterialRequest objMaterialRequest, DataTable DT)
		{
			SqlTransaction	trans			= null;
			SqlParameter[]	param_header	= null;
			SqlParameter[]	param_details	= null;

			SqlConnection	conn			= myConn.OpenConnection();

			string			qMrHeader		= "sp_MaterialRequestHeader";
			string			qMrDetails		= "Sp_MaterialRequestDetail";

			bool			status			= false;

			try
			{
				trans						= conn.BeginTransaction();

                param_header				= new SqlParameter[14];

				param_header[0]				= new SqlParameter("@MRNumber",			objMaterialRequest.MrNumber);
                param_header[1]				= new SqlParameter("@MRBookNumber",		objMaterialRequest.MrBookNumber);
                param_header[2]				= new SqlParameter("@CustomerCode",		objMaterialRequest.CustomerCode);
				param_header[3]				= new SqlParameter("@JobCode",			objMaterialRequest.JobCode); 
                param_header[4]				= new SqlParameter("@LocationOfDeliver",objMaterialRequest.LocationOfDelivery);
				param_header[5]				= new SqlParameter("@RequiredDate",		objMaterialRequest.RequiredDate);
				param_header[6]				= new SqlParameter("@ReceivedDate",		objMaterialRequest.ReceivedDate);
				param_header[7]				= new SqlParameter("@Remarks",			objMaterialRequest.Remarks);
				param_header[8]				= new SqlParameter("@MRNumberUpdateTo",	objMaterialRequest.MrNumberUpdateTo);
				param_header[9]				= new SqlParameter("@ManagerStatus",	objMaterialRequest.ManagerStatus);
				param_header[10]			= new SqlParameter("@EngineerStatus",	objMaterialRequest.EngineerStatus);
				param_header[11]			= new SqlParameter("@ID",				objMaterialRequest.Id);
				param_header[12]			= new SqlParameter("@EngineerNo",		objMaterialRequest.EngineerId);
				param_header[13]			= new SqlParameter("@ManagerNo",		objMaterialRequest.ManagerId);

				myConn.ExecuteTransactionProcedure(conn, trans, qMrHeader, param_header);

				param_details				= new SqlParameter[6];

				for (int i = 0; i< DT.Rows.Count; i++)
                {
					param_details[0]		= new SqlParameter("@MRNumber",		objMaterialRequest.MrNumber);
                    param_details[1]		= new SqlParameter("@ItemCode",		DT.Rows[i]["ItemCode"].ToString());
                    param_details[2]		= new SqlParameter("@QTY",			Convert.ToDecimal(DT.Rows[i]["Qty"].ToString()));
                    param_details[3]		= new SqlParameter("@OrderNo",		i+1);
                    param_details[4]		= new SqlParameter("@QTYOrderd",	Convert.ToDecimal(DT.Rows[i]["Qty"].ToString()));
					param_details[5]		= new SqlParameter("@Remarks",		DT.Rows[i]["Remarks"].ToString());
                  
                    myConn.ExecuteTransactionProcedure(conn, trans, qMrDetails, param_details);
				}

				trans.Commit();

				status						= true;
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

			return status;
		}

		/// <summary>
		/// Update material request details
		/// </summary>
		public bool UpdateMaterialRequest(MaterialRequest objMaterialRequest, DataTable DT)
		{
			SqlTransaction	trans				= null;
			SqlParameter[]	param_header		= null;
			SqlParameter[]	param_detail_remove = null;
			SqlParameter[]	param_details		= null;

			SqlConnection	conn			= myConn.OpenConnection();

			string			qMrHeader		= "sp_web_UpdateMaterialRequestHeader";
			string			qDeleteMrDetails= "sp_web_DeletematerialRequestDetails";
			string			qMrDetails		= "Sp_MaterialRequestDetail";

			bool			status			= false;

			try
			{
				trans						= conn.BeginTransaction();

                param_header				= new SqlParameter[14];

				param_header[0]				= new SqlParameter("@MRNumber",			objMaterialRequest.MrNumber);
                param_header[1]				= new SqlParameter("@MRBookNumber",		objMaterialRequest.MrBookNumber);
                param_header[2]				= new SqlParameter("@CustomerCode",		objMaterialRequest.CustomerCode);
				param_header[3]				= new SqlParameter("@JobCode",			objMaterialRequest.JobCode); 
                param_header[4]				= new SqlParameter("@LocationOfDeliver",objMaterialRequest.LocationOfDelivery);
				param_header[5]				= new SqlParameter("@RequiredDate",		objMaterialRequest.RequiredDate);
				param_header[6]				= new SqlParameter("@ReceivedDate",		objMaterialRequest.ReceivedDate);
				param_header[7]				= new SqlParameter("@Remarks",			objMaterialRequest.Remarks);
				param_header[8]				= new SqlParameter("@MRNumberUpdateTo",	objMaterialRequest.MrNumberUpdateTo);
				param_header[9]				= new SqlParameter("@ManagerStatus",	objMaterialRequest.ManagerStatus);
				param_header[10]			= new SqlParameter("@EngineerStatus",	objMaterialRequest.EngineerStatus);
				param_header[11]			= new SqlParameter("@ID",				objMaterialRequest.Id);
				param_header[12]			= new SqlParameter("@EngineerNo",		objMaterialRequest.EngineerId);
				param_header[13]			= new SqlParameter("@ManagerNo",		objMaterialRequest.ManagerId);

				myConn.ExecuteTransactionProcedure(conn, trans, qMrHeader, param_header);

				//
				param_detail_remove			= new SqlParameter[1];
				
				param_detail_remove[0]		= new SqlParameter("@mr_number",			objMaterialRequest.MrNumber);

				myConn.ExecuteTransactionProcedure(conn, trans, qDeleteMrDetails, param_detail_remove);
				
				param_details				= new SqlParameter[6];

				for (int i = 0; i< DT.Rows.Count; i++)
                {
					param_details[0]		= new SqlParameter("@MRNumber",		objMaterialRequest.MrNumber);
                    param_details[1]		= new SqlParameter("@ItemCode",		DT.Rows[i]["ItemCode"].ToString());
                    param_details[2]		= new SqlParameter("@QTY",			Convert.ToDecimal(DT.Rows[i]["Qty"].ToString()));
                    param_details[3]		= new SqlParameter("@OrderNo",		i + 1);
                    param_details[4]		= new SqlParameter("@QTYOrderd",	Convert.ToDecimal(DT.Rows[i]["Qty"].ToString()));
					param_details[5]		= new SqlParameter("@Remarks",		DT.Rows[i]["Remarks"].ToString());
                  
                    myConn.ExecuteTransactionProcedure(conn, trans, qMrDetails, param_details);
				}

				trans.Commit();

				status						= true;
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

			return status;
		}

		/// <summary>
		/// 
		/// </summary>
		public DataTable CheckProductExistance(string itemCode)
		{
			DataTable		dtItemDetails	= null;
            SqlParameter[]	param			= null;
            string			qItemDetails	= "sp_web_CheckItemExistance";

            try
            {
                param = new SqlParameter[1];

                param[0] = new SqlParameter("@item_code", itemCode);

                dtItemDetails				= myConn.ExecuteProcedure(qItemDetails, param);
                return dtItemDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
		}

        public DataTable GetCustomerJobDetails(int CustomerCode, int JobCode)
        {
            DataTable dtJobDetails = null;
            SqlParameter[] param = null;
            string qJobDetails = "sp_web_GetCustomerJobDetails";

            try
            {
                param = new SqlParameter[2];

                param[0] = new SqlParameter("@CustomerCode", CustomerCode);
                param[1] = new SqlParameter("@JobCode", JobCode);

                dtJobDetails = myConn.ExecuteProcedure(qJobDetails, param);
                return dtJobDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
