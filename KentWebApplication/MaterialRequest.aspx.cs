using DAO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SLII_Web.Classes;
using Entity = DAO.Entities;
using System.Web.Script.Services;
using KentWebApplication.Classes;
using System.Globalization;
using System.Text;
using System.Web.Services;

namespace KentWebApplication
{
    public partial class MaterialRequest : ParentPage
	{
		#region Constant

        private const string QUERY_CUSTOMER_CODE		= "cid";
		protected const string QUERY_JOB_CODE			= "jid";
		protected const string QUERY_TYPE_CODE			= "tid";
		protected const string QUERY_ENGINEER_CODE		= "eid";


		protected const string SESSION_ITEM_TABLE		= "ITEM_TABLE";
        protected const string SESSION_ITEM_TABLE_FILTER = "ITEM_TABLE_FILTERED";
        protected const string SESSION_CUSTOMER_ID		= "CUSTOMER_ID";
		protected const string SESSION_JOB_ID			= "JOB_ID";
		protected const string SESSION_FORM_MODE		= "FORM_MODE";
		protected const string SESSION_TYPE_ID			= "TYPE";
		protected const string SESSION_ENGINEER_ID		= "ENGINEER_ID";
		protected const string SESSION_MANAGER_ID		= "MANAGER_ID";
		protected const string SESSION_MR_ID			= "MR_ID";

		protected const string SESSION_PROJECT_NAME		= "PROJECT_NAME";
		protected const string SESSION_JOB_NAME			= "JOB_NAME";
		protected const string SESSION_ENGINEER_NAME	= "ENGINEER_NAME";
		protected const string SESSION_MANAGER_NAME		= "MANAGER_NAME";
		protected const string SESSION_MANAGER_EMAIL_ADDRESS = "MANAGER_EMAIL_ADDRESS";


		protected const string ENTRY_ENGINEER			= "ENTRY";
		protected const string FINISH_ENGINEER			= "FINISH";
		protected const string CLOSED_ENGINEER			= "CLOSED";

		protected const string ENTRY_MANAGER			= "ENTRY";
		protected const string FINISH_MANAGER			= "FINISH";
		protected const string CLOSED_MANAGER			= "CLOSED";

		protected const string COLUMN_ITEM_CODE			= "ItemCode";
		protected const string COLUMN_ITEM_NAME			= "ItemName";
		protected const string COLUMN_ITEM_QTY			= "Qty";
		protected const string COLUMN_ITEM_AMOUNT		= "Amount";
		protected const string COLUMN_ITEM_REMARKS		= "Remarks";
		protected const string COLUMN_ITEM_TOTAL		= "Total";
		protected const string COLUMN_ITEM_ORDER		= "OrderNo";
		protected const string COLUMN_ITEM_UOM			= "MainMeasure";

		protected const string COMMAND_REMOVE			= "Remove";
		protected const string COMMAND_EDIT				= "Change";

		#endregion

		#region Member

		private MaterialRequestDAO	materialRequestDao	=	null;
		private EstimationDAO		estimationDao		=	null;
		private DataTable			dtItems				=	null;

		private	int					type				=	-1;
        private static int          customerCode        =   -1;
        private static int          jobCode             =   -1;
		private int					engineerCode		=	-1;
		private int					managerCode			=	-1;
		private int					materialRequestId	=	-1;
		private int					id					=	-1;

        #endregion
		
		#region Enum
		public enum Mode
		{
			New			= 0,
			Saved		= 1,
			Submitted	= 2,
			Approved	= 3
		}

		#endregion

        #region Methods

        /// <summary>
        /// Set Message
        /// </summary>
        private void SetMessage(int type, string message)
        {
            switch (type)
            {
                case 1: // Success
                    success_alert.Visible				= true;
                    litSuccessMessage.Text				= message;
                    break;
                case 2: // Error
                    error_alert.Visible					= true;
                    litErrorMessage.Text				= message;
                    break;
                case 3:
                    duplicate_alert.Visible             = true;
                    litMessage.Text = message;
                    break;

            }
        }


        private void ResetErrors()
        {
            success_alert.Visible = false;
            error_alert.Visible = false;
            duplicate_alert.Visible = false;

            litSuccessMessage.Text = string.Empty;
            litErrorMessage.Text = string.Empty;
            litMessage.Text = string.Empty;

        }

        /// <summary>
        /// process query string values
        /// </summary>
        private void ProcessQueryString()
		{
			customerCode								= General.GetQueryStringInt(Request.QueryString[QUERY_CUSTOMER_CODE]);
			jobCode										= General.GetQueryStringInt(Request.QueryString[QUERY_JOB_CODE]);
			type										= General.GetQueryStringInt(Request.QueryString[QUERY_TYPE_CODE]);
			id											= General.GetQueryStringInt(Request.QueryString[QUERY_ENGINEER_CODE]);
		}

		/// <summary>
		/// Set the values from query string to the session
		/// </summary>
		private void SetSessionValues()
		{
			Session[SESSION_CUSTOMER_ID]				= customerCode;
			Session[SESSION_JOB_ID]						= jobCode;
			Session[SESSION_TYPE_ID]					= type;
			Session[SESSION_FORM_MODE]					= FormMode;
			Session[SESSION_ENGINEER_ID]				= engineerCode;
			Session[SESSION_MANAGER_ID]					= managerCode;
		}

