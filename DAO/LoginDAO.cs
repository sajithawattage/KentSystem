
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DAO
{
	public class LoginDAO
	{

		#region Member

		DBConnection myConn = null;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public LoginDAO()
		{
			if(myConn == null)
			{
				myConn =new DBConnection();
			}
			
		}

		#endregion
		
		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public string GetPasswordByUserName(string userName)
		{
			string			status				= string.Empty;
			string			qCandidateStatus	= "sp_web_GetPasswordByUserName";
			SqlParameter[]	param				= null;

			try
			{
				param							= new SqlParameter[1];

				param[0]						= new SqlParameter("@username", userName);
			
				status							= Convert.ToString(myConn.ExecuteScalarProcedure(qCandidateStatus, param));

				return status;
 
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		#endregion
	
	}
}
