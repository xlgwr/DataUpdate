
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMMS.Domain.Member
{
    [Serializable]
    [PetaPoco.TableName("m_Member")]
    [PetaPoco.PrimaryKey("MemberID",autoIncrement=true)]
    public class MemberModel
    {
        #region 实体属性
        
        /// <summary>
        /// 会员id(102016050001)
        /// </summary>
        public virtual long MemberID { get; set; }
        
        
        /// <summary>
        /// 会员帐号(唯一)
        /// </summary>
        public virtual string MemberName { get; set; }
        
        /// <summary>
        /// 会员级别
        /// </summary>
        public virtual int? MemberGradeID { get; set; }
        
        /// <summary>
        /// 会员密码
        /// </summary>
        public virtual string Password { get; set; }
        
        /// <summary>
        /// 余额
        /// </summary>
        public virtual string RemainingSum { get; set; }
        
        /// <summary>
        /// 开票日(如25日,遇到没有这个日期的情况用最后这一天)
        /// </summary>
        public virtual string InvoiceDate { get; set; }
        
        /// <summary>
        /// 0:个人 1:公司
        /// </summary>
        public virtual int? Type { get; set; }
        
        /// <summary>
        /// 公司会员资料id
        /// </summary>
        public virtual long? MemberComanyID { get; set; }
        
        /// <summary>
        /// 姓
        /// </summary>
        public virtual string Surname { get; set; }
        
        /// <summary>
        /// 名
        /// </summary>
        public virtual string GivenNames { get; set; }
        
        /// <summary>
        /// 称呼(Mr/Mrs/Ms)
        ///  Mr = 1,
        ///  Mrs = 2,
        ///  Ms = 3  
        /// </summary>
        public virtual string Salutation { get; set; }
        
        /// <summary>
        /// 中文姓名
        /// </summary>
        public virtual string FullName_Cn { get; set; }
        
        /// <summary>
        /// 繁体姓名
        /// </summary>
        public virtual string FullName_Tm { get; set; }
        
        /// <summary>
        /// 大厦名称
        /// </summary>
        public virtual string BuildName { get; set; }
        
        /// <summary>
        /// 街道
        /// </summary>
        public virtual string Street { get; set; }
        
        /// <summary>
        /// 街道号
        /// </summary>
        public virtual string StreetNumber { get; set; }
        
        /// <summary>
        /// 座号
        /// </summary>
        public virtual string SeatNO { get; set; }
        
        /// <summary>
        /// 楼层
        /// </summary>
        public virtual string Floor { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public virtual string RoomNO { get; set; }
        
        /// <summary>
        /// 屋号
        /// </summary>
        public virtual string HouseNO { get; set; }
        
        /// <summary>
        /// 区域
        /// </summary>
        public virtual string Area { get; set; }
        
        /// <summary>
        /// 城市
        /// </summary>
        public virtual string City { get; set; }
        
        /// <summary>
        /// 省
        /// </summary>
        public virtual string Province { get; set; }
        
        /// <summary>
        /// 国家(从国家表中去选)
        /// </summary>
        public virtual string CountryID { get; set; }
        
        /// <summary>
        /// 邮政编码
        /// </summary>
        public virtual string PostalCode { get; set; }
        
        /// <summary>
        /// 家庭电话
        /// </summary>
        public virtual string HomeTel { get; set; }
        
        /// <summary>
        /// 办公电话
        /// </summary>
        public virtual string OfficeTel { get; set; }
        
        /// <summary>
        /// 手机号码
        /// </summary>
        public virtual string MobilePhone { get; set; }
        
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public virtual string Email { get; set; }
        
        /// <summary>
        /// 目的1:Know my client
        /// </summary>
        public virtual int? Purpose1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Purpose2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Purpose3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Purpose4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Purpose5 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Purpose6 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Purpose7 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Pathway1 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Pathway2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Pathway3 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Pathway4 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Pathway5 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Pathway6 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string PicPath1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string PicPath2 { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// 
        //[ResultColumn]
        //public virtual string SHPicPath1 { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// 
        //[ResultColumn]
        //public virtual string SHPicPath2 { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual string PicPath3 { get; set; }
        
        /// <summary>
        /// 支付方式
        /// <!--0:paypal，1:中国银联，2:支付宝，3:银行转帐，4:支票-->
        /// </summary>
        public virtual int? PaymentWay { get; set; }
        
        /// <summary>
        /// 是否接受协议(0:不接受,1:接受)
        /// </summary>
        public virtual int? AcceptAgreement { get; set; }
        
        /// <summary>
        /// 邮箱验证(0:未通过 1:通过)
        /// </summary>
        public virtual int? EmailVerification { get; set; }
        
        /// <summary>
        /// 最后搜索时间
        /// </summary>
        public virtual System.DateTime? LastSeachTime { get; set; }
        
        /// <summary>
        /// 会员是否可用(1:可用 0:不可用)
        /// </summary>
        public virtual int? Enable { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual string Adduser { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual System.DateTime? Addtime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual string Upduser { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual System.DateTime? Updtime { get; set; }

        //[ResultColumn]
        //public List<EMMS.Domain.MemberRight.MemberRightModel> MemberRightModel { get; set; }

        [ResultColumn]
        public EMMS.Domain.MemberComanyModel MemberComanyModel { get; set; }

        [ResultColumn]
        public List<EMMS.Domain.ContactPersonModel> ContactPersonModel { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //[ResultColumn]
        //public virtual string LastSeachTimeString {
        //    get
        //    {

        //        if (LastSeachTime!=null)
        //        {
        //            try
        //            {
        //                //DateTime dt = Convert.ToDateTime(DateTime.Now.AddDays(-20).ToShortDateString());
        //                DateTime dt = LastSeachTime.Value.AddDays(+ActiveDay);
        //                return dt >= DateTime.Now ? "活跃" : "不活跃";
        //            }
        //            catch (Exception)
        //            {
        //                return  "不活跃";
        //            }
        //        }
        //        else
        //        {
        //            return  "不活跃";
        //        }
        //    }
        //}

        //[ResultColumn]
        //public int ActiveDay { get; set; }

        //新增
        /// <summary>
        /// 地址，旧数据库
        /// </summary>
        public virtual string Address { get; set; }
        /// <summary>
        /// 传真，旧数据库
        /// </summary>
        public virtual string Fax { get; set; }
        /// <summary>
        /// 备注，旧数据库,,业务类型，hkid，备注
        /// </summary>
        public virtual string Remark { get; set; }
        #endregion

        public class VarKey
        {
            public const string tablename = "m_member";
            public const string memberid = "memberid";
            public const string membername = "membername";
            public const string membergradeid = "membergradeid";
            public const string password = "password";
            public const string remainingsum = "remainingsum";
            public const string invoicedate = "invoicedate";
            public const string type = "type";
            public const string membercomanyid = "membercomanyid";
            public const string surname = "surname";
            public const string givennames = "givennames";
            public const string salutation = "salutation";
            public const string fullname_cn = "fullname_cn";
            public const string fullname_tm = "fullname_tm";
            public const string buildname = "buildname";
            public const string street = "street";
            public const string streetnumber = "streetnumber";
            public const string seatno = "seatno";
            public const string floor = "floor";
            public const string roomno = "roomno";
            public const string houseno = "houseno";
            public const string area = "area";
            public const string city = "city";
            public const string province = "province";
            public const string countryid = "countryid";
            public const string postalcode = "postalcode";
            public const string hometel = "hometel";
            public const string officetel = "officetel";
            public const string mobilephone = "mobilephone";
            public const string email = "email";
            public const string purpose1 = "purpose1";
            public const string purpose2 = "purpose2";
            public const string purpose3 = "purpose3";
            public const string purpose4 = "purpose4";
            public const string purpose5 = "purpose5";
            public const string purpose6 = "purpose6";
            public const string purpose7 = "purpose7";
            public const string pathway1 = "pathway1";
            public const string pathway2 = "pathway2";
            public const string pathway3 = "pathway3";
            public const string pathway4 = "pathway4";
            public const string pathway5 = "pathway5";
            public const string pathway6 = "pathway6";
            public const string picpath1 = "picpath1";
            public const string picpath2 = "picpath2";
            public const string picpath3 = "picpath3";
            public const string paymentway = "paymentway";
            public const string acceptagreement = "acceptagreement";
            public const string emailverification = "emailverification";
            public const string lastseachtime = "lastseachtime";
            public const string enable = "enable";
            public const string adduser = "adduser";
            public const string upduser = "upduser";
            public const string addtime = "addtime";
            public const string updtime = "updtime";
        }

 

    }
}
 