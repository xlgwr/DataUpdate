using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EMMS.UpdateOldDBToNew
{
    public abstract class entityItems : entityTID
    {
        public long Language { get; set; }

        public string tkeyNo { get; set; }

        public long tIndex { get; set; }

        public string tname { get; set; }

        public string ttype { get; set; }


    }
}
