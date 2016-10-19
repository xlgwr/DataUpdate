using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMMS.UpdateOldDBToNew.modelOld
{
    /// <summary>
    /// cust_approved（前端帐号）,am_cust_approved（自动监察帐号）
    /// </summary>
    [Serializable]
    //[PetaPoco.TableName("cust_approved")]
    //[PetaPoco.PrimaryKey("acno")]
    public class custApprovedModel
    {
        /// <summary>
        /// 公司ID不要--》正式数据库要自动递增。
        /// </summary>
        public virtual string acno { get; set; }
        /// <summary>
        /// 公司类型 不要，用于区分个人与公司,Institute公司
        /// </summary>
        public virtual string actype { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public virtual string coname { get; set; }
        /// <summary>
        /// 公司编号-->营业号
        /// </summary>
        public virtual string corpid { get; set; }
        /// <summary>
        /// 性质-->业务类型
        /// </summary>
        public virtual string btitle { get; set; }

        /// <summary>
        /// 性别-->联系人
        /// </summary>
        public virtual string title { get; set; }
        /// <summary>
        /// 名--联系人
        /// </summary>
        public virtual string fname { get; set; }
        /// <summary>
        /// lname--联系人
        /// </summary>
        public virtual string lname { get; set; }
        /// <summary>
        /// 香港ID--联系人
        /// </summary>
        public virtual string hkid { get; set; }
        /// <summary>
        /// 电话--联系人
        /// </summary>
        public virtual string phone { get; set; }
        /// <summary>
        /// 传真--联系人
        /// </summary>
        public virtual string fax { get; set; }
        /// <summary>
        /// 电邮--联系人
        /// </summary>
        public virtual string email { get; set; }
        /// <summary>
        /// 地址--联系人
        /// </summary>
        public virtual string addr { get; set; }
        /// <summary>
        /// 注册ID--MemberName
        /// </summary>
        public virtual string login { get; set; }
        /// <summary>
        /// 密码--登录
        /// </summary>
        public virtual string pw { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string remarks { get; set; }
        /// <summary>
        /// 注册状态--会员状态
        /// </summary>
        public virtual int enablelogin { get; set; }
        /// <summary>
        /// 导入时设定为“支票”
        /// </summary>
        public virtual string paymentmethod { get; set; }

    }
}
