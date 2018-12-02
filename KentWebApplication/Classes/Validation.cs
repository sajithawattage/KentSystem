using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KentWebApplication.Classes
{
	public class Validation
	{
		public static bool IsNumeric(string value)
		{
			bool	status			= false;
			double		outValue		= 0;

			status = Double.TryParse(value, out outValue);

			return status;
		}
		
		/// <summary>
		/// evaluate the value to a double or not
		/// </summary>
		public static bool IsDouble(string value)
		{
			bool	status			= false;
			double	outValue		= 0;

			status					= Double.TryParse(value, out outValue);

			return status;
		}

		/// <summary>
		/// 
		/// </summary>
		public static bool IsDate(string value)
		{
			bool		status		= false;
			DateTime	outValue	= Convert.ToDateTime("1753/01/01 12:00:00");

			status					= DateTime.TryParse(value, out outValue);

			return status;
		}
	}
}