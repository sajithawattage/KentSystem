using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAO;
using SLII_Web.Classes;

namespace KentWebApplication.Profile
{
	public partial class Profile : ParentPage
	{

		#region Member

		ProfileDAO			profileDao		= null;

		#endregion


		#region Event

		/// <summary>
		/// handles page load
		/// </summary>
		protected void Page_Load(object sender, EventArgs e)
		{
			
			
			if (!IsPostBack)
			{
				LoadData();

				this.dvErrorMessages.Visible	= false;
				this.dvSuccessMessages.Visible	= false;
			}
		}

		/// <summary>
		/// Handles change password button click event
		/// </summary>
		protected void btnUpdatePassword_Click(object sender, EventArgs e)
		{
			if (ValidatePassword())
			{
				string processedPassword			= string.Empty;

				if (profileDao == null)
				{
					profileDao						= new	ProfileDAO();
				}

				//update password
				processedPassword					= General.GetEncryptedValue(txtNewPassword.Text.Trim(), "AAECAwQFBgcICQoLDA0ODw==");

				profileDao.UpdatePasswordByUserName(UserName, processedPassword);

				dvSuccessMessages.Visible = true;
				litSuccessMessage.Text= "Password Successfully changed";

				txtNewPassword.Text = string.Empty;
				txtConfrimPassword.Text = string.Empty;

			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// validate password
		/// </summary>
		public bool ValidatePassword()
		{
			if (txtNewPassword.Text.Trim() == string.Empty)
			{
				this.dvErrorMessages.Visible = true;
				this.litSuccessMessage.Text = "New password cannot be empty";
				return false;
			}
			else if(txtConfrimPassword.Text.Trim() == string.Empty)
			{
				this.dvErrorMessages.Visible = true;
				this.litSuccessMessage.Text = "Please confirm the password";
				return false;
			}
			else if (txtNewPassword.Text.Trim() != txtConfrimPassword.Text.Trim())
			{
				this.dvErrorMessages.Visible = true;
				this.litSuccessMessage.Text = "New password and confirm password does not match.";
				return false;
			}
			else
			{
				this.dvErrorMessages.Visible = false;
				this.litSuccessMessage.Text = "";
				return true;
			}

		}

		public void LoadData()
		{
			if (profileDao == null)
			{
				profileDao						= new	ProfileDAO();
			}

			DataTable dtProfile					= profileDao.GetProfileDetails(UserName);
			if (dtProfile != null && dtProfile.Rows.Count > 0)
			{
				txtEngineerName.Text			= dtProfile.Rows[0]["EngineerName"].ToString();
				txtTelephone.Text				= dtProfile.Rows[0]["LoggingNIC"].ToString();
				txtEmailAddress.Text			= dtProfile.Rows[0]["PasswordHint"].ToString();
			}
		}

		#endregion

		protected void btnUpdateDetails_Click(object sender, EventArgs e)
		{
			if (profileDao == null)
			{
				profileDao						= new	ProfileDAO();
			}

			profileDao.UpdateProfileDetails(UserName, txtEmailAddress.Text.Trim(), txtTelephone.Text.Trim());

			dvSuccessMessages.Visible			= true;
			litSuccessMessage.Text				= "Profile details successfully changed";

		}

		

	}
}