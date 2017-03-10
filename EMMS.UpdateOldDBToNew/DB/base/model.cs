using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetPanamaDB.models
{
    public abstract class model
    {
        public string Remark { get; set; }
                
        public int getPage { get; set; }

        public int tStatus { get; set; }

        public string ClientIP { get; set; }

        public DateTime addDate { get; set; }

        public DateTime UpdateDate { get; set; }

    }
}
