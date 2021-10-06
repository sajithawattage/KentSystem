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

namespace KentWebApplication.Pages
{
    public partial class Estimation : System.Web.UI.Page
    {
        #region Constant

        private const string SESSION_ITEM_TABLE = "ITEM_TABLE";
        protected const string SESSION_ITEM_TABLE_FILTER = "ITEM_TABLE_FILTERED";
        private const string SESSION_CUSTOMER_ID = "CUSTOMER_ID";
        private const string SESSION_JOB_ID = "JOB_ID";
        private const string SESSION_FORM_MODE = "FORM_MODE";
        private const string SESSION_ESTIMATION_ID = "ESTIMATION_ID";
        private const string SESSION_ENGINEER_ID = "ENGINEER_ID";
        private const string SESSION_MANAGER_ID = "MANAGER_ID";

        private const string VIEWSTATE_EDIT_MODE = "IS_EDIT";

        private const string STATUS_SAVE = "ENTRY";
        private const string STATUS_FINISH = "FINISH";

        private const string QUERY_CUSTOMER_CODE = "cid";
        private const string QUERY_JOB_CODE = "jid";
        private const string QUERY_ESTIMATION_CODE = "eid";
        private const string QUERY_MANAGER_CODE = "mid";
        private const string QUERY_TYPE_CODE = "type";

        private const string COMMAND_REMOVE = "Remove";
        private const string COMMAND_EDIT = "Change";

        private const string COLUMN_ITEM_CODE = "ItemCode";
        private const string COLUMN_ITEM_NAME = "ItemName";
        private const string COLUMN_ITEM_QTY = "QTY";
        private const string COLUMN_ITEM_AMOUNT = "Amount";
        private const string COLUMN_ITEM_TOTAL = "Total";
        private const string COLUMN_ITEM_ORDER = "itemIndex";
        private const string COLUMN_ITEM_UOM = "MainMeasure";
        private const string COLUMN_ITEM_REMARKS = "Remarks";

        //protected const string COLUMN_ITEM_CODE = "ItemCode";
        //protected const string COLUMN_ITEM_NAME = "ItemName";
        //protected const string COLUMN_ITEM_QTY = "Qty";
        //protected const string COLUMN_ITEM_AMOUNT = "Amount";
        //protected const string COLUMN_ITEM_REMARKS = "Remarks";
        //protected const string COLUMN_ITEM_TOTAL = "Total";
        //protected const string COLUMN_ITEM_ORDER = "OrderNo";
        //protected const string COLUMN_ITEM_UOM = "MainMeasure";

        private const string COLUMN_ITEM_FINAL_QTY = "FinalQty";
        private const string COLUMN_ITEM_ISSUE_QTY = "IssuedQty";
        private const string COLUMN_ITEM_REQUEST_QTY = "RequestedQty";
        private const string COLUMN_ITEM_PENDING_QTY = "PendingQty";
        private const string COLUMN_ITEM_BALANCE_QTY = "BalanceQty";

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

        #region Member

        private DataTable dt = new DataTable();
        private DataRow dr = null;
        private EstimationDAO objEstimationDAO = null;

        private const string SESSION_PROJECT_NAME = "PROJECT_NAME";
        private const string SESSION_JOB_NAME = "JOB_NAME";
        private const string SESSION_ENGINEER_NAME = "ENGINEER_NAME";
        private const string SESSION_MANAGER_NAME = "MANAGER_NAME";
        private const string SESSION_MANAGER_EMAIL_ADDRESS = "MANAGER_EMAIL_ADDRESS";

        private int customerCode = -1;
        private int jobCode = -1;
        private int engineerCode = -1;
        private int managerCode = -1;
        private int status = -1;
        
        private Mode formMode = Mode.New;

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //get values from the query string
                this.PopulateQueryString();

                //reset session
                Session[SESSION_ITEM_TABLE] = null;
                Session[SESSION_ESTIMATION_ID] = null;

                //reset view state
                ViewState[VIEWSTATE_EDIT_MODE] = false;

                if (status != -1)
                {
                    this.InitForm();
                }

                //add values to session
                this.SetInitialSessionValues();

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

        }

        /// <summary>
        /// Get the values in the query to the member variables
        /// </summary>
        private void PopulateQueryString()
        {
            customerCode = General.GetQueryStringInt(Request.QueryString[QUERY_CUSTOMER_CODE]);
            jobCode = General.GetQueryStringInt(Request.QueryString[QUERY_JOB_CODE]);
            engineerCode = General.GetQueryStringInt(Request.QueryString[QUERY_ESTIMATION_CODE]);
            managerCode = General.GetQueryStringInt(Request.QueryString[QUERY_MANAGER_CODE]);
            status = General.GetQueryStringInt(Request.QueryString[QUERY_TYPE_CODE]);
        }

