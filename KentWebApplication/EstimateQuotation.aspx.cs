using DAO;
using KentWebApplication.Classes;
using SLII_Web.Classes;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.UI.WebControls;
using Entity = DAO.Entities;

namespace KentWebApplication
{
    public partial class EstimateQuotation : ParentPage
    {
        #region Constant

        protected const string QUERY_CUSTOMER_CODE = "cid";
        protected const string QUERY_JOB_CODE = "jid";
        protected const string QUERY_TYPE_CODE = "tid";
        protected const string QUERY_ENGINEER_CODE = "eid";

        protected const string SESSION_ITEM_TABLE = "ITEM_TABLE";
        protected const string SESSION_ITEM_TABLE_FILTER = "ITEM_TABLE_FILTERED";
        protected const string SESSION_CUSTOMER_ID = "CUSTOMER_ID";
        protected const string SESSION_JOB_ID = "JOB_ID";
        protected const string SESSION_FORM_MODE = "FORM_MODE";
        protected const string SESSION_TYPE_ID = "TYPE";
        protected const string SESSION_ENGINEER_ID = "ENGINEER_ID";
        protected const string SESSION_MANAGER_ID = "MANAGER_ID";
        protected const string SESSION_ES_ID = "MR_ID";

        protected const string SESSION_PROJECT_NAME = "PROJECT_NAME";
        protected const string SESSION_JOB_NAME = "JOB_NAME";
        protected const string SESSION_ENGINEER_NAME = "ENGINEER_NAME";
        protected const string SESSION_MANAGER_NAME = "MANAGER_NAME";
        protected const string SESSION_MANAGER_EMAIL_ADDRESS = "MANAGER_EMAIL_ADDRESS";

        protected const string ENTRY_ENGINEER = "ENTRY";
        protected const string FINISH_ENGINEER = "FINISH";
        protected const string CLOSED_ENGINEER = "CLOSED";

        protected const string ENTRY_MANAGER = "ENTRY";
        protected const string FINISH_MANAGER = "FINISH";
        protected const string CLOSED_MANAGER = "CLOSED";

        protected const string COLUMN_ITEM_CODE = "ItemCode";
        protected const string COLUMN_ITEM_NAME = "ItemName";
        protected const string COLUMN_ITEM_QTY = "Qty";
        protected const string COLUMN_ITEM_AMOUNT = "Amount";
        protected const string COLUMN_ITEM_REMARKS = "Remarks";
        protected const string COLUMN_ITEM_TOTAL = "Total";
        protected const string COLUMN_ITEM_ORDER = "OrderNo";
        protected const string COLUMN_ITEM_UOM = "MainMeasure";

        protected const string COMMAND_REMOVE = "Remove";
        protected const string COMMAND_EDIT = "Change";

        #endregion

        #region Member

        private EstimateQuotationDAO estimateQuoDao = null;
        private DataTable dtItems = null;

        private int type = -1;
        private static int customerCode = -1;
        private static int jobCode = -1;
        private int engineerCode = -1;
        private int managerCode = -1;
        private int materialRequestId = -1;
        private int id = -1;

        #endregion

        #region Enum
        public enum Mode
        {
            New = 0,
            Saved = 1,
            Submitted = 2,
            Approved = 3
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
                    success_alert.Visible = true;
                    litSuccessMessage.Text = message;
                    break;
                case 2: // Error
                    error_alert.Visible = true;
                    litErrorMessage.Text = message;
                    break;
                case 3:
                    duplicate_alert.Visible = true;
                    litMessage.Text = message;
                    break;
            }
        }

        /// <summary>
        /// process query string values
        /// </summary>
        private void ProcessQueryString()
        {
            customerCode = General.GetQueryStringInt(Request.QueryString[QUERY_CUSTOMER_CODE]);
            jobCode = General.GetQueryStringInt(Request.QueryString[QUERY_JOB_CODE]);
            type = General.GetQueryStringInt(Request.QueryString[QUERY_TYPE_CODE]);
            id = General.GetQueryStringInt(Request.QueryString[QUERY_ENGINEER_CODE]);
        }

