using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetPanamaDB.models
{
    /// <summary>
    /// 公司/地址/中介/人员
    /// </summary>
    [Serializable]
    [PetaPoco.TableName("EntitysAll")]
    [PetaPoco.PrimaryKey("Tid")]
    public class EntitysAll : modelTID
    {

        /// <summary>
        /// 新增标记，用于生成关系
        /// </summary>
        public long Entityid { get; set; }


        public string name { get; set; }

        public string nameDesc { get; set; }
        public string nameDescSearch { get; set; }

        public string nameURL { get; set; }
        /// <summary>
        /// 公司/地址/中介/人员
        /// Entity/Address/Intermediary/Officer
        /// </summary>
        public string ttype { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        //[Index(Order = 2)]
        //[StringLength(128)]
        public string Countries { get; set; }

        /// <summary>
        /// 共有
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 公司 有
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 地址 有
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 公司 有
        /// </summary>
        public string CompanyType { get; set; }
        public string DormDate { get; set; }
        /// <summary>
        /// 公司 有
        /// </summary>
        public string Jurisdiction { get; set; }


    }



    /// <summary>
    /// 公司/地址/中介/人员
    /// </summary>
    [Serializable]
    [PetaPoco.TableName("t_Panama")]
    [PetaPoco.PrimaryKey("PublicID",autoIncrement=true)]
    public class t_Panama 
    {

        /// <summary>
        /// 新增标记，用于生成关系
        /// </summary>
        public long PublicID { get; set; }


        public string Name_EN { get; set; }
        public string Name_CN { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        //[Index(Order = 2)]
        //[StringLength(128)]
        public string Countries { get; set; }

        /// <summary>
        /// 共有
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 公司 有
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 地址 有
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 公司 有
        /// </summary>
        public string CompanyType { get; set; }
        public string DormDate { get; set; }
        /// <summary>
        /// 公司 有
        /// </summary>
        public string Jurisdiction { get; set; }
        public string Remark { get; set; }
        public int Show { get; set; }
        public int DataGradeID { get; set; }
        public int Enable { get; set; }
        public int Language { get; set; }

        public long HtmlID { get; set; }

    }
}
