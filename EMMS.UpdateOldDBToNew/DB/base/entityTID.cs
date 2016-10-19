using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMMS.UpdateOldDBToNew
{
    public abstract class entityTID : entity
    {
        public long Tid { get; set; }

    }
}
