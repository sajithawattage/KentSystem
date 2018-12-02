using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Web.Configuration;

namespace DAO
{
	public static class Config
	{

		private const string SMTP_HOST			= "SmtpHostName";
		private const string SMTP_USER_NAME		= "SmtpUserName";
		private const string SMTP_PASSWORD		= "SmtpPassword";

		#region Properties

		public static string SmtpHostName => GetAppSettingString(SMTP_HOST);

	    public static string SmtpUserName => GetAppSettingString(SMTP_USER_NAME);

	    public static string SmtpPassword => GetAppSettingString(SMTP_PASSWORD);

	    #endregion

		/// <summary>
		/// 
		/// </summary>
		private static string GetAppSettingString(string key)
		{
			// Get the application settings
			NameValueCollection				appSettings				= WebConfigurationManager.AppSettings;

			// Get the setting with the key
			string							setting					= appSettings[key];

			// Check if null
			if ( setting == null )
			{
                // Throw exception
                throw new Exception("Cannot find the key : " + key);
			}

			return setting;
		}

		/// <summary>
		/// 
		/// </summary>
		public static string GetConnectionString(string configName)
		{
			var result = "";

			if ((ConfigurationManager.ConnectionStrings[configName] != null) &&
					(!string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[configName].ConnectionString)))
			{
				result = ConfigurationManager.ConnectionStrings[configName].ConnectionString;
			}
			else
			{
				throw new System.ArgumentException("Connection string with the key '" + configName + "' could not be found.");
			}

			return result;
		}

	}
}
