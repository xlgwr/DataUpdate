using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMMS.UpdateOldDBToNew
{
    public abstract class entityMain : entityTID
    {
        public long Language { get; set; }

        public string tname { get; set; }

        public string ttype { get; set; }

        public string thtml { get; set; }

    }
}
