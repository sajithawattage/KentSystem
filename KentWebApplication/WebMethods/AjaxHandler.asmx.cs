using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace SLII_Web.WebMethods
{
	/// <summary>
	/// Summary description for AjaxHandler
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[ScriptService]
	public class AjaxHandler : System.Web.Services.WebService
	{

		[WebMethod]	
		public string HelloWorld()
		{
			return "Hello World";
		}

	}
}
