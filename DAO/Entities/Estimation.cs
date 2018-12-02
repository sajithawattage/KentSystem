using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAO.Entities
{
    public class Estimation
    {
		public int CustomerID { get; set;}
		public int jobID { get; set;}
        public int EstimationID { get; set; }
        public string Manager { get; set; }
        public string Engineer { get; set; }
        public DateTime Applydate { get; set; }
        public decimal TotalVal { get; set; }
		public string Status { get; set;}
		public int ManagerId { get; set;}
		public int EngineerId { get; set;}
    }
}
