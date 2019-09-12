using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteDB.Windows.Service.Context
{
    public class Log : GenericModel
    {
        public String Name { get; set; }        
        public DateTime? StartedOn { get; set; }
        public DateTime? FinishedOn { get; set; }
        public int? TotaBytes { get; set; }
        public int? SleepingMiliseconds { get; set; }
        public int? TotalMiliSeconds { get; set; }
        public int? RealJobMiliSeconds { get; set; }
    }
}