        /// <summary>
        /// Set the values from query string to the session
        /// </summary>
        private void SetSessionValues()
        {
            Session[SESSION_CUSTOMER_ID] = customerCode;
            Session[SESSION_JOB_ID] = jobCode;
            Session[SESSION_TYPE_ID] = type;
            Session[SESSION_FORM_MODE] = FormMode;
            Session[SESSION_ENGINEER_ID] = engineerCode;
            Session[SESSION_MANAGER_ID] = managerCode;
        }

        /// <summary>
        /// load estimation details
        /// </summary>
        private void LoadESDetails(int customerCode, int jobCode)
        {
            try
            {
                HomeDAO homeDao = null;
                string userName = string.Empty;

                userName = this.Context.User.Identity.Name;

                if (homeDao == null)
                {
                    homeDao = new HomeDAO();
                }

                string empCode = homeDao.GetEmployeeCodeByUserName(userName);   //donald


                //get details from site details
                if (id != -1)
                {
                    DataTable dtSiteDetails = homeDao.GetSiteByCustomerAndJob(customerCode, jobCode, id);

                    if (dtSiteDetails != null && dtSiteDetails.Rows.Count > 0)
                    {
                        txtManager.Text = dtSiteDetails.Rows[0]["ManagerName"].ToString();
                        txtEngineer.Text = dtSiteDetails.Rows[0]["EngineerName"].ToString();
                        engineerCode = Convert.ToInt32(dtSiteDetails.Rows[0]["EngineerNo"].ToString());
                        managerCode = Convert.ToInt32(dtSiteDetails.Rows[0]["ManagerNo"].ToString());

                        litSubEstimateID.Text = "New";

                        litProjectName.Text = string.Format("{0} | {1}", dtSiteDetails.Rows[0]["CustomerName"].ToString(), dtSiteDetails.Rows[0]["JobName"].ToString()); ;
                        dtApplyDate.Text = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

                        Session[SESSION_PROJECT_NAME] = dtSiteDetails.Rows[0]["CustomerName"].ToString();
                        Session[SESSION_JOB_NAME] = dtSiteDetails.Rows[0]["JobName"].ToString();
                        Session[SESSION_MANAGER_NAME] = dtSiteDetails.Rows[0]["ManagerName"].ToString();
                        Session[SESSION_ENGINEER_NAME] = dtSiteDetails.Rows[0]["EngineerName"].ToString();
                        Session[SESSION_MANAGER_EMAIL_ADDRESS] = dtSiteDetails.Rows[0]["ManagerEmailAddress"].ToString();
                    }
                }

                LoadDataGridItems();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Load data grid items from the list
        /// </summary>
        public void LoadDataGridItems()
        {
            if (Session[SESSION_ITEM_TABLE] != null)
            {
                grdItems.DataSource = (DataTable)Session[SESSION_ITEM_TABLE];
                grdItems.DataBind();
            }
        }

        /// <summary>
        /// load data to the form
        /// </summary>
        private void LoadData()
        {
            if (type != -1)
            {
                switch (type)
                {
                    case 0: //load new material request form
                        FormMode = Mode.New;

                        //reset the session MR ID
                        Session[SESSION_ES_ID] = null;

                        //load sub estimation header details
                        this.LoadESDetails(customerCode, jobCode);

                        break;
                    case 1: //load previously saved material request form
                        FormMode = Mode.Saved;

                        //load material request header details
                        this.GetSubEstimateById(customerCode, jobCode);

                        //load material request item list
                        this.GetSubEstimationDetails();

                        break;
                    case 2:

                        //load material request header details
                        this.GetSubEstimationMrNumber(id);

                        //load material request item list
                        this.GetSubEstimationDetails();

                        this.DisableForm();

                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// get estimate quotation header details
        /// </summary>
        private void GetSubEstimateById(int customerId, int jobId)
        {
            if (estimateQuoDao == null)
            {
                estimateQuoDao = new EstimateQuotationDAO();
            }

            HomeDAO homeDao = null;
            string userName = string.Empty;

            userName = this.Context.User.Identity.Name;

            if (homeDao == null)
            {
                homeDao = new HomeDAO();
            }

            string empCode = homeDao.GetEmployeeCodeByUserName(userName);   //donald
            DataTable dtMaterialRequest = estimateQuoDao.GetMaterialRequestByID(customerId, jobId, empCode);  //donald

            if (dtMaterialRequest != null && dtMaterialRequest.Rows.Count > 0)
            {
                this.SetUi(dtMaterialRequest);
            }
        }

        /// <summary>
        /// Get the sub estimation header by the mr number
        /// </summary>
        /// <param name="mrNumber"></param>
        private void GetSubEstimationMrNumber(int mrNumber)
        {
            if (estimateQuoDao == null)
            {
                estimateQuoDao = new EstimateQuotationDAO();
            }

            DataTable dtMaterialRequest = estimateQuoDao.GetMaterialRequestByMRNumber(mrNumber);

            if (dtMaterialRequest != null && dtMaterialRequest.Rows.Count > 0)
            {
                SetUi(dtMaterialRequest);
            }
        }

        /// <summary>
        /// Set the UI elemwent s accordning to the data populated
        /// </summary>
        private void SetUi(DataTable dtMaterialRequest)
        {
            // Header details
            materialRequestId = Convert.ToInt32(dtMaterialRequest.Rows[0]["MRNumber"].ToString());
            Session[SESSION_ES_ID] = materialRequestId;
            //set estimation id
            hfEstimationId.Value = dtMaterialRequest.Rows[0]["EstimateNo"].ToString();

            txtEngineer.Text = dtMaterialRequest.Rows[0]["EngineerName"].ToString();
            txtManager.Text = dtMaterialRequest.Rows[0]["ManagerName"].ToString();
            dtApplyDate.Text = Convert.ToDateTime(dtMaterialRequest.Rows[0]["RequiredDate"].ToString()).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            dtReceivedDate.Text = Convert.ToDateTime(dtMaterialRequest.Rows[0]["ReceivedDate"].ToString()).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            txtComments.Text = dtMaterialRequest.Rows[0]["Remarks"].ToString();
            txtDeliverLocation.Text = dtMaterialRequest.Rows[0]["LocationOfDeliver"].ToString();

            litProjectName.Text = string.Format("{0} | {1}", dtMaterialRequest.Rows[0]["CustomerName"].ToString(), dtMaterialRequest.Rows[0]["JobName"].ToString());
            litSubEstimateID.Text = dtMaterialRequest.Rows[0]["MRNumber"].ToString();

            engineerCode = Convert.ToInt32(dtMaterialRequest.Rows[0]["EngineerNo"].ToString());
            managerCode = Convert.ToInt32(dtMaterialRequest.Rows[0]["ManagerNo"].ToString());

            Session[SESSION_PROJECT_NAME] = dtMaterialRequest.Rows[0]["CustomerName"].ToString();
            Session[SESSION_JOB_NAME] = dtMaterialRequest.Rows[0]["JobName"].ToString();
            Session[SESSION_MANAGER_NAME] = dtMaterialRequest.Rows[0]["ManagerName"].ToString();
            Session[SESSION_ENGINEER_NAME] = dtMaterialRequest.Rows[0]["EngineerName"].ToString();
            Session[SESSION_MANAGER_EMAIL_ADDRESS] = dtMaterialRequest.Rows[0]["ManagerEmailAddress"].ToString();

        }

        /// <summary>
        /// get material request item list
        /// </summary>
        private void GetSubEstimationDetails()
        {
            if (estimateQuoDao == null)
            {
                estimateQuoDao = new EstimateQuotationDAO();
            }

            DataTable dtMaterialRequestItem = estimateQuoDao.GetMaterialRequestItemByMaterialRequestId(materialRequestId);

            if (dtMaterialRequestItem != null && dtMaterialRequestItem.Rows.Count > 0)
            {
                Session[SESSION_ITEM_TABLE] = dtMaterialRequestItem;
                txtTotalItem.Text = dtMaterialRequestItem.Rows.Count.ToString();

                // material request item details
                grdItems.DataSource = dtMaterialRequestItem;
                grdItems.DataBind();
            }
        }

        /// <summary>
        /// save material request data
        /// </summary>
        private bool SaveMaterialRequest(string managerStatus, string engineerStatus)
        {
            bool isSaved = false;

            try
            {
                if (this.ValidateForm())
                {
                    Entity.EstimateQuotation materialRequest = new Entity.EstimateQuotation();

                    materialRequest.MrBookNumber = -1;
                    materialRequest.CustomerCode = Convert.ToInt32(Session[SESSION_CUSTOMER_ID].ToString());
                    materialRequest.JobCode = Convert.ToInt32(Session[SESSION_JOB_ID].ToString());
                    materialRequest.LocationOfDelivery = txtDeliverLocation.Text.Trim();
                    materialRequest.RequiredDate = Convert.ToDateTime(dtApplyDate.Text.Trim());
                    materialRequest.Remarks = txtComments.Text.Trim();
                    materialRequest.MrNumberUpdateTo = 1;
                    materialRequest.Id = 1;
                    materialRequest.ManagerStatus = managerStatus;
                    materialRequest.EngineerStatus = engineerStatus;
                    materialRequest.EngineerId = Convert.ToInt32(Session[SESSION_ENGINEER_ID].ToString());
                    materialRequest.ManagerId = Convert.ToInt32(Session[SESSION_MANAGER_ID].ToString());

                    //create object instant for DAO
                    if (estimateQuoDao == null)
                    {
                        estimateQuoDao = new EstimateQuotationDAO();
                    }

                    //get data table
                    if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
                    {
                        dtItems = (DataTable)Session[SESSION_ITEM_TABLE];
                    }

                    if (managerStatus == ENTRY_MANAGER && engineerStatus == FINISH_ENGINEER)
                    {
                        materialRequest.ReceivedDate = Convert.ToDateTime(DateTime.Now);
                    }
                    else
                    {
                        materialRequest.ReceivedDate = Convert.ToDateTime("1753/01/01 12:00:00");
                    }

                    //check for form mode
                    if (FormMode == Mode.New)
                    {
                        materialRequest.MrNumber = Convert.ToInt32(GenerateMaterialRequestID());
                        Session[SESSION_ES_ID] = materialRequest.MrNumber;

                        this.ProcessItemList(materialRequest.MrNumber);

                        isSaved = estimateQuoDao.SaveMaterialRequest(materialRequest, (DataTable)Session[SESSION_ITEM_TABLE]);
                    }
                    else if (FormMode == Mode.Saved)
                    {
                        materialRequest.MrNumber = (int)Session[SESSION_ES_ID];

                        this.ProcessItemList(materialRequest.MrNumber);

                        isSaved = estimateQuoDao.UpdateMaterialRequest(materialRequest, (DataTable)Session[SESSION_ITEM_TABLE]);
                    }

                    //
                    if (isSaved)
                    {
                        Session[SESSION_FORM_MODE] = Mode.Saved;


                        if (engineerStatus == FINISH_ENGINEER)
                        {
                            bool result = this.SendEmail(materialRequest.ReceivedDate, materialRequest.MrNumber.ToString());

                            if (result)
                            {
                                this.SetMessage(1, "Material request successfully saved.");
                            }
                            else
                            {
                                this.SetMessage(1, "Material request successfully saved.But email sending failed.");
                            }
                        }
                        this.SetMessage(1, "Material request successfully saved.");
                    }
                    else
                    {
                        this.SetMessage(2, "Error occurred while saving the Material Request.");
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return isSaved;
        }


        /// <summary>
        /// Send the email to the email address
        /// </summary>
        public bool SendEmail(DateTime receivedData, string mrNumber)
        {
            bool result = false;

            Email email = new Email()
            {
                HostAddress = Config.SmtpHostName,
                FromAddress = Config.SmtpUserName,
                SmtpPassword = Config.SmtpPassword,
                ToAddress = Session[SESSION_MANAGER_EMAIL_ADDRESS].ToString(),
                Subject = "New Sub Estimation Received ",
                Body = GenerateEmailBody(Session[SESSION_PROJECT_NAME].ToString(),
                                         Session[SESSION_JOB_NAME].ToString(),
                                         Session[SESSION_ENGINEER_NAME].ToString(),
                                         receivedData,
                                         mrNumber)
            };

            try
            {
                email.SendMail();
                result = true;
            }
            catch (Exception)
            {

            }

            return result;

        }

        /// <summary>
        /// automatically generates an id for the estimation
        /// </summary>
        protected string GenerateMaterialRequestID()
        {
            string materialRequestId = string.Empty;
            if (estimateQuoDao == null)
            {
                estimateQuoDao = new EstimateQuotationDAO();
            }

            materialRequestId = estimateQuoDao.GetNextMaterailRequestId();
            return materialRequestId;
        }

        /// <summary>
        /// Process item list to a data table.
        /// </summary>
        private void ProcessItemList(int materialRequestId)
        {
            int count = 1;

            if (dtItems == null)
            {
                dtItems = this.GenerateDataTable();

                foreach (GridViewRow row in grdItems.Rows)
                {
                    DataRow dr = dtItems.NewRow();

                    dr["MrNumber"] = materialRequestId;
                    dr["ItemCode"] = row.Cells[1].Text.ToString();
                    dr["ItemName"] = row.Cells[2].Text.ToString();
                    dr["QTY"] = Convert.ToDouble(row.Cells[4].Text.ToString()); // donald
                    dr["OrderNo"] = Convert.ToInt32(count);
                    dr["QTYOrderd"] = Convert.ToDouble(row.Cells[4].Text.ToString());
                    dr["Remarks"] = row.Cells[5].Text.ToString();

                    dtItems.Rows.Add(dr);
                    count = count + 1;
                }
            }

            Session[SESSION_ITEM_TABLE] = dtItems;
        }

        /// <summary>
        /// Process date
        /// </summary>
		protected DateTime ProcessDate(string dateValue)
        {
            DateTime processValue = DateTime.Now;
            if (dateValue != string.Empty)
            {
                processValue = Convert.ToDateTime(dateValue);
            }
            return processValue;
        }

        /// <summary>
        /// Generate data table
        /// </summary>
        protected DataTable GenerateDataTable()
        {
            DataTable dataTable = new DataTable();

            DataColumn dcMrNumber = new DataColumn("MrNumber", typeof(Int32));
            DataColumn dcItemCode = new DataColumn("ItemCode", typeof(string));
            DataColumn dcItemName = new DataColumn("ItemName", typeof(string));
            DataColumn dcQty = new DataColumn("QTY", typeof(Double));
            DataColumn dcOrderNo = new DataColumn("OrderNo", typeof(Int32));
            DataColumn dcQtyOrder = new DataColumn("QTYOrderd", typeof(Int32));
            DataColumn dcRemarks = new DataColumn("Remarks", typeof(string));
            DataColumn dcMainMeasure = new DataColumn("MainMeasure", typeof(string));

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
        private bool ValidateForm()
        {
            var status = true;
            if (grdItems.Rows.Count <= 0)
            {
                status = false;
                SetMessage(2, "Please add items to save the Material request");
            }

            return status;
        }

        /// <summary>
        /// Validations
        /// </summary>
        protected bool Validations()
        {
            bool status = true;

            if (txtItemId.Text.Trim() == string.Empty)
            {
                status = false;
                SetMessage(2, "Item ID cannot be empty. Please check the try again.");
            }

            if (txtItemName.Text.Trim() == string.Empty)
            {
                status = false;
                SetMessage(2, "Item Name cannot be empty. Please check the try again.");
            }

            if (txtQty.Text.Trim() == string.Empty)
            {
                status = false;
                SetMessage(2, "Item quantity cannot be empty. Please check the try again.");
            }

            if (!Validation.IsNumeric(txtQty.Text.Trim()))
            {
                status = false;
                SetMessage(2, "Item quantity should be a number. Please check the try again.");
            }

            if (!ValidateItem(txtItemId.Text.Trim()))
            {
                status = false;
                SetMessage(2, "Selected item is invalid. Please check the item name or select the item again.");
            }

            //if(ValidateQunatity())
            //{
            //	status								= false;
            //	SetMessage(2, "This Item Exceed Manager’s Estimated Quantity.");
            //}

            return status;
        }

        /// <summary>
        /// Validate item 
        /// </summary>
        private bool ValidateItem(string itemCode)
        {
            DataTable dtItemDetails = null;
            bool status = false;
            if (estimateQuoDao == null)
            {
                estimateQuoDao = new EstimateQuotationDAO();
            }
            dtItemDetails = estimateQuoDao.CheckProductExistance(itemCode);
            if (dtItemDetails != null && dtItemDetails.Rows.Count > 0)
            {
                if ((itemCode == dtItemDetails.Rows[0]["ItemCode"].ToString())
                    && (txtItemName.Text.Trim() == dtItemDetails.Rows[0]["ItemDescription"].ToString().Trim()))
                {
                    status = true;
                }
            }

            return status;
        }

        /// <summary>
        /// validate the qty
        /// </summary>
        /// <returns></returns>
        protected bool ValidateQunatity()
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

            double finalQty = Convert.ToDouble(hfApprovedQty.Value);
            double issuedQty = Convert.ToDouble(hfIssuedQty.Value);
            double requestedQty = Convert.ToDouble(txtRequestQty.Text.Trim());

            double qty = Convert.ToDouble(txtQty.Text.Trim());
            if (finalQty > 0)  // if (finalQty != -1 || finalQty != 0.0)  
            {
                if (qty > (finalQty - (issuedQty + requestedQty)))
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Check item already contains in the dataTable
        /// </summary>
        protected bool CheckItemStatus(DataTable dt, string itemId)
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
                DataRow dr = dt.NewRow();

                dr[COLUMN_ITEM_CODE] = itemId;
                dr[COLUMN_ITEM_NAME] = itemName;
                dr[COLUMN_ITEM_QTY] = Convert.ToDouble(qty);
                dr[COLUMN_ITEM_REMARKS] = remarks;
                dr[COLUMN_ITEM_ORDER] = dt.Rows.Count + 1;
                dr[COLUMN_ITEM_UOM] = uom;

                dt.Rows.Add(dr);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ResetForm()
        {
            txtItemId.Text = string.Empty;
            txtItemName.Text = string.Empty;
            txtUnitofMeasure.Text = string.Empty;
            txtRequestQty.Text = string.Empty;
            txtQty.Text = string.Empty;
            txtRemarks.Text = string.Empty;
        }

        /// <summary>
        /// Reset the session values 
        /// </summary>
        protected void ResetSession()
        {
            Session[SESSION_ITEM_TABLE] = null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void DisableForm()
        {
            txtEstimationId.Enabled = false;
            dtApplyDate.Enabled = false;
            txtComments.Enabled = false;
            txtDeliverLocation.Enabled = false;

            txtItemId.Enabled = false;
            txtItemName.Enabled = false;
            txtQty.Enabled = false;
            txtUnitofMeasure.Enabled = false;
            btnAdd.Enabled = false;
            txtRemarks.Enabled = false;


            btnSubmitForApproval.Visible = false;
            btnSave.Visible = false;

            //grdItems.Columns[5].Visible		= false;
            grdItems.Columns[6].Visible = false;

        }

        /// <summary>
        /// Generate the email body
        /// </summary>
        protected string GenerateEmailBody(string siteName, string jobNumber, string EngineerName, DateTime date, string mrNumber)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine(string.Format("Site : {0}", siteName));
            message.AppendLine(string.Format("Job No : {0}", jobNumber));
            message.AppendLine(string.Format("Engineer : {0}", EngineerName));
            message.AppendLine(string.Format("Date : {0}", date));
            message.AppendLine(string.Format("Web S.Estimate No : {0}", mrNumber));

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
                    FormMode = (Mode)Session[SESSION_FORM_MODE];
                }
                else
                {
                    Response.Redirect("~/Home.aspx");
                }
            }

            error_alert.Visible = false;
            success_alert.Visible = false;
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
        /// Handles the add button click
        /// </summary>
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
            {
                dtItems = (DataTable)Session[SESSION_ITEM_TABLE];
            }
            else
            {
                dtItems = this.GenerateDataTable();
            }

            if (Validations())
            {
                string itemId = txtItemId.Text.Trim();
                string itemName = txtItemName.Text.Trim();
                string itemQty = txtQty.Text.Trim();
                string itemRemarks = txtRemarks.Text.Trim();
                string itemUom = txtUnitofMeasure.Text.Trim();

                if (Session["EditRow"] != null)
                {
                    DataRow dr = (DataRow)Session["EditRow"];

                    if (this.CheckItemStatus(dtItems, txtItemId.Text.Trim())
                        && txtItemId.Text.Trim() == dr[COLUMN_ITEM_CODE].ToString().Trim())
                    {
                        if (dr.ItemArray.Length > 0)
                        {
                            int rowIndex = dtItems.Rows.IndexOf(dr);

                            dtItems.PrimaryKey = new DataColumn[1] { dtItems.Columns["ItemCode"] };  // set your primary key
                            DataRow dRow = dtItems.Rows.Find(dr[COLUMN_ITEM_CODE]);

                            dRow[COLUMN_ITEM_CODE] = itemId;
                            dRow[COLUMN_ITEM_NAME] = itemName;
                            dRow[COLUMN_ITEM_QTY] = Convert.ToDouble(itemQty);
                            dRow[COLUMN_ITEM_REMARKS] = itemRemarks;
                            dRow[COLUMN_ITEM_UOM] = itemUom;

                            this.ResetErrors();
                            hfEditingItemNo.Value = string.Empty;

                        }
                    }
                    else
                    {
                        SetDuplicateItemMessage(dtItems, itemId);
                    }
                }
                else
                {
                    if (!CheckItemStatus(dtItems, txtItemId.Text.Trim()))
                    {
                        AddRows(itemId, itemName, itemQty, itemRemarks, itemUom, dtItems);
                    }
                    else
                    {
                        SetDuplicateItemMessage(dtItems, itemId);
                    }
                }


                txtTotalItem.Text = dtItems.Rows.Count.ToString();

                this.ResetForm();
                this.txtItemName.Focus();

                txtItemId.ReadOnly = false;
                txtItemName.ReadOnly = false;

                Session["EditRow"] = null;
                Session[SESSION_ITEM_TABLE] = dtItems;

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
        /// Handles the item row command
        /// </summary>
        protected void grdItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == COMMAND_REMOVE)
            {
                if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
                {
                    dtItems = (DataTable)Session[SESSION_ITEM_TABLE];
                    if (dtItems.Rows.Count > 0)
                    {
                        dtItems.PrimaryKey = new DataColumn[1] { dtItems.Columns["ItemCode"] };  // set your primary key
                        DataRow dRow = dtItems.Rows.Find(e.CommandArgument);
                        if (dRow != null)
                        {
                            dtItems.Rows.Remove(dRow);

                            txtTotalItem.Text = dtItems.Rows.Count.ToString();
                            //set data table to session 
                            Session[SESSION_ITEM_TABLE] = dtItems;
                        }
                    }
                }
            }
            else if (e.CommandName == COMMAND_EDIT)
            {
                string index = e.CommandArgument.ToString();

                this.SetEditMode(index);
            }

            grdItems.DataSource = dtItems;
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
                    txtItemId.Text = dRow["ItemCode"].ToString();
                    txtItemName.Text = dRow["ItemName"].ToString();
                    txtQty.Text = dRow["Qty"].ToString();
                    txtRemarks.Text = dRow["Remarks"].ToString();
                    txtUnitofMeasure.Text = dRow["MainMeasure"].ToString();

                    //disable item name and code
                    txtItemId.ReadOnly = true;
                    txtItemName.ReadOnly = true;

                    //dtItems.Rows.Remove(dRow);
                    Session["EditRow"] = dRow;
                    Session[SESSION_ITEM_TABLE] = dtItems;

                }
            }
        }

        /// <summary>
        /// Handles the page index event
        /// </summary>
        protected void grdItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdItems.PageIndex = e.NewPageIndex;

            if ((DataView)Session[SESSION_ITEM_TABLE_FILTER] != null)
            {
                grdItems.DataSource = (DataView)Session[SESSION_ITEM_TABLE_FILTER]; ;
                grdItems.DataBind();
            }
            else if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
            {
                grdItems.DataSource = (DataTable)Session[SESSION_ITEM_TABLE];
                grdItems.DataBind();
            }
        }

        #endregion

        #region Properties
        public Mode FormMode { set; get; } = Mode.New;

        #endregion

        #region Web Methods

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string[] GetEstimationDetails(string prefix, int estimationId)
        {
            string[] itemList = null;

            EstimateQuotationDAO estimateQuotationDao = new EstimateQuotationDAO();

            itemList = estimateQuotationDao.GetEstimationItems(HttpUtility.UrlDecode(prefix), estimationId, customerCode, jobCode);

            return itemList;
        }

        [System.Web.Services.WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetEstimationItemByCode(string itemCode)
        {
            string itemList = null;

            MaterialRequestDAO materialRequestDao = new MaterialRequestDAO();

            itemList = materialRequestDao.GetEstimationItemById(itemCode);

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

        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
            {
                dtItems = (DataTable)Session[SESSION_ITEM_TABLE];
                DataView dtFilter = dtItems.AsDataView();

                dtFilter.RowFilter = "ItemName like '%" + txtSearchQuery.Text.Trim().Replace("'", "''") + "%'";
                if (dtFilter.ToTable().Rows.Count > 0)
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

        protected void btnClearSearch_OnClick(object sender, EventArgs e)
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