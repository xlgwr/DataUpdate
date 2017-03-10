using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EMMS.UpdateOldDBToNew.modelNew
{
    public class CaseFormVm
    {
        public CaseFormVm()
        {
            DefendantlLists = new List<PersonInfo>();
            JudgeLists = new List<PersonInfo>();
            PlaintiffLists = new List<PersonInfo>();
            RepresentationDLists = new List<PersonInfo>();
            RepresentationPLists = new List<PersonInfo>();
        }
        /// <summary>
        /// 解析前的数据
        /// </summary>
        public m_Case_items CaseItems { get; set; }
        /// <summary>
        /// 解析后的数据
        /// </summary>
        public CaseModel CaseInfo { get; set; }
        /// <summary>
        /// 原告
        /// </summary>
        public List<PersonInfo> PlaintiffLists { get; set; }
        /// <summary>
        /// 被告
        /// </summary>
        public List<PersonInfo> DefendantlLists { get; set; }


        /// <summary>
        /// 被告代表
        /// </summary>
        public List<PersonInfo> RepresentationDLists { get; set; }
        /// <summary>
        /// 原告代表
        /// </summary>
        public List<PersonInfo> RepresentationPLists { get; set; }
        /// <summary>
        /// 法官
        /// </summary>
        public List<PersonInfo> JudgeLists { get; set; }


        /// <summary>
        /// 原告
        /// </summary>
        public string PlaintiffListsString { get; set; }
        /// <summary>
        /// 被告
        /// </summary>
        public string DefendantlListsString { get; set; }
        /// <summary>
        /// 被告代表
        /// </summary>
        public string RepresentationDListsString { get; set; }
        /// <summary>
        /// 原告代表
        /// </summary>
        public string RepresentationPListsString { get; set; }
        /// <summary>
        /// 法官
        /// </summary>
        public string JudgeListsString { get; set; }

    }

    public class PersonInfo
    {


        public long Id { get; set; }

        public long Caseid { get; set; }

        public long Entityid { get; set; }
        /// <summary>
        /// 组别,与原告对应上
        /// </summary>
        public int GroupNO { get; set; }
        /// <summary>
        /// 英文名字
        /// </summary>
        public string FullName_En { get; set; }
        /// <summary>
        /// 中文名字
        /// </summary>
        public string FullName_Cn { get; set; }
        /// <summary>
        /// 公司/个人 1:个人2:公司3:法官  默认为个人
        /// </summary>
        public int Type { get; set; }

    }

    [Serializable]
    [PetaPoco.TableName("t_Case")]
    [PetaPoco.PrimaryKey("Caseid", autoIncrement = true)]
    public class CaseModel
    {
        #region 实体属性

        /// <summary>
        /// 唯一标识(自动递增)
        /// </summary>
        public virtual long CaseId { get; set; }


        /// <summary>
        /// 案件编号(不是GUID,而是法院定的编号,不可以修改,系统内部关系用)
        /// </summary>
        public virtual string CaseNo { get; set; }

        /// <summary>
        /// 案件编号(默认与CaseNo是同一个，但这个可以人工修正,显示时都显示这个编号)
        /// </summary>
        public virtual string CaseNoNew { get; set; }

        /// <summary>
        /// 中文法庭编号 如(民事訴訟 001/2014) 是案件上解析出来的
        /// </summary>
        public virtual string CaseNo_Cn { get; set; }

        /// <summary>
        /// 次数[2/4]#
        /// </summary>
        public virtual string NumberTimes { get; set; }

        /// <summary>
        /// 法院id
        /// </summary>
        public virtual long CourtID { get; set; }

        /// <summary>
        /// 案件类别:民事上訴/傷亡訴訟/民事訴訟/僱員補償案件/刑事案件/破產案件/雜項案件/小額錢債審裁處上訴/建築及仲裁訴訟/裁判法院上訴
        /// </summary>
        public virtual long CaseTypeID { get; set; }

        /// <summary>
        /// 案件年份2015
        /// </summary>
        public virtual string Year { get; set; }

        /// <summary>
        /// 序号001
        /// </summary>
        public virtual string SerialNo { get; set; }

        /// <summary>
        /// 开庭日期
        /// </summary>
        public virtual string CourtDay { get; set; }

        /// <summary>
        /// 当事人各方（原告，被告的原始记录)
        /// </summary>
        public virtual string Parties { get; set; }

        /// <summary>
        /// 原告
        /// </summary>
        public virtual string Plaintiff { get; set; }

        /// <summary>
        /// 原告地址
        /// </summary>
        public virtual string P_Address { get; set; }

        /// <summary>
        /// 被告
        /// </summary>
        public virtual string Defendant { get; set; }

        /// <summary>
        /// 被告地址
        /// </summary>
        public virtual string D_Address { get; set; }

        /// <summary>
        /// 原因/性质
        /// </summary>
        public virtual string Nature { get; set; }

        /// <summary>
        /// 法官
        /// </summary>
        public virtual string Judge { get; set; }

        /// <summary>
        /// 应讯代表
        /// </summary>
        public virtual string Representation { get; set; }

        /// <summary>
        /// 原告代表
        /// </summary>
        public virtual string Representation_P { get; set; }

        /// <summary>
        /// 被告代表
        /// </summary>
        public virtual string Representation_D { get; set; }

        /// <summary>
        /// 聆讯
        /// </summary>
        public virtual string Hearing { get; set; }

        /// <summary>
        /// 币别(excel上有)
        /// </summary>
        public virtual string Currency { get; set; }

        /// <summary>
        /// 总金额(excel上有)
        /// </summary>
        public virtual int Amount { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public virtual string Other { get; set; }

        /// <summary>
        /// 其他1
        /// </summary>
        public virtual string Other1 { get; set; }

        /// <summary>
        /// 有无判决书(0:无 1:有)
        /// </summary>
        public virtual int Judgement { get; set; }

        /// <summary>
        /// HtmlID
        /// </summary>
        public virtual long HtmlID { get; set; }

        /// <summary>
        /// 1：线下可见（未审） 2:线上可见（已审）
        /// </summary>
        public virtual int Show { get; set; }

        /// <summary>
        /// 数据级别(0:公开 1:内部人员可见 2:主管可见 3:超级用户可见)
        /// </summary>
        public virtual string DataGradeID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string AddUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual System.DateTime? AddDatetime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string UpdUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual System.DateTime? UpdDatetime { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
        /// <summary>
        /// 0:禁用 1:启用
        /// </summary>
        public virtual int Enable { get; set; }
        public virtual string oldDBCourtID { get; set; }

        #endregion



    }


    [Serializable]
    [PetaPoco.TableName("m_CaseType")]
    [PetaPoco.PrimaryKey("CaseTypeID", autoIncrement = true)]
    public class CaseTypeModel
    {
        #region 实体属性

        /// <summary>
        /// 唯一标识(自动递增)
        /// </summary>
        public virtual long CaseTypeID { get; set; }

        /// <summary>
        /// 案件编码
        /// </summary>
        public virtual string CaseType { get; set; }
        /// <summary>
        /// 案件类别:民事上訴/傷亡訴訟/民事訴訟/僱員補償案件/刑事案件/破產案件/雜項案件/小額錢債審裁處上訴/建築及仲裁訴訟/裁判法院上訴
        /// </summary>
        public virtual string CaseType_En { get; set; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public virtual string CaseType_Cn { get; set; }

        /// <summary>
        /// 法院ID
        /// </summary>
        public virtual long CourtID { get; set; }

        /// <summary>
        /// 法院类型
        /// </summary>
        public virtual long Tykpe { get; set; }
        #endregion



    }

    /// <summary>
    /// 公司关键字设定
    /// </summary>
    [Serializable]
    [PetaPoco.TableName("m_ComWord")]
    [PetaPoco.PrimaryKey("WordId", autoIncrement = true)]
    public class MKeyWordModel
    {
        /// <summary>
        /// 主建 参数KEY(自编)
        /// </summary>
        public long WordId { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string WordName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public long DspNo { get; set; }

        public string Remark { get; set; }


    }
    [Serializable]
    [PetaPoco.TableName("s_Entity")]
    [PetaPoco.PrimaryKey("Entityid", autoIncrement = true)]
    public class EntityModel
    {
        public class VarKey
        {
            public const string tablename = "s_Entity";
            public const string PrimaryKey = "Entityid";
        }

        #region 实体属性

        /// <summary>
        /// 唯一标识(自动递增)
        /// </summary>
        public virtual long Entityid { get; set; }


        /// <summary>
        /// 类别1:P 2:C 3:U(unknown)
        /// </summary>
        public virtual int Type { get; set; }

        /// <summary>
        /// 标记1:法庭 2:公司 3:土地 4:信贷 5:公共
        /// </summary>
        public virtual int Flag { get; set; }

        /// <summary>
        /// 参数(当flag=1时 这里将保存0:原告,1:被告 2:法官 3:原告律师 4:被告律师等)
        /// </summary>
        public virtual int Param1 { get; set; }

        public virtual int Same { get; set; }



        #endregion



    }

    public class apiReg
    {
        public static string getValueCN(string value)
        {
            var _FullName_Cn = value;
            try
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _FullName_Cn = value.Replace(" ", "");
                }
                return _FullName_Cn;
            }
            catch (Exception ex)
            {
                return _FullName_Cn;
            }
        }
        public static string getValueEN(string value)
        {
            var _FullName_En = value;
            try
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _FullName_En = value.Replace(",", " ");
                    _FullName_En = apiReg.regDoubleSpace2(_FullName_En);
                }

                return _FullName_En;
            }
            catch (Exception ex)
            {
                return _FullName_En;
            }
        }
        public static string regDoubleSpace2(string strV)
        {
            var v = strV;
            try
            {

                var reg1 = @"[\f\n\r\t\v]";//去除换行符等
                var reg2 = @"[\u3000\u0020]{2,}";//去空格,两个以上空格改为一个，全角、半角

                v = ReplaceReg(reg1, v, " ");

                v = ReplaceReg(reg2, v, " ");


                return v;
            }
            catch (Exception)
            {
                return v;
            }

        }
        public static string ReplaceReg(string patt, string input, string totxt)
        {
            try
            {
                Regex rgx = new Regex(patt, RegexOptions.IgnoreCase);
                return rgx.Replace(input.ToString(), totxt).Trim();
            }
            catch (Exception ex)
            {

                return input;
            }
        }


    }
    [Serializable]
    [PetaPoco.TableName("s_Person")]
    [PetaPoco.PrimaryKey("Entityid", autoIncrement = false)]
    public class PersonModel
    {
        public class VarKey
        {
            public const string tablename = "s_Person";
            public const string PrimaryKey = "Entityid";
        }
        #region 实体属性

        /// <summary>
        /// 
        /// </summary>
        public virtual long Entityid { get; set; }


        private string _FullName_En = "";
        private string _FullName_Cn = "";
        /// <summary>
        /// 全名
        /// </summary>
        public virtual string FullName_En
        {
            get
            {
                return _FullName_En;
            }
            set
            {

                _FullName_En = apiReg.getValueEN(value);

            }

        }

        /// <summary>
        /// 简体姓名
        /// </summary>
        public virtual string FullName_Cn
        {
            get
            {
                return _FullName_Cn;
            }
            set
            {
                _FullName_Cn = apiReg.getValueCN(value);

            }
        }

        /// <summary>
        /// 别名英文
        /// </summary>
        public virtual string Alias_En { get; set; }

        /// <summary>
        /// 别名中文
        /// </summary>
        public virtual string Alias_Cn { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public virtual string Nickname { get; set; }

        /// <summary>
        /// 性别 0:男 1:女 2:其他
        /// </summary>
        public virtual int? Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public virtual System.DateTime? Birthdate { get; set; }

        /// <summary>
        /// 婚姻状态 0;未婚 1：已婚 2：离婚 3:鳏寡 4、其他
        /// </summary>
        public virtual int? MaritalStatus { get; set; }

        /// <summary>
        /// 汉字编码
        /// </summary>
        public virtual string ChineseCode { get; set; }

        /// <summary>
        /// 国籍(Countryid)
        /// </summary>
        public virtual int? Nationality { get; set; }

        /// <summary>
        /// 出生地
        /// </summary>
        public virtual string PlaceBirth { get; set; }

        /// <summary>
        /// 1：线下可见（未审） 2:线上可见（已审）(信贷以此为准)
        /// </summary>
        public virtual int Show { get; set; }

        /// <summary>
        /// 数据级别
        /// </summary>
        public virtual string DataGradeID { get; set; }
        public virtual string Remarks { get; set; }





        #endregion



    }
    [Serializable]
    [PetaPoco.TableName("s_Company")]
    [PetaPoco.PrimaryKey("Entityid", autoIncrement = false)]
    public class CompanyModel
    {
        #region 实体属性

        /// <summary>
        /// 公司ID
        /// </summary>
        public virtual long Entityid { get; set; }


        /// <summary>
        /// 公司编号
        /// </summary>
        public virtual string CRNo { get; set; }

        /// <summary>
        /// 营业执照号码
        /// </summary>
        public virtual string CIBRNO { get; set; }

        /// <summary>
        /// 0:英文 1:简体 2:繁体
        /// </summary>
        public virtual int Language { get; set; }


        ///// <summary>
        ///// 公司全名_英文
        ///// </summary>
        //public virtual string FullName_En { get; set; }

        ///// <summary>
        ///// 公司全名_中文
        ///// </summary>
        //public virtual string FullName_Cn { get; set; }
        private string _FullName_En = "";
        private string _FullName_Cn = "";
        /// <summary>
        /// 全名
        /// </summary>
        public virtual string FullName_En
        {
            get
            {
                return _FullName_En;
            }
            set
            {
                _FullName_En = apiReg.getValueEN(value);

            }

        }

        /// <summary>
        /// 简体姓名
        /// </summary>
        public virtual string FullName_Cn
        {
            get
            {
                return _FullName_Cn;
            }
            set
            {
                _FullName_Cn = apiReg.getValueCN(value);

            }
        }

        /// <summary>
        /// 公司国别
        /// </summary>
        public virtual long CountryID { get; set; }

        /// <summary>
        /// 公司类别 0:有限公司 1:无限公司 2:股份公司
        /// </summary>
        public virtual string CompanyType { get; set; }

        /// <summary>
        /// 成立日期
        /// </summary>
        public virtual string IncorporationDate { get; set; }

        /// <summary>
        /// 注册资本
        /// </summary>
        public virtual string AuthorizedCapital { get; set; }

        /// <summary>
        /// 地域 0:本地 1:国外 2:其他
        /// </summary>
        public virtual int? Areas { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public virtual string PlaceRegistration { get; set; }

        /// <summary>
        /// 是否上市(0:未上市 1:已上市)
        /// </summary>
        public virtual int? Listed { get; set; }

        /// <summary>
        /// 已发行股票(0:未发行 1:已发行)
        /// </summary>
        public virtual int? IssuedShares { get; set; }

        /// <summary>
        /// 现状(公司注册处有这项)
        /// </summary>
        public virtual string ActiveStatus { get; set; }

        /// <summary>
        /// 清盘模式
        /// </summary>
        public virtual string WindingUpMode { get; set; }

        /// <summary>
        /// 已告解散日期
        /// </summary>
        public virtual string DissolutionDate { get; set; }

        /// <summary>
        /// 押记登记册
        /// </summary>
        public virtual string RegisterOfCharges { get; set; }

        /// <summary>
        /// 重要事项
        /// </summary>
        public virtual string ImportantNote { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remarks { get; set; }

        /// <summary>
        /// 网页源码id 
        /// </summary>
        public virtual long HtmlID { get; set; }

        /// <summary>
        /// 1：线下可见（未审） 2:线上可见（已审）(公司以此为准)
        /// </summary>
        public virtual int Show { get; set; }

        /// <summary>
        /// 数据级别
        /// </summary>
        public virtual string DataGradeID { get; set; }

        public virtual long Enable { get; set; }

        /// <summary>
        /// 添加者
        /// </summary>
        public virtual string AddUser { get; set; }

        /// <summary>
        /// 添加日期
        /// </summary>
        public virtual DateTime? AddDatetime { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        public virtual string UpdUser { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public virtual DateTime? UpdDatetime { get; set; }


        #endregion



    }
    
    [Serializable]
    [PetaPoco.TableName("t_Plaintiff")]
    [PetaPoco.PrimaryKey("id", autoIncrement = true)]
    public class PlaintiffModel
    {
        #region 实体属性

        /// <summary>
        /// 唯一标识(自动递增)
        /// </summary>
        public virtual long id { get; set; }


        /// <summary>
        /// 案件编号
        /// </summary>
        public virtual long Caseid { get; set; }

        /// <summary>
        /// Entityid
        /// </summary>
        public virtual long Entityid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int GroupNO { get; set; }





        #endregion



    }

    [Serializable]
    [PetaPoco.TableName("t_Defendant")]
    [PetaPoco.PrimaryKey("id", autoIncrement = true)]
    public class DefendantModel
    {
        #region 实体属性

        /// <summary>
        /// 唯一标识(自动递增)
        /// </summary>
        public virtual long id { get; set; }


        /// <summary>
        /// 案件编号
        /// </summary>
        public virtual long Caseid { get; set; }

        /// <summary>
        /// Entityid
        /// </summary>
        public virtual long Entityid { get; set; }

        /// <summary>
        /// 组别,为了与被告代表对应
        /// </summary>
        public virtual int GroupNO { get; set; }





        #endregion



    }

    [Serializable]
    [PetaPoco.TableName("t_Representation_P")]
    [PetaPoco.PrimaryKey("id", autoIncrement = true)]
    public class RepresentationPModel
    {
        #region 实体属性

        /// <summary>
        /// 唯一标识(自动递增)
        /// </summary>
        public virtual long id { get; set; }


        /// <summary>
        /// 案件编号
        /// </summary>
        public virtual long Caseid { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public virtual long Entityid_R { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int GroupNO { get; set; }





        #endregion



    }

    [Serializable]
    [PetaPoco.TableName("t_Representation_D")]
    [PetaPoco.PrimaryKey("id", autoIncrement = true)]
    public class RepresentationDModel
    {
        #region 实体属性

        /// <summary>
        /// 唯一标识(自动递增)
        /// </summary>
        public virtual long id { get; set; }


        /// <summary>
        /// 案件编号
        /// </summary>
        public virtual long Caseid { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public virtual long Entityid_R { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int GroupNO { get; set; }





        #endregion



    }

    [Serializable]
    [PetaPoco.TableName("t_Judge")]
    [PetaPoco.PrimaryKey("id", autoIncrement = true)]
    public class JudgeModel
    {
        #region 实体属性

        /// <summary>
        /// 唯一标识(自动递增)
        /// </summary>
        public virtual long id { get; set; }


        /// <summary>
        /// 案件编号
        /// </summary>
        public virtual long Caseid { get; set; }

        /// <summary>
        /// Entityid
        /// </summary>
        public virtual long Entityid { get; set; }





        #endregion



    }

    [Serializable]
    [PetaPoco.TableName("t_UpdateLog")]
    [PetaPoco.PrimaryKey("Updateid", autoIncrement = true)]
    public class UpdateLogModel
    {
        #region 实体属性

        /// <summary>
        /// 唯一标识(自动递增)
        /// </summary>
        public virtual long Updateid { get; set; }


        /// <summary>
        /// 案件编号
        /// </summary>
        public virtual long Caseid { get; set; }

        /// <summary>
        /// 修改内容
        /// </summary>
        public virtual string UpdateContent { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public virtual System.DateTime? Updtime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public virtual string Upduser { get; set; }





        #endregion



    }

    [Serializable]
    [PetaPoco.TableName("t_Address")]
    [PetaPoco.PrimaryKey("AddressID", autoIncrement = true)]
    public class AddressModel
    {
        #region 实体属性

        /// <summary>
        /// 
        /// </summary>
        public long AddressID { get; set; }

        /// <summary>
        /// 人或公司编号
        /// </summary>
        public long Entityid { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 大厦名称
        /// </summary>
        public string BuildName { get; set; }

        /// <summary>
        /// 街道
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// 街号牌
        /// </summary>
        public string StreetNumber { get; set; }

        /// <summary>
        /// 座号
        /// </summary>
        public string SeatNO { get; set; }

        /// <summary>
        /// 人或公司编号
        /// </summary>
        public string Floor { get; set; }

        /// <summary>
        /// 楼层
        /// </summary>
        public string RoomNO { get; set; }

        /// <summary>
        /// 屋号
        /// </summary>
        public string HouseNO { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 地段类型
        /// </summary>
        public string LotType { get; set; }

        /// <summary>
        /// 地段编号
        /// </summary>
        public string LotNo { get; set; }

        /// <summary>
        /// 分段
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// 小分段
        /// </summary>
        public string Subsection { get; set; }

        /// <summary>
        /// 邮政信箱
        /// </summary>
        public string POBox { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Resident(住址)
        /// </summary>
        public string AddressType { get; set; }

        /// <summary>
        /// 所有权 1:拥有Owned 2:租用Rented
        /// </summary>
        public long OwnerShip { get; set; }

        /// <summary>
        /// 生活时长(31年00月)
        /// </summary>
        public string LenghtTime { get; set; }





        #endregion



    }

    [Serializable]
    [PetaPoco.TableName("t_PublicRelation")]
    [PetaPoco.PrimaryKey("PLRelationID", autoIncrement = true)]
    public partial class PublicRelationModel
    {
        #region 实体属性

        /// <summary>
        /// 
        /// </summary>
        public virtual long PLRelationID { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public virtual long Entityid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual long TableID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual long PublicID { get; set; }





        #endregion



    }
}
