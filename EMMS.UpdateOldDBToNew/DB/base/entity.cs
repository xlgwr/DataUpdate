using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMMS.UpdateOldDBToNew
{
    public abstract class entity
    {
        public string Remark { get; set; }

        public int tStatus { get; set; }

        public string ClientIP { get; set; }

        public string adduser { get; set; }

        public string upduser { get; set; }

        public DateTime addtime { get; set; }

        public DateTime updtime { get; set; }

    }
}
