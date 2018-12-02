using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAO.Entities
{
    public class EstimateQuotation
    {

        public int MrNumber { get; set; }
        public int MrBookNumber { get; set; }
        public int CustomerCode { get; set; }
        public int JobCode { get; set; }
        public string LocationOfDelivery { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Remarks { get; set; }
        public int MrNumberUpdateTo { get; set; }
        public string ManagerStatus { get; set; }
        public string EngineerStatus { get; set; }
        public int Id { get; set; }
        public int EngineerId { get; set; }
        public int ManagerId { get; set; }
    }
}