        private DateTime RequiredDateSetter()
        {
            return GetSriLankanTime().AddDays(22);
        }

		/// <summary>
		/// load estimation details
		/// </summary>
        private void LoadEstimationDetails(int customerCode, int jobCode)
        {
            try
            {
                if (estimationDao == null)
                {
                    estimationDao					        = new EstimationDAO();
                }

                string userName = this.Context.User.Identity.Name;

                var homeDao = new HomeDAO();

                var empCode = homeDao.GetEmployeeCodeByUserName(userName);   //donald

                var dtEstimationHeaderDetails         = estimationDao.GetEstimationHeaderByEngineer(customerCode, jobCode, empCode);
				
				if (dtEstimationHeaderDetails != null && dtEstimationHeaderDetails.Rows.Count > 0)
				{
					//get estimation details
					txtManager.Text					        = dtEstimationHeaderDetails.Rows[0]["ManagerName"].ToString();
					txtEngineer.Text				        = dtEstimationHeaderDetails.Rows[0]["EngineerName"].ToString();
					litProjectName.Text				        = dtEstimationHeaderDetails.Rows[0]["CustomerName"].ToString() + " | "+ 
														        dtEstimationHeaderDetails.Rows[0]["JobName"].ToString();
                    
                    litSubEstimateID.Text                   = "New";

					engineerCode					        = Convert.ToInt32(dtEstimationHeaderDetails.Rows[0]["EngineerNo"].ToString());
					managerCode						        = Convert.ToInt32(dtEstimationHeaderDetails.Rows[0]["ManagerNo"].ToString());
					
					txtEstimationId.Text			        = dtEstimationHeaderDetails.Rows[0]["EstimateNo"].ToString();
					hfEstimationId.Value			        = dtEstimationHeaderDetails.Rows[0]["EstimateNo"].ToString();
                    dtApplyDate.Text                        = RequiredDateSetter().ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
					
					Session[SESSION_PROJECT_NAME]           = dtEstimationHeaderDetails.Rows[0]["CustomerName"].ToString();
					Session[SESSION_JOB_NAME]		        = dtEstimationHeaderDetails.Rows[0]["JobName"].ToString();
					Session[SESSION_MANAGER_NAME]	        = dtEstimationHeaderDetails.Rows[0]["ManagerName"].ToString();
					Session[SESSION_ENGINEER_NAME]	        = dtEstimationHeaderDetails.Rows[0]["EngineerName"].ToString();
					Session[SESSION_MANAGER_EMAIL_ADDRESS]	= dtEstimationHeaderDetails.Rows[0]["ManagerEmailAddress"].ToString();
					
				}
				else
				{
					//get details from site details
					if(id != -1)
					{
						DataTable	dtSiteDetails		    = homeDao.GetSiteByCustomerAndJob(customerCode, jobCode, id);
					
						if (dtSiteDetails != null && dtSiteDetails.Rows.Count > 0)
						{
							txtManager.Text				    = dtSiteDetails.Rows[0]["ManagerName"].ToString();
							txtEngineer.Text			    = dtSiteDetails.Rows[0]["EngineerName"].ToString();

							engineerCode				    = Convert.ToInt32(dtSiteDetails.Rows[0]["EngineerNo"].ToString());
							managerCode					    = Convert.ToInt32(dtSiteDetails.Rows[0]["ManagerNo"].ToString());

							litProjectName.Text			    = string.Format("{0} | {1}", dtSiteDetails.Rows[0]["CustomerName"].ToString(), dtSiteDetails.Rows[0]["JobName"].ToString());

                            dtApplyDate.Text                = RequiredDateSetter().ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

							Session[SESSION_PROJECT_NAME]   = dtSiteDetails.Rows[0]["CustomerName"].ToString();
							Session[SESSION_JOB_NAME]		= dtSiteDetails.Rows[0]["JobName"].ToString();
							Session[SESSION_MANAGER_NAME]	= dtSiteDetails.Rows[0]["ManagerName"].ToString();
							Session[SESSION_ENGINEER_NAME]	= dtSiteDetails.Rows[0]["EngineerName"].ToString();
							Session[SESSION_MANAGER_EMAIL_ADDRESS]	= dtSiteDetails.Rows[0]["ManagerEmailAddress"].ToString();
						}
					}
					
				}

				this.LoadDataGridItems();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

		/// <summary>
		/// Load data grid items from the list
		/// </summary>
		private void LoadDataGridItems()
		{
		    if (Session[SESSION_ITEM_TABLE] == null) return;

		    grdItems.DataSource					= (DataTable)Session[SESSION_ITEM_TABLE];
		    grdItems.DataBind();
		}

		/// <summary>
		/// load data to the form
		/// </summary>
		private void LoadData()
		{
		    if (type == -1) return;

		    switch(type)
		    {
		        case 0: //load new material request form
		            FormMode						= Mode.New;

		            //reset the session MR ID
		            Session[SESSION_MR_ID]			= null;

		            //load data from estimation
		            this.LoadEstimationDetails(customerCode, jobCode);

		            break;
		        case 1: //load previously saved material request form
		            FormMode						= Mode.Saved;

		            //load material request header details
		            this.GetMaterialRequestById(customerCode, jobCode);

		            //load material request item list
		            this.GetMaterialRequestDetails();
						
		            break;
		        case 2:

		            //load material request header details
		            this.GetMaterialRequestByMrNumber(id);

		            //load material request item list
		            this.GetMaterialRequestDetails();
						
		            this.DisableForm();

		            break;
		    }
		}

        /// <summary>
		/// get material request header details
		/// </summary>
		private void GetMaterialRequestById(int customerId, int jobId)


		{
			if(materialRequestDao == null)
			{
				materialRequestDao						= new MaterialRequestDAO();
			}
            	
            HomeDAO homeDao				            = new HomeDAO();

		    string empCode		= homeDao.GetEmployeeCodeByUserName(this.Context.User.Identity.Name);   //donald
              
            DataTable dtMaterialRequest = materialRequestDao.GetMaterialRequestByID( customerId, jobId, empCode);  //donald

			if (dtMaterialRequest != null && dtMaterialRequest.Rows.Count > 0)
			{
				SetUi(dtMaterialRequest);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mrNumber"></param>
		private void GetMaterialRequestByMrNumber(int mrNumber)
		{
			if(materialRequestDao == null)
			{
				materialRequestDao						= new MaterialRequestDAO();
			}

			DataTable dtMaterialRequest					= materialRequestDao.GetMaterialRequestByMRNumber(mrNumber);
			if (dtMaterialRequest != null && dtMaterialRequest.Rows.Count > 0)
			{
				SetUi(dtMaterialRequest);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void SetUi(DataTable dtMaterialRequest)
		{
			// Header details
			materialRequestId						= Convert.ToInt32(dtMaterialRequest.Rows[0]["MRNumber"].ToString());
			Session[SESSION_MR_ID]					= materialRequestId;

			//set estimation id
			hfEstimationId.Value					= dtMaterialRequest.Rows[0]["EstimateNo"].ToString();
            litSubEstimateID.Text                   = dtMaterialRequest.Rows[0]["MRNumber"].ToString();

			txtEngineer.Text						= dtMaterialRequest.Rows[0]["EngineerName"].ToString();
			txtManager.Text							= dtMaterialRequest.Rows[0]["ManagerName"].ToString();
            dtApplyDate.Text = Convert.ToDateTime(dtMaterialRequest.Rows[0]["RequiredDate"].ToString()).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            dtReceivedDate.Text						= Convert.ToDateTime(dtMaterialRequest.Rows[0]["ReceivedDate"].ToString()).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
			txtComments.Text						= dtMaterialRequest.Rows[0]["Remarks"].ToString();
			txtDeliverLocation.Text					= dtMaterialRequest.Rows[0]["LocationOfDeliver"].ToString();
				
			litProjectName.Text						= dtMaterialRequest.Rows[0]["CustomerName"].ToString() + " | "+ 
														dtMaterialRequest.Rows[0]["JobName"].ToString();

			engineerCode							= Convert.ToInt32(dtMaterialRequest.Rows[0]["EngineerNo"].ToString());
			managerCode								= Convert.ToInt32(dtMaterialRequest.Rows[0]["ManagerNo"].ToString());

			Session[SESSION_PROJECT_NAME]			= dtMaterialRequest.Rows[0]["CustomerName"].ToString();
			Session[SESSION_JOB_NAME]				= dtMaterialRequest.Rows[0]["JobName"].ToString();
			Session[SESSION_MANAGER_NAME]			= dtMaterialRequest.Rows[0]["ManagerName"].ToString();
			Session[SESSION_ENGINEER_NAME]			= dtMaterialRequest.Rows[0]["EngineerName"].ToString();
			Session[SESSION_MANAGER_EMAIL_ADDRESS]	= dtMaterialRequest.Rows[0]["ManagerEmailAddress"].ToString();

		}

		/// <summary>
		/// get material request item list
		/// </summary>
		private void GetMaterialRequestDetails()
		{
			if(materialRequestDao == null)
			{
				materialRequestDao						= new MaterialRequestDAO();
			}

			DataTable	dtMaterialRequestItem			= materialRequestDao.GetMaterialRequestItemByMaterialRequestId(materialRequestId);
			if (dtMaterialRequestItem != null && dtMaterialRequestItem.Rows.Count > 0)
			{
				Session[SESSION_ITEM_TABLE]				= dtMaterialRequestItem;
				txtTotalItem.Text						= dtMaterialRequestItem.Rows.Count.ToString();

				// material request item details
				grdItems.DataSource						= dtMaterialRequestItem;
				grdItems.DataBind();
			}
		}

		/// <summary>
		/// save material request data
		/// </summary>
		private bool SaveMaterialRequest(string managerStatus, string engineerStatus)
		{
			bool isSaved									= false;

			try
			{
				if (this.ValidateForm())
				{
				    Entity.MaterialRequest materialRequest = new Entity.MaterialRequest
				    {
				        MrBookNumber = -1,
				        CustomerCode = Convert.ToInt32(Session[SESSION_CUSTOMER_ID].ToString()),
				        JobCode = Convert.ToInt32(Session[SESSION_JOB_ID].ToString()),
				        LocationOfDelivery = txtDeliverLocation.Text.Trim(),
				        RequiredDate = Convert.ToDateTime(dtApplyDate.Text.Trim()),
				        Remarks = txtComments.Text.Trim(),
				        MrNumberUpdateTo = 1,
				        Id = 1,
				        ManagerStatus = managerStatus,
				        EngineerStatus = engineerStatus,
				        EngineerId = Convert.ToInt32(Session[SESSION_ENGINEER_ID].ToString()),
				        ManagerId = Convert.ToInt32(Session[SESSION_MANAGER_ID].ToString())
				    };


				    //create object instant for DAO
					if (materialRequestDao == null)
					{
						materialRequestDao					= new MaterialRequestDAO(); 
					}

					//get data table
					if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
					{
						dtItems							= (DataTable)Session[SESSION_ITEM_TABLE];
					}

					if (managerStatus == ENTRY_MANAGER && engineerStatus == FINISH_ENGINEER)
					{
					    materialRequest.ReceivedDate            = this.GetSriLankanTime();

					}
					else
					{
						materialRequest.ReceivedDate			= Convert.ToDateTime("1753/01/01 12:00:00");
					}

					//check for form mode
					if (FormMode == Mode.New)
					{
						materialRequest.MrNumber			= Convert.ToInt32(GenerateMaterialRequestID());
						Session[SESSION_MR_ID]				= materialRequest.MrNumber;			
						
						this.ProcessItemList(materialRequest.MrNumber);

						isSaved								= materialRequestDao.SaveMaterialRequest(materialRequest, (DataTable)Session[SESSION_ITEM_TABLE]);
					}
					else if (FormMode == Mode.Saved)
					{
						materialRequest.MrNumber			= (int)Session[SESSION_MR_ID];

						this.ProcessItemList(materialRequest.MrNumber);

						isSaved								= materialRequestDao.UpdateMaterialRequest(materialRequest, (DataTable)Session[SESSION_ITEM_TABLE]);
					}

					//
					if (isSaved)
					{
						Session[SESSION_FORM_MODE]			= Mode.Saved;

                        Email email = null;

                        //get HD Code
                        DataTable dtJobDetails = materialRequestDao.GetCustomerJobDetails(materialRequest.CustomerCode,
                                                                    materialRequest.JobCode);
                        if (dtJobDetails != null && dtJobDetails.Rows.Count > 0)
                        {
                            if (Convert.ToInt16(dtJobDetails.Rows[0]["HDCode"].ToString()) == 0)
                            {
                                //send email
                                email = new Email()
                                {
                                    HostAddress = Config.SmtpHostName,
                                    FromAddress = Config.SmtpUserName,
                                    SmtpPassword = Config.SmtpPassword,
                                    ToAddress = Session[SESSION_MANAGER_EMAIL_ADDRESS].ToString(),
                                    Subject = "New  MR Received ",
                                    Body = GenerateEmailBody(
                                            Session[SESSION_PROJECT_NAME].ToString(),
                                            Session[SESSION_JOB_NAME].ToString(),
                                            Session[SESSION_ENGINEER_NAME].ToString(),
                                            materialRequest.ReceivedDate,
                                            materialRequest.MrNumber.ToString())

                                };
                            }
                            else
                            {
                                email = SendOtherEmail(materialRequest.ReceivedDate, 
                                                 materialRequest.MrNumber.ToString(),
                                                 dtJobDetails.Rows[0]["Email"].ToString());
                            }
                        }
                        
                        if (engineerStatus == FINISH_ENGINEER)
                        {
                            if (email.SendMail())
                            {
                                this.SetMessage(1, "Material Request Successfully saved.");
                            }
                            else
                            {
                                this.SetMessage(1, "Material Request Successfully Saved.But email sending failed.");
                            }

                        }
                        else
                        {
                            this.SetMessage(1, "Material Request Successfully saved.");
                        }

					}
					else
					{
						this.SetMessage(2, "Error occurred while saving the Material Request.");
					}
				}
			}
			catch (Exception)
			{
			    // ignored
			}

		    return isSaved;

		}


        public Email SendOtherEmail(DateTime reciveDate, string requestNo, string toAddress)
        {
            Email email = new Email()
            {
                HostAddress = Config.SmtpHostName,
                FromAddress = Config.SmtpUserName,
                SmtpPassword = Config.SmtpPassword,
                ToAddress = toAddress,
                Subject = "New MR Received For Recommendation",
                Body = GenerateEmailBody(Session[SESSION_PROJECT_NAME].ToString(),
                                    Session[SESSION_JOB_NAME].ToString(),
                                    Session[SESSION_ENGINEER_NAME].ToString(), reciveDate, requestNo)

            };

            return email;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DateTime GetSriLankanTime()
        {
            string  displayName     = "(GMT+05:30) Sri Jaywardanapura Time";
            string  standardName    = "Sri Lanka Time";
            TimeSpan offset         = new TimeSpan(05, 30, 00);
            TimeZoneInfo mawson     = TimeZoneInfo.CreateCustomTimeZone(standardName, offset, displayName, standardName);

            DateTime localTime      = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, mawson);
            return localTime;
        }

		/// <summary>
		/// automatically generates an id for the estimation
		/// </summary>
		private string GenerateMaterialRequestID()
		{
			string materialRequestId;

			if(materialRequestDao == null)
			{
				materialRequestDao			= new MaterialRequestDAO();
			}

			materialRequestId				= materialRequestDao.GetNextMaterailRequestId();
			return materialRequestId;
		}

		/// <summary>
		/// Process item list to a data table.
		/// </summary>
		private void ProcessItemList(int materialRequestId)
		{
			int count					= 1;

			if(dtItems == null)
			{
				dtItems					= this.GenerateDataTable();

				foreach (GridViewRow row in grdItems.Rows)
				{
					DataRow dr				= dtItems.NewRow();
				
					dr["MrNumber"]			= materialRequestId;
					dr["ItemCode"]			= row.Cells[1].Text.ToString();
					dr["ItemName"]			= row.Cells[2].Text.ToString();
					dr["QTY"]				= Convert.ToDouble(row.Cells[4].Text.ToString()); // donald
					dr["OrderNo"]			= Convert.ToInt32(count);
					dr["QTYOrderd"]			= Convert.ToDouble(row.Cells[4].Text.ToString());
					dr["Remarks"]			= row.Cells[5].Text.ToString();
				
					dtItems.Rows.Add(dr);           
					count					= count + 1;
				}
			}
			
			Session[SESSION_ITEM_TABLE] = dtItems;
		}

		/// <summary>
		/// Generate data table
		/// </summary>
		private DataTable GenerateDataTable()
		{
			DataTable dataTable			= new DataTable();

			DataColumn dcMrNumber		= new DataColumn("MrNumber", typeof(Int32));
			DataColumn dcItemCode		= new DataColumn("ItemCode", typeof(string));
			DataColumn dcItemName		= new DataColumn("ItemName", typeof(string));
			DataColumn dcQty			= new DataColumn("QTY", typeof(Double));
			DataColumn dcOrderNo		= new DataColumn("OrderNo", typeof(Int32));
			DataColumn dcQtyOrder		= new DataColumn("QTYOrderd", typeof(Int32));
			DataColumn dcRemarks		= new DataColumn("Remarks", typeof(string));
			DataColumn dcMainMeasure	= new DataColumn("MainMeasure", typeof(string));

			dataTable.Columns.Add(dcMrNumber);
			dataTable.Columns.Add(dcItemCode);
			dataTable.Columns.Add(dcItemName);
			dataTable.Columns.Add(dcQty);
			dataTable.Columns.Add(dcOrderNo);
			dataTable.Columns.Add(dcQtyOrder);
			dataTable.Columns.Add(dcRemarks);
			dataTable.Columns.Add(dcMainMeasure);

			return dataTable;
		}

		/// <summary>
		/// validate form controls
		/// </summary>
		/// <returns></returns>
		private bool ValidateForm()
		{
			bool status					= true;
			if (grdItems.Rows.Count <= 0)
			{
				status					= false;
				SetMessage(2, "Please add items to save the Material request");
			}

            if (!RequiredDateValidation())
            {
                status = false;
                SetMessage(2, "Required Date should not be with in 21 days from today.");
            }

			return status;
		}

		/// <summary>
		/// Validations
		/// </summary>
		protected bool Validations()
		{
			bool status								= true;

			if (txtItemId.Text.Trim() == string.Empty)
			{
				status								= false;
				SetMessage(2, "Item ID cannot be empty. Please check the try again.");
			}

			if (txtItemName.Text.Trim() == string.Empty)
			{
				status								= false;
				SetMessage(2, "Item Name cannot be empty. Please check the try again.");
			}

			if (txtQty.Text.Trim() == string.Empty)
			{
				status								= false;
				SetMessage(2, "Item quantity cannot be empty. Please check the try again.");
			}

			if(!Validation.IsNumeric(txtQty.Text.Trim()))
			{
				status								= false;
				SetMessage(2, "Item quantity should be a number. Please check the try again.");
			}

			if (!ValidateItem(txtItemId.Text.Trim()))
			{
				status								= false;
				SetMessage(2, "Selected item is invalid. Please check the item name or select the item again.");
			}

            if(hfEstimationId.Value != string.Empty)
            {
                if (Convert.ToDouble(HiddenFieldlitBalanceQty.Value) <= 0)
                {
                    status = false;
                    SetMessage(2, "This Item does not have enough quantity to order.");
                }
                else if (ValidateQunatity())
                {
                    status = false;
                    SetMessage(2, "This Item Exceed Manager’s Estimated Quantity.");
                }
            }
			

			return status;
		}

		/// <summary>
		/// 
		/// </summary>
		private bool ValidateItem(string itemCode)
		{
		    bool		status				= false;

			if(materialRequestDao == null)
			{
				materialRequestDao			= new MaterialRequestDAO();
			}

			var dtItemDetails = materialRequestDao.CheckProductExistance(itemCode);
			if (dtItemDetails != null && dtItemDetails.Rows.Count > 0)
			{
				if ((itemCode == dtItemDetails.Rows[0]["ItemCode"].ToString()) 
					&& (txtItemName.Text.Trim() == dtItemDetails.Rows[0]["ItemDescription"].ToString().Trim()))
				{
					status					= true;
				}
			}

			return status;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool RequiredDateValidation()
        {
            DateTime    currentDate     = this.GetSriLankanTime();
            DateTime    requiredDate    = Convert.ToDateTime(dtApplyDate.Text);

            //TODO : Read from configuration
            var result = (requiredDate.Date - currentDate.Date).TotalDays > 21;

            return result;
        }

		/// <summary>
		/// validate the qty
		/// </summary>
		/// <returns></returns>
		private bool ValidateQunatity()
		{
			bool result = false;

            if (hfApprovedQty.Value == "")  // donald
            {
                hfApprovedQty.Value = "0";
            }

            if (hfIssuedQty.Value == "")
            {
                hfIssuedQty.Value = "0";
            }

            if (txtRequestQty.Text == "")
            {
                txtRequestQty.Text = "0";
            }

			double finalQty			= Convert.ToDouble(HiddenFieldlblApproveQty.Value);
			double requestedQty		= Convert.ToDouble(HiddenFieldlitBalanceQty.Value);

			double qty				= Convert.ToDouble(txtQty.Text.Trim());

            if (finalQty > 0)  // if (finalQty != -1 || finalQty != 0.0)  
            {
			    if ((qty > requestedQty))
			    {
				    result				= true;
			    }
			}

			return result;
		}

		/// <summary>
		/// Check item already contains in the dataTable
		/// </summary>
		private bool CheckItemStatus(DataTable dt, string itemId)
		{
			return dt.AsEnumerable().Any(row => itemId == row.Field<String>("ItemCode"));
		}

		/// <summary>
		/// Add rows to the data grid view
		/// </summary>
		private void AddRows(string itemId, string itemName, string qty, string remarks, string uom, DataTable dt)
        {
			if (dt != null)
			{
				DataRow dr						= dt.NewRow();
            
				dr[COLUMN_ITEM_CODE]			= itemId;
				dr[COLUMN_ITEM_NAME]			= itemName;
				dr[COLUMN_ITEM_QTY]				= Convert.ToDouble(qty);
				dr[COLUMN_ITEM_REMARKS]			= remarks;
				dr[COLUMN_ITEM_ORDER]			= dt.Rows.Count + 1;
				dr[COLUMN_ITEM_UOM]				= uom;

				dt.Rows.Add(dr);
			}
        }

		/// <summary>
		/// 
		/// </summary>
		private void ResetForm()
		{
			txtItemId.Text						= string.Empty;
			txtItemName.Text					= string.Empty;
			txtUnitofMeasure.Text				= string.Empty;
			txtRequestQty.Text					= string.Empty;
			txtQty.Text							= string.Empty;
			txtRemarks.Text						= string.Empty;
		}

		/// <summary>
		/// Reset the session values 
		/// </summary>
		private void ResetSession()
		{
			Session[SESSION_ITEM_TABLE]			= null;
		}

		/// <summary>
		/// 
		/// </summary>
		private void DisableForm()
		{
			txtEstimationId.Enabled			= false;
			dtApplyDate.Enabled				= false;
			txtComments.Enabled				= false;
			txtDeliverLocation.Enabled		= false;

			txtItemId.Enabled				= false;
			txtItemName.Enabled				= false;
			txtQty.Enabled					= false;
			txtUnitofMeasure.Enabled		= false;
			btnAdd.Enabled					= false;
			txtRemarks.Enabled				= false;


			btnSubmitForApproval.Visible	= false;
			btnSave.Visible					= false;

			//grdItems.Columns[5].Visible		= false;
			grdItems.Columns[6].Visible		= false;
			
		}

		/// <summary>
		/// Generate the email body
		/// </summary>
		private string GenerateEmailBody (string siteName, string jobNumber, string EngineerName, DateTime date, string mrNumber)
		{
			StringBuilder message = new StringBuilder();

			message.AppendLine(string.Format("Site : {0}", siteName));
			message.AppendLine(string.Format("Job No : {0}", jobNumber));
			message.AppendLine(string.Format("Engineer : {0}", EngineerName));
			message.AppendLine(string.Format("Date : {0}", date));
			message.AppendLine(string.Format("Web MR No : {0}", mrNumber));

			return message.ToString();
		}

		#endregion

        #region Events

		/// <summary>
		/// Handles the page load event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
			//disable controls on page load
			txtRequestQty.Attributes.Add("readonly", "readonly");
			txtUnitofMeasure.Attributes.Add("readonly", "readonly");

            if (!IsPostBack)
            {
				//reset data grid sessions
				this.ResetSession();
				//process query string values
				this.ProcessQueryString();
				//load data according to the form type
				this.LoadData();
				//set session values
				this.SetSessionValues();
			}
			else
			{
				if (Session[SESSION_FORM_MODE] != null)
				{
					FormMode						= (Mode)Session[SESSION_FORM_MODE];
				}
				else
				{
					Response.Redirect("~/Home.aspx");
				}
			}

            error_alert.Visible						= false;
            success_alert.Visible					= false;
        }
		
		/// <summary>
		/// handles approval submit button click event
		/// </summary>
		protected void btnSubmitForApproval_Click(object sender, EventArgs e)
		{
		    if (this.SaveMaterialRequest(ENTRY_MANAGER, FINISH_ENGINEER))
		    {
                Response.Redirect("~/Home.aspx", true);
            }
            
		}

		/// <summary>
		/// Handles save button click event
		/// </summary>
		protected void btnSave_Click(object sender, EventArgs e)
		{
			this.SaveMaterialRequest(string.Empty, ENTRY_ENGINEER);
		}

		/// <summary>
		/// Handles clear button click event
		/// </summary>
		protected void btnClearAll_Click(object sender, EventArgs e)
		{
 
		}

		/// <summary>
		/// Handles the add button click
		/// </summary>
		protected void btnAdd_Click(object sender, EventArgs e)
		{
			if ((DataTable)Session[SESSION_ITEM_TABLE] != null) 
			{
				dtItems								= (DataTable)Session[SESSION_ITEM_TABLE];
			}
			else
			{
				dtItems								= this.GenerateDataTable();
			}

			if (Validations())
			{
				string itemId						= txtItemId.Text.Trim();
				string itemName						= txtItemName.Text.Trim();
				string itemQty						= txtQty.Text.Trim();
				string itemRemarks					= txtRemarks.Text.Trim();
				string itemUom						= txtUnitofMeasure.Text.Trim();
					
				if (Session["EditRow"] != null)
				{
					DataRow	dr						= (DataRow)Session["EditRow"];

					if(this.CheckItemStatus(dtItems, txtItemId.Text.Trim()) 
						&&  txtItemId.Text.Trim() == dr[COLUMN_ITEM_CODE].ToString().Trim())
					{
						if (dr.ItemArray.Length > 0)
						{

							dtItems.PrimaryKey			= new DataColumn[1] { dtItems.Columns["ItemCode"] };  // set your primary key
							DataRow dRow				= dtItems.Rows.Find(dr[COLUMN_ITEM_CODE]);

							dRow[COLUMN_ITEM_CODE]		= itemId;
							dRow[COLUMN_ITEM_NAME]		= itemName;
							dRow[COLUMN_ITEM_QTY]		= Convert.ToDouble(itemQty);
							dRow[COLUMN_ITEM_REMARKS]	= itemRemarks;
							dRow[COLUMN_ITEM_UOM]		= itemUom;


                            this.ResetErrors();
						    hfEditingItemNo.Value = string.Empty;
						}
					}
					else
					{
						this.SetDuplicateItemMessage(dtItems, itemId);

					}
				}
				else
				{
					if(!this.CheckItemStatus(dtItems, txtItemId.Text.Trim()))
					{
						AddRows(itemId, itemName, itemQty, itemRemarks, itemUom, dtItems);

					    this.ResetErrors();

					}
					else
					{
						this.SetDuplicateItemMessage(dtItems, itemId);
                    }
				}
					
                
				txtTotalItem.Text						= dtItems.Rows.Count.ToString();     
            
				this.ResetForm();
				this.txtItemName.Focus();

				Session["EditRow"]						= null;
				Session[SESSION_ITEM_TABLE]				= dtItems;
            
				grdItems.DataSource						= dtItems;
				grdItems.DataBind();

                HiddenFieldlblApproveQty.Value          = "0";
                HiddenFieldlitIssuedQty.Value           = "0";
                HiddenFieldlitPendingQty.Value          = "0";
                HiddenFieldlitBalanceQty.Value          = "0";

                lblApproveQty.Text = string.Empty;
                litIssuedQty.Text = string.Empty;
                litPendingQty.Text = string.Empty;
                litBalanceQty.Text = string.Empty;

                txtItemId.ReadOnly = false;
                txtItemName.ReadOnly = false;

            }
            else
            {
                lblApproveQty.Text                      = HiddenFieldlblApproveQty.Value;
                litIssuedQty.Text                       = HiddenFieldlitIssuedQty.Value;
                litPendingQty.Text                      = HiddenFieldlitPendingQty.Value;
                litBalanceQty.Text                      = HiddenFieldlitBalanceQty.Value;
            }
			
		}

		/// <summary>
		/// Handles the item row command
		/// </summary>
		protected void grdItems_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == COMMAND_REMOVE)
			{
				if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
				{
					dtItems									= (DataTable)Session[SESSION_ITEM_TABLE];
					if (dtItems.Rows.Count > 0)
					{
						dtItems.PrimaryKey					= new DataColumn[1] { dtItems.Columns["ItemCode"] };  // set your primary key
						DataRow dRow						= dtItems.Rows.Find(e.CommandArgument);
						if (dRow != null)
						{
							dtItems.Rows.Remove(dRow);

							txtTotalItem.Text					= dtItems.Rows.Count.ToString();
							//set data table to session 
							Session[SESSION_ITEM_TABLE]			= dtItems;
						}
					}
				}
			}
			else if(e.CommandName == COMMAND_EDIT)
			{
				var			index							= e.CommandArgument.ToString();

                this.SetEditMode(index);
			}

			grdItems.DataSource								= dtItems;
			grdItems.DataBind();
		}

        private void SetEditMode(string index)
        {
            if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
            {
                dtItems = (DataTable)Session[SESSION_ITEM_TABLE];
                dtItems.PrimaryKey = new DataColumn[1] { dtItems.Columns["ItemCode"] };  // set your primary key

                DataRow dRow = dtItems.Rows.Find(index);

                if (dRow != null)
                {
                    txtItemId.Text          = dRow["ItemCode"].ToString();
                    txtItemName.Text        = dRow["ItemName"].ToString();
                    txtQty.Text             = dRow["Qty"].ToString();
                    txtRemarks.Text         = dRow["Remarks"].ToString();
                    txtUnitofMeasure.Text   = dRow["MainMeasure"].ToString();

                    //load item quantity details
                    if (materialRequestDao == null)
                    {
                        materialRequestDao = new MaterialRequestDAO();
                    }

                    int eid = -1;
                    if (hfEstimationId.Value != "")
                    {
                        eid = Convert.ToInt32(hfEstimationId.Value);
                    }

                    string[] itemdetails = materialRequestDao.GetEstimationItems(txtItemName.Text, eid);

                    if (itemdetails.Length >= 1)
                    {
                        string[] itemdetail = itemdetails[0].Split('~');
                        string final = itemdetail[4];
                        string issued = itemdetail[5];
                        string req = itemdetail[6];

                        lblApproveQty.Text = final;
                        HiddenFieldlblApproveQty.Value = final;

                        litIssuedQty.Text = issued;
                        HiddenFieldlitIssuedQty.Value = issued;

                        this.CalculatePendingAndBalance(Convert.ToDouble(final),
                                                            Convert.ToDouble(issued),
                                                            Convert.ToDouble(req));

                    }

                    //disable item name and code
                    txtItemId.ReadOnly = true;
                    txtItemName.ReadOnly = true;

                    //dtItems.Rows.Remove(dRow);
                    Session["EditRow"] = dRow;
                    Session[SESSION_ITEM_TABLE] = dtItems;

                }
            }
        }


        private void CalculatePendingAndBalance(double estimated, double issued, double ordered) {

            if (ordered < issued) {

                litPendingQty.Text = "0";
                litBalanceQty.Text = (estimated - issued).ToString();

                HiddenFieldlitPendingQty.Value = "0";
                HiddenFieldlitBalanceQty.Value= (estimated - issued).ToString();

                txtRequestQty.Text = (estimated - issued).ToString();

            } else {
                
                litPendingQty.Text = (ordered - issued).ToString();
                litBalanceQty.Text = (estimated - ordered).ToString();

                HiddenFieldlitPendingQty.Value = (ordered - issued).ToString();
                HiddenFieldlitBalanceQty.Value= (estimated - ordered).ToString();

                txtRequestQty.Text = (estimated - ordered).ToString();
                
            }
        }

		/// <summary>
		/// Handles the page index event
		/// </summary>
		protected void grdItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			grdItems.PageIndex				= e.NewPageIndex;

            if((DataView)Session[SESSION_ITEM_TABLE_FILTER] != null)
            {
                grdItems.DataSource = (DataView)Session[SESSION_ITEM_TABLE_FILTER]; ;
                grdItems.DataBind();
            }
			else if ((DataTable)Session[SESSION_ITEM_TABLE] != null) 
			{
				dtItems						= (DataTable)Session[SESSION_ITEM_TABLE];

                grdItems.DataSource = dtItems;
                grdItems.DataBind();
            }
	
		}

        private void SetDuplicateItemMessage(DataTable dt, string itemId)
        {
            DataRow dr = dt.AsEnumerable().FirstOrDefault(row => itemId == row.Field<String>("ItemCode"));
            if (dr != null && dr.ItemArray.Any())
            {
                this.SetMessage(3, "Item already exists. Quantity :  " + dr[COLUMN_ITEM_QTY].ToString());
            }
            else
            {
                this.SetMessage(3, "Item already exists.");
            }
            hfEditingItemNo.Value = itemId;
        }

		#endregion

		#region Properties

        private Mode FormMode { set; get; } = Mode.New;

        #endregion

		#region Web Methods

		[System.Web.Services.WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string[] GetEstimationDetails(string prefix, int estimationId)
        {
			string[]			itemList			= null;
			
			MaterialRequestDAO	materialRequestDao	= new MaterialRequestDAO();

            itemList = materialRequestDao.GetEstimationItems(HttpUtility.UrlDecode(prefix), estimationId);

			return itemList;
        }

		[System.Web.Services.WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public static string GetEstimationItemByCode(string itemCode)
        {
			string			itemList			= null;
			
			MaterialRequestDAO	materialRequestDao	= new MaterialRequestDAO();

			itemList								= materialRequestDao.GetEstimationItemById(itemCode);

			return itemList;
        }
       
		#endregion

        protected void txtQty_TextChanged1(object sender, EventArgs e)
        {
            decimal value;
            if (Decimal.TryParse(txtQty.Text, out value))
            // It's a decimal
            { }
            else
            {
                txtQty.Text = "1";
            }
            // No it's not
        }

        protected void btnMsg_OnClick(object sender, EventArgs e)
        {
            var index = hfEditingItemNo.Value;

            this.SetEditMode(index);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
            {
                dtItems = (DataTable)Session[SESSION_ITEM_TABLE];
                DataView dtFilter = dtItems.AsDataView();

                dtFilter.RowFilter = "ItemName like '%" + txtSearchQuery.Text.Trim().Replace("'", "''") + "%'";
                if(dtFilter.ToTable().Rows.Count > 0)
                {
                    lblFilterSummary.Text = string.Format("{0} items found.", dtFilter.ToTable().Rows.Count.ToString());
                }
                else
                {
                    lblFilterSummary.Text = "0 items found.";
                }

                pnlFilterSummary.Visible = true;

                Session[SESSION_ITEM_TABLE_FILTER] = dtFilter;

                grdItems.DataSource = dtFilter;
                grdItems.DataBind();
            }
            
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
            {
                lblFilterSummary.Text = "";
                pnlFilterSummary.Visible = false;

                Session[SESSION_ITEM_TABLE_FILTER] = null;

                grdItems.DataSource = (DataTable)Session[SESSION_ITEM_TABLE];
                grdItems.DataBind();
            }
        }
    }
}