        /// <summary>
        /// Set the initail session values to the session object
        /// </summary>
        private void SetInitialSessionValues()
        {
            Session.Add(SESSION_CUSTOMER_ID, customerCode);
            Session.Add(SESSION_JOB_ID, jobCode);
            Session.Add(SESSION_MANAGER_ID, managerCode);
            Session.Add(SESSION_ENGINEER_ID, engineerCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmitforApproval_Click(object sender, EventArgs e)
        {
            if (this.Save(STATUS_FINISH))
            {
                Response.Redirect("~/Home.aspx", true);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.Save(STATUS_SAVE);
        }


        protected void grdClosedExams_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == COMMAND_REMOVE)
            {
                if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
                {
                    dt = (DataTable)Session[SESSION_ITEM_TABLE];
                    if (dt.Rows.Count > 0)
                    {
                        dt.PrimaryKey = new DataColumn[1] { dt.Columns["ItemCode"] };  // set your primary key
                        DataRow dRow = dt.Rows.Find(e.CommandArgument);
                        if (dRow != null)
                        {
                            dt.Rows.Remove(dRow);

                            txtTotalItem.Text = dt.Rows.Count.ToString();
                            //set data table to session 
                            Session[SESSION_ITEM_TABLE] = dt;
                        }
                    }
                }
            }
            else if (e.CommandName == COMMAND_EDIT)
            {
                var index = e.CommandArgument.ToString();
                this.SetEditMode(index);

            }

            grdItems.DataSource = dt;
            grdItems.DataBind();

        }

        private void SetEditMode(string index)
        {
            if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
            {
                dt = (DataTable)Session[SESSION_ITEM_TABLE];

                dt.PrimaryKey = new DataColumn[1] { dt.Columns["ItemCode"] };  // set your primary key

                DataRow dRow = dt.Rows.Find(index);

                if (dRow != null)
                {
                    txtItemId.Text = dRow["ItemCode"].ToString();
                    txtItemName.Text = dRow["ItemName"].ToString();
                    txtQty.Text = dRow["Qty"].ToString();
                    txtUnitofMeasure.Text = dRow["MainMeasure"].ToString();
                    txtRemarks.Text = dRow["Remarks"].ToString();

                    Session["EditRow"] = dRow;

                    txtItemId.ReadOnly = true;
                    txtItemName.ReadOnly = true;

                    Session[SESSION_ITEM_TABLE] = dt;

                    ViewState[VIEWSTATE_EDIT_MODE] = true;

                }
            }
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            string pName = grdItems.SelectedRow.Cells[1].Text;
        }

        /// <summary>
        /// handle button click event
        /// </summary>
        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
            {
                dt = (DataTable)Session[SESSION_ITEM_TABLE];
            }
            else
            {
                dt = this.GenerateDataTable();
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

                    if (this.CheckItemStatus(dt, txtItemId.Text.Trim())
                        && txtItemId.Text.Trim() == dr[COLUMN_ITEM_CODE].ToString().Trim())
                    {
                        if (dr.ItemArray.Length > 0)
                        {
                            int rowIndex = dt.Rows.IndexOf(dr);

                            dt.PrimaryKey = new DataColumn[1] { dt.Columns["ItemCode"] };  // set your primary key
                            DataRow dRow = dt.Rows.Find(dr[COLUMN_ITEM_CODE]);

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
                        SetDuplicateItemMessage(dt, itemId);
                    }
                }
                else
                {
                    if (!this.CheckItemStatus(dt, txtItemId.Text.Trim()))
                    {
                        AddRows(itemId, itemName, itemQty, itemRemarks, itemUom, dt);
                    }
                    else
                    {
                        SetDuplicateItemMessage(dt, itemId);
                    }
                }


                txtTotalItem.Text = dt.Rows.Count.ToString();

                this.ResetForm();
                this.txtItemName.Focus();

                txtItemId.ReadOnly = false;
                txtItemName.ReadOnly = false;

                Session["EditRow"] = null;
                Session[SESSION_ITEM_TABLE] = dt;

                grdItems.DataSource = dt;
                grdItems.DataBind();
            }
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
            DataColumn dcQtyOrder = new DataColumn("itemIndex", typeof(Int32));
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

        protected bool CheckItemStatus(DataTable dt, string itemId)
        {
            return dt.AsEnumerable().Any(row => itemId == row.Field<String>("ItemCode"));
        }

        /// <summary>
        /// 
        /// </summary>
        protected void grdItems_RowEditing(object sender, GridViewEditEventArgs e)
        {
            grdItems.EditIndex = e.NewEditIndex;

            grdItems.DataSource = (DataTable)Session[SESSION_ITEM_TABLE];
            grdItems.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void grdItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            grdItems.PageIndex = e.NewPageIndex;

            if ((DataView)Session[SESSION_ITEM_TABLE_FILTER] != null)
            {
                grdItems.DataSource = (DataView)Session[SESSION_ITEM_TABLE_FILTER];
                grdItems.DataBind();
            }
            else if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
            {
                grdItems.DataSource = (DataTable)Session[SESSION_ITEM_TABLE];
                grdItems.DataBind();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize form with the data
        /// </summary>
        protected void InitForm()
        {
            dtApplyDate.Text = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            switch (status)
            {
                case 1:
                default:
                    FormMode = Mode.New;

                    Session.Add(SESSION_FORM_MODE, FormMode);

                    this.LoadSiteDetails(customerCode, jobCode);

                    this.GenerateEstimationID();
                    //generate id for 

                    break;
                case 2:
                    FormMode = Mode.Saved;

                    Session.Add(SESSION_FORM_MODE, FormMode);

                    //get estimation header
                    this.LoadSiteDetails(customerCode, jobCode);

                    break;
                case 3: // view 
                    FormMode = Mode.Submitted;

                    Session.Add(SESSION_FORM_MODE, FormMode);

                    this.LoadSiteDetails(customerCode, jobCode);

                    //enable hidden columns
                    this.DisableForm();
                    break;
                case 4:
                    FormMode = Mode.Approved;
                    Session.Add(SESSION_FORM_MODE, FormMode);

                    this.LoadSiteDetails(customerCode, jobCode);

                    this.DisableForm();

                    break;
            }

        }

        /// <summary>
        /// load site details
        /// </summary>
        private void LoadSiteDetails(int customerId, int jobId)
        {
            DataTable dtSiteDetails = null;

            if (objEstimationDAO == null)
            {
                objEstimationDAO = new EstimationDAO();
            }

            dtSiteDetails = objEstimationDAO.GetSiteDetailsByJobCustomerCode(customerId, jobId, engineerCode);

            if (dtSiteDetails != null && dtSiteDetails.Rows.Count > 0)
            {
                txtManager.Text = dtSiteDetails.Rows[0]["ManagerName"].ToString();
                txtEngineer.Text = dtSiteDetails.Rows[0]["EnginnerName"].ToString();

                litProjectName.Text = $"{dtSiteDetails.Rows[0]["CustomerName"].ToString()} | {dtSiteDetails.Rows[0]["JobName"].ToString()}";

                Session[SESSION_PROJECT_NAME] = dtSiteDetails.Rows[0]["CustomerName"].ToString();
                Session[SESSION_JOB_NAME] = dtSiteDetails.Rows[0]["JobName"].ToString();
                Session[SESSION_MANAGER_NAME] = dtSiteDetails.Rows[0]["ManagerName"].ToString();
                Session[SESSION_ENGINEER_NAME] = dtSiteDetails.Rows[0]["EnginnerName"].ToString();
                Session[SESSION_MANAGER_EMAIL_ADDRESS] = dtSiteDetails.Rows[0]["ManagerEmailAddress"].ToString();


                var dtEstimationHeader = objEstimationDAO.GetEstimationHeader(customerId, jobId, engineerCode.ToString());
                if (dtEstimationHeader != null && dtEstimationHeader.Rows.Count > 0)
                {
                    txtEstimationId.Text = dtEstimationHeader.Rows[0]["EstimateNo"].ToString();
                    dtApplyDate.Text = Convert.ToDateTime(dtEstimationHeader.Rows[0]["EntryDate"])
                                                                            .ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

                    //set estimation value
                    Session[SESSION_ESTIMATION_ID] = dtEstimationHeader.Rows[0]["EstimateNo"].ToString();

                    var dtEstimationDetails = objEstimationDAO.GetEstimationDetails(Convert.ToInt32(txtEstimationId.Text.Trim()));

                    if (dtEstimationDetails != null && dtEstimationDetails.Rows.Count > 0)
                    {
                        txtTotalItem.Text = dtEstimationDetails.Rows.Count.ToString();

                        //set to session
                        Session[SESSION_ITEM_TABLE] = dtEstimationDetails;

                        grdItems.DataSource = dtEstimationDetails;
                        grdItems.DataBind();
                    }
                }
            }
        }

        /// <summary>
        /// Reset the form
        /// </summary>
        private void ResetForm()
        {
            txtItemId.Text = string.Empty;
            txtItemName.Text = string.Empty;
            txtQty.Text = string.Empty;
            txtUnitofMeasure.Text = string.Empty;
            txtRemarks.Text = string.Empty;
        }

        /// <summary>
        /// Add the rows
        /// </summary>
        private void AddRows(string ItemId, string ItemName, string QTY, string uom, string remarks)
        {
            if (dt != null)
            {
                dr = dt.NewRow();

                dr[COLUMN_ITEM_CODE] = ItemId;
                dr[COLUMN_ITEM_NAME] = ItemName;
                dr[COLUMN_ITEM_QTY] = QTY;
                dr[COLUMN_ITEM_ORDER] = dt.Rows.Count + 1;
                dr[COLUMN_ITEM_UOM] = uom;
                dr[COLUMN_ITEM_REMARKS] = remarks;

                dr[COLUMN_ITEM_FINAL_QTY] = "0";
                dr[COLUMN_ITEM_ISSUE_QTY] = "0";
                dr[COLUMN_ITEM_REQUEST_QTY] = "0";
                dr[COLUMN_ITEM_PENDING_QTY] = "0";
                dr[COLUMN_ITEM_BALANCE_QTY] = "0";

                dt.Rows.Add(dr);
            }
        }

        /// <summary>
        /// validate the form
        /// </summary>
        private bool ValidateForm()
        {
            bool result = true;

            if (Session[SESSION_CUSTOMER_ID] == null)
            {
                result = false;
            }
            else if (Session[SESSION_JOB_ID] == null)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Set message 
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

        private void ResetErrors()
        {
            success_alert.Visible = false;
            error_alert.Visible = false;
            duplicate_alert.Visible = false;

            litSuccessMessage.Text = string.Empty;
            litErrorMessage.Text = string.Empty;
            litMessage.Text = string.Empty;

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


        /// <summary>
        /// save the form values
        /// </summary>
        private bool Save(string type)
        {
            var status = false;
            try
            {
                if (this.ValidateForm())
                {
                    DAO.Entities.Estimation objEstimation = new DAO.Entities.Estimation
                    {
                        CustomerID = Convert.ToInt32(Session[SESSION_CUSTOMER_ID].ToString()),
                        jobID = Convert.ToInt32(Session[SESSION_JOB_ID].ToString()),
                        Manager = txtManager.Text.Trim(),
                        Engineer = txtEngineer.Text.Trim(),
                        Applydate = Convert.ToDateTime(dtApplyDate.Text.Trim()),
                        EngineerId = Convert.ToInt32(Session[SESSION_ENGINEER_ID].ToString()),
                        ManagerId = Convert.ToInt32(Session[SESSION_MANAGER_ID].ToString()),
                        Status = type
                    };


                    if (objEstimationDAO == null)
                    {
                        objEstimationDAO = new EstimationDAO();
                    }

                    if (FormMode == Mode.New)
                    {
                        //get new estimation id
                        objEstimation.EstimationID = Convert.ToInt32(this.GenerateEstimationID());
                        status = objEstimationDAO.SaveEstimation(objEstimation, (DataTable)Session[SESSION_ITEM_TABLE]);
                    }
                    else if (FormMode == Mode.Saved)
                    {
                        //get old estimation id
                        if (Session[SESSION_ESTIMATION_ID] != null)
                        {
                            objEstimation.EstimationID = Convert.ToInt32(Session[SESSION_ESTIMATION_ID].ToString());
                            status = objEstimationDAO.UpdateEstimation(objEstimation, (DataTable)Session[SESSION_ITEM_TABLE]);
                        }
                    }

                    if (status)
                    {
                        if (type == STATUS_FINISH)
                        {
                            //send email
                            Email email = new Email()
                            {
                                HostAddress = Config.SmtpHostName,
                                FromAddress = Config.SmtpUserName,
                                SmtpPassword = Config.SmtpPassword,
                                ToAddress = Session[SESSION_MANAGER_EMAIL_ADDRESS].ToString(),
                                Subject = "New Estimate Received",
                                Body = GenerateEmailBody(
                                        Session[SESSION_PROJECT_NAME].ToString(),
                                        Session[SESSION_JOB_NAME].ToString(),
                                        Session[SESSION_ENGINEER_NAME].ToString(),
                                        objEstimation.Applydate,
                                        objEstimation.EstimationID.ToString())

                            };

                            if (email.SendMail())
                            {
                                this.SetMessage(1, "Estimate Successfully saved.");
                            }
                            else
                            {
                                this.SetMessage(1, "Estimate Successfully saved.But email sending failed.");
                            }
                        }
                        else
                        {
                            this.SetMessage(1, "Successfully saved.");
                        }

                        Session[SESSION_FORM_MODE] = Mode.Saved;
                        Session[SESSION_ESTIMATION_ID] = objEstimation.EstimationID;

                        FormMode = Mode.Saved;
                    }
                    else
                    {
                        this.SetMessage(2, "Error occurred while saving data");
                    }
                }

            }
            catch (Exception ex)
            {
                this.SetMessage(2, ex.Message);
            }

            return status;

        }

        /// <summary>
        /// automatically generates an id for the estimation
        /// </summary>
        protected string GenerateEstimationID()
        {
            string estimationId = string.Empty;
            if (objEstimationDAO == null)
            {
                objEstimationDAO = new EstimationDAO();
            }

            estimationId = objEstimationDAO.GetNextEstimationId();
            if (estimationId != string.Empty)
            {
                txtEstimationId.Text = estimationId;
            }
            return estimationId;
        }

        /// <summary>
        /// disable the form values
        /// </summary>
        protected void DisableForm()
        {
            txtEstimationId.Enabled = false;
            dtApplyDate.Enabled = false;

            txtItemId.Enabled = false;
            txtItemName.Enabled = false;
            txtQty.Enabled = false;
            txtUnitofMeasure.Enabled = false;
            btnAdd.Enabled = false;

            btnSubmitForApproval.Enabled = false;
            btnSave.Enabled = false;

            //grdItems.Enabled				= false;

        }

        /// <summary>
        /// validations 
        /// </summary>
        /// <returns></returns>
        protected bool Validations()
        {
            if (txtItemId.Text.Trim() == string.Empty)
            {
                return false;
            }
            else if (txtItemName.Text.Trim() == string.Empty)
            {
                return false;
            }
            else if (txtQty.Text.Trim() == string.Empty)
            {
                return false;
            }
            else if (Validation.IsNumeric(txtQty.Text.Trim()))
            {
                return true;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Generate the email body
        /// </summary>
        protected string GenerateEmailBody(string siteName, string jobNumber, string EngineerName, DateTime date, string estimateNo)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine(string.Format("Site : {0}", siteName));
            message.AppendLine(string.Format("Job No : {0}", jobNumber));
            message.AppendLine(string.Format("Engineer : {0}", EngineerName));
            message.AppendLine(string.Format("Date : {0}", date));
            message.AppendLine(string.Format("Estimation No : {0}", estimateNo));

            return message.ToString();
        }

        #endregion

        #region Web Methods

        [System.Web.Services.WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string[] GetAllItems(string prefix)
        {
            EstimationDAO objEstimationDAO = new EstimationDAO();

            return objEstimationDAO.GetAllItems(HttpUtility.UrlDecode(prefix), 1);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Mode of the form
        /// </summary>
        public Mode FormMode
        {
            set { this.formMode = value; }
            get { return this.formMode; }
        }

        #endregion

        protected void grdItems_DataBound(object sender, EventArgs e)
        {
            if (FormMode == Mode.New || FormMode == Mode.Saved)
            {
                grdItems.Columns[5].Visible = false;
                grdItems.Columns[6].Visible = false;
                grdItems.Columns[7].Visible = false;
            }
        }

        protected void txtQty_TextChanged1(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtQty.Text, out decimal value))
            // It's a decimal
            { }
            else
            {
                txtQty.Text = "0";
            }
            // No it's not
        }

        protected void btnMsg_OnClick(object sender, EventArgs e)
        {
            var index = hfEditingItemNo.Value;

            this.SetEditMode(index);
        }

        protected void btnClearSearch_OnClick(object sender, EventArgs e)
        {
            if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
            {
                lblFilterSummary.Text = "";
                pnlFilterSummary.Visible = false;

                grdItems.DataSource = (DataTable)Session[SESSION_ITEM_TABLE];
                grdItems.DataBind();
            }
        }

        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            if ((DataTable)Session[SESSION_ITEM_TABLE] != null)
            {
                dt = (DataTable)Session[SESSION_ITEM_TABLE];
                DataView dtFilter = dt.AsDataView();

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

                grdItems.DataSource = dtFilter;
                grdItems.DataBind();
            }
        }
    }
}