using DAO;
using Microsoft.Reporting.WebForms;
using SLII_Web.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KentWebApplication.Reports
{
    public partial class SubEstimateViewer : System.Web.UI.Page
    {
        private EstimateQuotationDAO estimateQuotationDao = null;
        private EstimationDAO estimationDao = null;
        private DataTable dtItems = null;
        private int id = -1;

        protected DateTime requiredDate;
        protected DateTime orderDate;
        protected int mrNumber = -1;
        protected string Location = string.Empty;
        protected string EngineerName = string.Empty;
        protected string ManagerName = string.Empty;
        protected string remarks = string.Empty;
        protected string jobName = string.Empty;

        protected const string QUERY_ESTIMATION_CODE = "eid";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.ProcessQueryString();

                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Reports/SubEstimate.rdlc");

                this.GetMaterialRequestByMrNumber(id);

                ReportParameter[] paramter = new ReportParameter[8];

                paramter[0] = new ReportParameter("requiredDate", requiredDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));
                paramter[1] = new ReportParameter("mrNumber", id.ToString());
                paramter[2] = new ReportParameter("location", Location);
                paramter[3] = new ReportParameter("manager", ManagerName);
                paramter[4] = new ReportParameter("engineer", EngineerName);
                paramter[5] = new ReportParameter("remarks", remarks);
                paramter[6] = new ReportParameter("JobName", jobName);
                paramter[7] = new ReportParameter("OrderDate", orderDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));

                ReportDataSource datasource = new ReportDataSource("SubEstimate", dtItems);

                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(datasource);
                ReportViewer1.LocalReport.SetParameters(paramter);
            }
        }

        #region Methods

        private void ProcessQueryString()
        {
            id = General.GetQueryStringInt(Request.QueryString[QUERY_ESTIMATION_CODE]);
        }

        private void GetMaterialRequestByMrNumber(int mrNumber)
        {
            if (estimateQuotationDao == null)
            {
                estimateQuotationDao = new EstimateQuotationDAO();
            }

            DataTable dtMaterialRequest = estimateQuotationDao.GetMaterialRequestByMRNumber(mrNumber);
            if (dtMaterialRequest != null && dtMaterialRequest.Rows.Count > 0)
            {
                requiredDate = Convert.ToDateTime(dtMaterialRequest.Rows[0]["RequiredDate"].ToString());
                mrNumber = Convert.ToInt32(dtMaterialRequest.Rows[0]["MRNumber"].ToString());
                Location = dtMaterialRequest.Rows[0]["LocationOfDeliver"].ToString();
                EngineerName = dtMaterialRequest.Rows[0]["EngineerName"].ToString();
                ManagerName = dtMaterialRequest.Rows[0]["ManagerName"].ToString();
                remarks = dtMaterialRequest.Rows[0]["Remarks"].ToString();
                jobName = dtMaterialRequest.Rows[0]["CustomerName"].ToString() + " | " +
                                                          dtMaterialRequest.Rows[0]["JobName"].ToString();
                orderDate = Convert.ToDateTime(dtMaterialRequest.Rows[0]["ReceivedDate"].ToString());

                GetMaterialRequestDetails(Convert.ToInt32(dtMaterialRequest.Rows[0]["MRNumber"]));
            }
        }

        /// <summary>
        /// Get sub estimate details 
        /// </summary>
        private void GetMaterialRequestDetails(int materialRequestId)
        {
            if (estimateQuotationDao == null)
            {
                estimateQuotationDao = new EstimateQuotationDAO();
            }

            DataTable dtMaterialRequestItem = estimateQuotationDao.GetMaterialRequestItemByMaterialRequestId(materialRequestId);
            if (dtMaterialRequestItem != null && dtMaterialRequestItem.Rows.Count > 0)
            {
                dtItems = dtMaterialRequestItem;
            }
        }


        #endregion

    }
}