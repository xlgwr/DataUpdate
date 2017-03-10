using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetPanamaDB.models
{
    /// <summary>
    /// Intermediary(中介):获取所有
    /// Entity:获取所有 Officer（Shareholder of）记录
    /// Address: 获取所有
    /// </summary>
    [Serializable]
    [PetaPoco.TableName("Connections")]
    [PetaPoco.PrimaryKey("Tid")]
    public class Connections : modelTID
    {
        /// <summary>
        /// 中介/人员
        /// Intermediary/Officer
        /// </summary>
        public string nameFrom { get; set; }
        public string nameFromDesc { get; set; }
        public string nameFromURL { get; set; }

        public string nameType { get; set; }

        /// <summary>
        /// 公司/地址
        /// Entity/Address
        /// </summary>
        public string nameTo { get; set; }
        public string nameToDesc { get; set; }
        public string nameToURL { get; set; }

    }
    public class ConnectionsVM : Connections
    {
        public long EntityidTo { get; set; }
        public string TypeTo { get; set; }
        public long EntityidFrom { get; set; }
        public string TypeFrom { get; set; }
    }
}
