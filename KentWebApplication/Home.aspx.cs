using DAO;
using SLII_Web.Classes;
using System;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace KentWebApplication
{
    public partial class Home : ParentPage
    {
        #region Member

        private string userName = string.Empty;
        private HomeDAO homeDao = null;

        #endregion

        #region Constant

        protected const string ROLE_ADMIN = "1";
        protected const string ROLE_MANAGER = "2";
        protected const string ROLE_ENGINEER = "3";

        protected const string URL_ESTIMATION = "Estimation.aspx?jid={0}&cid={1}&type={2}&eid={3}&mid={4}";
        protected const string URL_MATERIAL_REQUEST = "MaterialRequest.aspx?jid={0}&cid={1}&eid={2}&tid={3}";
        protected const string URL_MATERIAL_LIST = "MaterialRequestlist.aspx?jid={0}&cid={1}&eid={2}";

        protected const string URL_QUOTATION_ESTIMATE = "EstimateQuotation.aspx?jid={0}&cid={1}&eid={2}&tid={3}";
        protected const string URL_QUOTATION_ESTIMATE_LIST = "EstimateQuotationList.aspx?jid={0}&cid={1}&eid={2}";


        #endregion

        #region Event

        protected void Page_Load(object sender, EventArgs e)
        {

            if (homeDao == null)
            {
                homeDao = new HomeDAO();
            }

            if (!IsPostBack)
            {
                userName = this.Context.User.Identity.Name;
                if (userName != string.Empty)
                {
                    this.BindSitesByUser(userName);

                    this.GetUserStatistics(userName);

                    //disable SubEstimate

                }
                else
                {
                    Response.Redirect("~/Login.aspx");
                }
            }
        }

        protected void hlSignOut_Click(object sender, EventArgs e)
        {
            HttpCookie dummyCookie = new HttpCookie(FormsAuthentication.FormsCookieName, String.Empty);

            // Add cookie to response, and do not cache it.
            Response.Cookies.Add(dummyCookie);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            // Sign out of forms authentication.
            FormsAuthentication.SignOut();

            // Abandon the session
            Session.Abandon();

            // Redirect to the home page (which FormsAuthentication should pick up, and redirect to the login).
            Response.Redirect("Login.aspx");
        }

        protected void rpSites_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            HyperLink hlEstimate = (HyperLink)e.Item.FindControl("hlEstimation");
            HyperLink hlMaterialRequest = (HyperLink)e.Item.FindControl("hlMaterialRequest");
            HyperLink hlMaterialRequestList = (HyperLink)e.Item.FindControl("hlMaterialRequestList");
            HyperLink hlQuotationEstimate = (HyperLink)e.Item.FindControl("hlQuotaionEstimate");
            HyperLink hlQuotationEstimateList = (HyperLink)e.Item.FindControl("hlQuotaionEstimateList");

            HiddenField hfCustomerCode = (HiddenField)e.Item.FindControl("hfCId");
            HiddenField hfJobCode = (HiddenField)e.Item.FindControl("hfJId");
            HiddenField hfEngineerCode = (HiddenField)e.Item.FindControl("hfEId");
            HiddenField hfManagerCode = (HiddenField)e.Item.FindControl("hfMId");
            HiddenField hfEstimationType = (HiddenField)e.Item.FindControl("hfType");
            HiddenField hfManagerStatus = (HiddenField)e.Item.FindControl("hfMStatus");
            HiddenField hfEngineerStatus = (HiddenField)e.Item.FindControl("hfEStatus");
            HiddenField hfEstimateManagerStatus = (HiddenField)e.Item.FindControl("hfESMStatus");
            HiddenField hfEstimateEngineerStatus = (HiddenField)e.Item.FindControl("hfESEStatus");
            HiddenField hfMrActive = (HiddenField)e.Item.FindControl("hfMra");

            if (hlEstimate != null && hfCustomerCode != null && hfJobCode != null &&
                    hfEngineerCode != null && hfManagerCode != null && hfEstimationType != null)
            {
                string formattedUrl = string.Format(URL_ESTIMATION, new object[]{
                                                                        hfJobCode.Value.ToString(),
                                                                        hfCustomerCode.Value.ToString(),
                                                                        hfEstimationType.Value.ToString(),
                                                                        hfEngineerCode.Value.ToString(),
                                                                        hfManagerCode.Value.ToString()
                                                                    });

                hlEstimate.NavigateUrl = formattedUrl;
                hlEstimate.Text = this.GetLinkNameByType(Convert.ToInt32(hfEstimationType.Value));
            }

            if (hlMaterialRequest != null && hlMaterialRequestList != null)
            {

                //get material request list url
                hlMaterialRequestList.NavigateUrl = string.Format(URL_MATERIAL_LIST, new object[]{
                                                                        hfJobCode.Value.ToString(),
                                                                        hfCustomerCode.Value.ToString(),
                                                                        hfEngineerCode.Value.ToString()
                                                                    });


                this.HandleMRButtonBehaviour(hfJobCode.Value.ToString(), hfCustomerCode.Value.ToString(), hfEngineerCode.Value.ToString(),
                                                            hlMaterialRequest, hfEngineerStatus.Value, hfManagerStatus.Value, hfMrActive.Value);

                this.HandleSubEstimationButtonBehaviour(hfJobCode.Value.ToString(), hfCustomerCode.Value.ToString(), hfEngineerCode.Value.ToString(),
                                                            hlQuotationEstimate, hfEstimateEngineerStatus.Value, hfEstimateManagerStatus.Value, Convert.ToInt32(hfEstimationType.Value));

                this.HandleSubEstimationListButtonBehaviour(hfJobCode.Value.ToString(), hfCustomerCode.Value.ToString(), hfEngineerCode.Value.ToString(), hlQuotationEstimateList);

            }
        }

        /// <summary>
        /// Handles the behaviour of the estimation button
        /// </summary>
        private void HandleMRButtonBehaviour(string jobCode, string customerCode, string engineerCode, HyperLink hlMaterialRequest, string engineerState, string managerState, string mrActive)
        {
            if (mrActive == "Y")
            {
                hlMaterialRequest.Enabled = true;

                //get material request type
                int status = this.GetMRTypeByStatus(this.ProcessEngineerStatus(engineerState), this.ProcessManagerStatus(managerState));

                //get new material request url
                hlMaterialRequest.NavigateUrl = string.Format(URL_MATERIAL_REQUEST, new object[]{
                                                                        jobCode,
                                                                        customerCode,
                                                                        engineerCode,
                                                                        status.ToString()
                                                                    });

                this.GetMRLinkNameByType(this.ProcessEngineerStatus(engineerState), this.ProcessManagerStatus(managerState), hlMaterialRequest);
            }
            else
            {
                hlMaterialRequest.Enabled = false;
            }
        }

        /// <summary>
        /// handles the sub estimation button click event
        /// </summary>
        private void HandleSubEstimationButtonBehaviour(string jobCode, string customerCode, string engineerCode, HyperLink hlQuotationEstimate, string engineerState, string managerState, int estimateState)
        {
            if (estimateState == 3 || estimateState == 4)
            {
                if (DisableSubEstimate(Convert.ToInt32(customerCode), Convert.ToInt32(jobCode), engineerCode))
                {
                    hlQuotationEstimate.Enabled = false;
                }
                else
                {
                    int estimateStatus = this.GetESTypebyStatus(this.ProcessEngineerStatus(engineerState), this.ProcessManagerStatus(managerState));

                    hlQuotationEstimate.NavigateUrl = string.Format(URL_QUOTATION_ESTIMATE, new object[]{ jobCode, customerCode, engineerCode,
                                                                                                    estimateStatus.ToString() });

                    GetSubEstimateLinkNameByType(this.ProcessEngineerStatus(engineerState), this.ProcessManagerStatus(managerState), hlQuotationEstimate);
                }
            }
            else
            {
                hlQuotationEstimate.Enabled = false;
            }
        }

        /// <summary>
        /// get the name of the material request by manager and engineer status
        /// </summary>
        private void GetSubEstimateLinkNameByType(string eType, string mType, HyperLink hlSubEstimate)
        {
            hlSubEstimate.Text = "New Sub Estimate";

            if (eType == "NEW" && mType == "NEW")
            {
                hlSubEstimate.Text = "New Sub Estimate";
                hlSubEstimate.CssClass = "btn bg-olive margin";
            }
            else if (eType == "ENTRY" && mType == "NEW")
            {
                hlSubEstimate.Text = "Edit Sub Estimate";
                hlSubEstimate.CssClass = "btn bg-maroon margin";
            }
        }

        /// <summary>
        /// handles the sub estimate list button click
        /// </summary>
        private void HandleSubEstimationListButtonBehaviour(string jobCode, string customerCode, string engineerCode, HyperLink hlQuotationEstimateList)
        {
            hlQuotationEstimateList.NavigateUrl = string.Format(URL_QUOTATION_ESTIMATE_LIST, new object[] { jobCode, customerCode, engineerCode });
        }

        #endregion

        #region Methods

        public void BindSitesByUser(string userName)
        {
            DataTable dtSites = null;

            string empCode = homeDao.GetEmployeeCodeByUserName(userName);

            if (empCode != string.Empty)
            {
                if (User.IsInRole(ROLE_ENGINEER))
                {
                    dtSites = this.GetSitesByEngineer(empCode);

                    if (dtSites != null && dtSites.Rows.Count > 0)
                    {
                        rpSites.DataSource = dtSites;
                        rpSites.DataBind();
                    }

                }
                else if (User.IsInRole(ROLE_MANAGER))
                {
                    dtSites = this.GetSitesByManager(empCode);

                    if (dtSites != null && dtSites.Rows.Count > 0)
                    {
                        rpSites.DataSource = dtSites;
                        rpSites.DataBind();
                    }
                }
            }
        }

        public void GetUserStatistics(string userName)
        {
            DataTable dtDetails = homeDao.GetUserStatistics(userName);
            if (dtDetails != null && dtDetails.Rows.Count > 0)
            {
                litUserName.Text = dtDetails.Rows[0]["EngineerName"].ToString();
            }
        }

        public bool DisableSubEstimate(int customerId, int jobId, string engineerId)
        {
            var result = false;
            var sEstimateCount = homeDao.GetSEstimateCount(customerId, jobId);
            EstimationDAO estimateDao = new EstimationDAO();

            DataTable dtEstimateHeader = estimateDao.GetEstimationHeader(customerId, jobId, engineerId);
            if (dtEstimateHeader != null && dtEstimateHeader.Rows.Count > 0)
            {
                var maxSubEstimates = Convert.ToInt32(dtEstimateHeader.Rows[0]["MaxSubEstimates"].ToString());
                if (sEstimateCount >= maxSubEstimates)
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        protected DataTable GetSitesByManager(string empCode)
        {
            DataTable dtSites = homeDao.GetSitesByManager(empCode);
            return dtSites;

        }

        /// <summary>
        /// Get the site engineer details by employee code
        /// </summary>
        protected DataTable GetSitesByEngineer(string empCode)
        {
            DataTable dtSites = homeDao.GetSitesByEngineer(empCode);
            return dtSites;
        }

        /// <summary>
        /// get the link name by the type of the link. This will display in the fornt end
        /// </summary>
        protected string GetLinkNameByType(int type)
        {
            switch (type)
            {
                case 1:
                default:
                    return "New Estimate";
                case 2:
                    return "Edit Estimate";
                case 3:
                    return "View Estimate";
                case 4:
                    return "View Estimate";
            }
        }

        /// <summary>
        /// get the name of the material request by manager and engineer status
        /// </summary>
        protected void GetMRLinkNameByType(string eType, string mType, HyperLink hlMaterialRequest)
        {
            hlMaterialRequest.Text = "New Material Request";

            if (eType == "NEW" && mType == "NEW")
            {
                hlMaterialRequest.Text = "New Material Request";
                hlMaterialRequest.CssClass = "btn bg-olive margin";
            }
            else if (eType == "ENTRY" && mType == "NEW")
            {
                hlMaterialRequest.Text = "Edit Material Request";
                hlMaterialRequest.CssClass = "btn bg-maroon margin";
            }
        }

        /// <summary>
        /// Get Material request type according to the engineer and manager status
        /// </summary>
        protected int GetMRTypeByStatus(string eType, string mType)
        {
            int name = 0;

            if ((eType == "ENTRY" && mType == "NEW") || (eType == "CONFIRM" && mType == "NEW"))
            {
                name = 1;
            }

            return name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eType"></param>
        /// <param name="mType"></param>
        /// <returns></returns>
        protected int GetESTypebyStatus(string eType, string mType)
        {
            int name = 0;

            if (eType == "NEW" && mType == "NEW")
            {
                name = 0;
            }
            else if (eType == "ENTRY" && mType == "NEW")
            {
                name = 1;
            }

            return name;
        }

        /// <summary>
        /// process the manager state
        /// </summary>
        protected string ProcessManagerStatus(string statusValue)
        {
            if (statusValue == string.Empty)
            {
                statusValue = "NEW";
            }
            return statusValue;
        }

        protected string ProcessEngineerStatus(string statusValue)
        {
            if (statusValue == string.Empty)
            {
                statusValue = "NEW";
            }
            return statusValue;
        }
        #endregion

    }
}