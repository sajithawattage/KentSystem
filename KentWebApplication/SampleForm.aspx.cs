using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KentWebApplication
{
    public partial class SampleForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //var info = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            //DateTimeOffset localServerTime = DateTimeOffset.Now;

            //DateTimeOffset istambulTime = TimeZoneInfo.ConvertTime(localServerTime, info);

            //Label1.Text = istambulTime.ToString();


            DateTime currentTime = DateTime.UtcNow;

            //Label1.Text = currentTime.LocalDateTime.ToString();

            string displayName = "(GMT+05:30) Sri Jaywardanapura Time";
            string standardName = "Sri Lanka Time";
            TimeSpan offset = new TimeSpan(05, 30, 00);
            TimeZoneInfo mawson = TimeZoneInfo.CreateCustomTimeZone(standardName, offset, displayName, standardName);

            DateTime localTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, mawson);
            Label1.Text = localTime.ToString();
        }

        
    }
}