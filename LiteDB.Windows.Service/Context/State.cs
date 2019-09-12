using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteDB.Windows.Service.Context
{
    public class State : GenericModel
    {
        public String Name { get; set; }
        public String Code { get; set; }
    }
}