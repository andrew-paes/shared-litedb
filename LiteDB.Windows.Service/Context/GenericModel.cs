using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteDB.Windows.Service.Context
{
    public abstract class GenericModel
    {
        protected GenericModel() { }

        protected GenericModel(DateTime? createdDate,
                            DateTime? modifiedDate)
        {
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
        }

        public int Id { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}