using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAO;
using SLII_Web.Classes;

namespace KentWebApplication
{
	public partial class EstimateQuotationList : System.Web.UI.Page
	{

		#region Enum
		public enum Mode
		{
			New			= 0,
			Saved		= 1,
			Submitted	= 2,
			Approved	= 3
		}

		#endregion

		#region Constant

		protected const string QUERY_CUSTOMER_CODE		        = "cid";
		protected const string QUERY_JOB_CODE			        = "jid";
		protected const string QUERY_TYPE_CODE			        = "tid";
		protected const string QUERY_ENGINEER_CODE		        = "eid";

		protected const string SESSION_ITEM_TABLE		        = "ITEM_TABLE";
		protected const string SESSION_CUSTOMER_ID		        = "CUSTOMER_ID";
		protected const string SESSION_JOB_ID			        = "JOB_ID";
		protected const string SESSION_ENGINEER_ID		        = "ENGINEER_ID";
		protected const string SESSION_FORM_MODE		        = "FORM_MODE";
		protected const string SESSION_TYPE_ID			        = "TYPE";

		protected const string COMMAND_DOWNLOAD			        = "download";
		protected const string COMMAND_VIEW				        = "view";

        protected const string URL_MATERIAL_REQUEST = "EstimateQuotation.aspx?jid={0}&cid={1}&eid={2}&tid={3}";
		protected const string URL_PRINT				        = "~/Reports/SubEstimateViewer.aspx?eid={0}";

		#endregion

		#region Member

		private EstimateQuotationDAO	estimateQuotationDao	= null;

		private	int					    type				    =	-1;
		private int					    customerCode		    =	-1;
		private int					    jobCode				    =	-1;
		private int					    engineerCode		    =	-1;
		private int					    managerCode			    =	-1;

		private Mode				    formMode			    = Mode.New;

		#endregion

		#region Event

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				//process query string values
				this.ProcessQueryString();
				//set session values
				this.SetSessionValues();
				//get material request list
				this.GetMaterialRequestList();
			}
		}

		protected void grdMaterialRequest_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == COMMAND_DOWNLOAD)
			{
				string estimationId						        = e.CommandArgument.ToString();
				
				//get new material request url
				string formattedMrUrl					        = string.Format(URL_PRINT, estimationId);

				Response.Redirect(formattedMrUrl);
			}
			else if(e.CommandName == COMMAND_VIEW)
			{
				string estimationId						        = e.CommandArgument.ToString();
				
				//get new material request url
				string formattedMrUrl					        = string.Format(URL_MATERIAL_REQUEST, new object[] {
																		Session[SESSION_JOB_ID].ToString(),
																		Session[SESSION_CUSTOMER_ID].ToString(),
																		estimationId, "2" });

				Response.Redirect(formattedMrUrl);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Get material request details
		/// </summary>
		private void GetMaterialRequestList()
		{
			try
			{
                if (estimateQuotationDao == null)
                {
                    estimateQuotationDao = new EstimateQuotationDAO();
                }

                DataTable dtMaterialRequest = estimateQuotationDao.GetMaterialRequestList(customerCode, jobCode, engineerCode);
				if (dtMaterialRequest != null && dtMaterialRequest.Rows.Count > 0)
				{
					Session[SESSION_ITEM_TABLE]			= dtMaterialRequest;

					litProjectName.Text					= dtMaterialRequest.Rows[0]["CustomerName"].ToString();

					grdMaterialRequest.DataSource		= dtMaterialRequest;
					grdMaterialRequest.DataBind();

				}
			}
			catch (Exception ex)
			{

			}
		}

		/// <summary>
		/// Process the query string values
		/// </summary>
		private void ProcessQueryString()
		{
			customerCode								= General.GetQueryStringInt(Request.QueryString[QUERY_CUSTOMER_CODE]);
			jobCode										= General.GetQueryStringInt(Request.QueryString[QUERY_JOB_CODE]);
			type										= General.GetQueryStringInt(Request.QueryString[QUERY_TYPE_CODE]);
			engineerCode								= General.GetQueryStringInt(Request.QueryString[QUERY_ENGINEER_CODE]);
		}

		/// <summary>
		/// Set the values from query string to the session
		/// </summary>
		private void SetSessionValues()
		{
			Session[SESSION_CUSTOMER_ID]				= customerCode;
			Session[SESSION_JOB_ID]						= jobCode;
			Session[SESSION_TYPE_ID]					= type;
			Session[SESSION_ENGINEER_ID]				= engineerCode;
			Session[SESSION_FORM_MODE]					= FormMode;
		}

		#endregion

		#region Properties
		public Mode FormMode
		{
			set { this.formMode = value; }
			get { return this.formMode; }
		}

		#endregion

		protected void grdMaterialRequest_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			grdMaterialRequest.PageIndex				= e.NewPageIndex;

			DataTable dtItems							= null;
			if ((DataTable)Session[SESSION_ITEM_TABLE] != null) 
			{
				dtItems									= (DataTable)Session[SESSION_ITEM_TABLE];
			}
			else
			{
                dtItems = estimateQuotationDao.GetMaterialRequestList(customerCode, jobCode, engineerCode);
			}

			if (dtItems != null && dtItems.Rows.Count > 0)
			{
				grdMaterialRequest.DataSource			= dtItems;
				grdMaterialRequest.DataBind();	
			}
			
		}

	}
}