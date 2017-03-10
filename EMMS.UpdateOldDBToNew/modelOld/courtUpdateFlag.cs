using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMMS.UpdateOldDBToNew.modelOld
{
    /// <summary>
    /// courtUpdateFlag（案件表更新标记表)
    /// </summary>
    [Serializable]
    [PetaPoco.TableName("courtUpdateFlag")]
    [PetaPoco.PrimaryKey("courtid")]
    public class courtUpdateFlag
    {
        /// <summary>
        /// 法院ID,递增表,每次加1.
        /// </summary>
        public virtual DateTime Actiondate { get; set; }        

    }
}
