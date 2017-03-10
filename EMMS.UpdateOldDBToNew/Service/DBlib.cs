using Common.Logging;
using EMMS.Common;
using EMMS.Domain;
using EMMS.Domain.Member;
using EMMS.UpdateOldDBToNew.modelNew;
using EMMS.UpdateOldDBToNew.modelOld;
using GetPanamaDB.models;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMMS.UpdateOldDBToNew.Service
{
    public class DBlib
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 创建数据库连接 旧数据库 eCM
        /// </summary>
        public static Database dbOld = new PetaPoco.Database("MSSQLOld");
        /// <summary>
        /// 创建数据库连接 新数据库 EMMS
        /// </summary>
        public static Database dbNewEmms = new PetaPoco.Database("MSSQLNew");
        /// <summary>
        /// 创建数据库连接 爬虫数据库 EMMS_WebGetData
        /// </summary>
        public static Database dbWebGetData = new PetaPoco.Database("MSSQLOffline");
        /// <summary>
        /// 创建数据库连接 爬虫数据库 PanamaDB
        /// </summary>
        public static Database dbPanamaDB = new PetaPoco.Database("MSSQLPanamaDB");
        public static Form _form1 = null;

        public static List<MKeyWordModel> _tmpGetComName = new List<MKeyWordModel>();

        public static bool isRunRelite = false;

        public static void SetMsg(Form f, Control cl, string msg, bool isAppend)
        {
            try
            {
                if (_form1 == null)
                {
                    _form1 = f;
                }

                _form1.Invoke(new Action(delegate()
                    {

                        if (!isAppend)
                        {
                            cl.Text = msg;
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(cl.Text))
                        {
                            cl.Text = msg;
                        }
                        else
                        {
                            cl.Text = msg + "\n" + cl.Text;
                        }

                        if (cl.Text.Length > 1024 * 10)
                        {
                            cl.Text = cl.Text.Substring(0, 1024 * 10) + "......................";
                        }
                    }));
            }
            catch (Exception ex)
            {
                _form1 = f;
            }
        }

        public static void SetEnable(Form f, Control cl, bool isenable)
        {
            try
            {
                if (_form1 == null)
                {
                    _form1 = f;
                }

                _form1.Invoke(new Action(delegate()
                {
                    cl.Enabled = isenable;
                }));
            }
            catch (Exception ex)
            {
                _form1 = f;
            }
        }
        #region cust_approved（前端帐号）及 am_cust_approved（自动监察帐号）


        /// <summary>
        /// 更新账号到新数据库
        /// 处理规则
        /// cust_approved：
        /// 1.如新数据库，无记录，则新增到新数据库，否则，无需处理。
        /// 2.旧数据库密码为明文，按新系统加密。写入新数据库
        /// 3.前端帐号与自动监察帐号相同账号只更新一次。
        /// 4.按 actype 属性区分个人及公司.m_Member(会员),m_MemberComany(会员公司)
        /// 5.公司中的联系人资料写入:m_ContactPerson(公司联系人) 
        /// </summary>
        public static void UpdateCustOldToNew(object t)
        {
            var msg = "";
            long icountSaveAll = 0;
            long ifailSaveAll = 0;
            long iSameNoSave = 0;

            objT o = (objT)t;
            try
            {
                SetEnable(o.f, o.btn, false);
                SetEnable(o.f, o.btn2, false);

                SetMsg(o.f, o.cl, msg, true);

                Sql _sql = new Sql();
                _sql.Append(" select * from");
                _sql.Append(" (select * from cust_approved");
                _sql.Append(" UNION ");
                _sql.Append(" select * from am_cust_approved) a");
                var tmpAllOldCust = dbOld.Fetch<custApprovedModel>(_sql);

                msg = string.Format("########################开始处理--》已获取记录：{0} 条. 当前时间：{1}.###############################", tmpAllOldCust.Count, DateTime.Now);
                logger.Debug(msg);
                SetMsg(o.f, o.cl, msg, true);

                var runFlag = 0;
                foreach (var item in tmpAllOldCust)
                {
                    runFlag++;

                    msg = string.Format("***{0}/{1} 开始处理--》公司ID：{2},用户名：{3}.", runFlag, tmpAllOldCust.Count, item.acno, item.login);
                    logger.Debug(msg);
                    SetMsg(o.f, o.cl, msg, true);
                    if (!string.IsNullOrWhiteSpace(item.login))
                    {
                        var tmpName = item.login.Trim();
                        var tmpExit = IsMemberName(tmpName);
                        if (!tmpExit)
                        {
                            try
                            {
                                //公司个人判定
                                var isType = false;//true:公司，false:个人
                                if (item.actype.ToLower().Contains("institute"))
                                {
                                    isType = true;
                                }
                                isType = checkType(item.coname);

                                //开始处理。
                                var MBModel = new MemberModel();

                                //共用，基础表
                                MBModel.MemberName = item.login;
                                MBModel.Password = item.pw;
                                //MBModel.MemberGradeID = Convert.ToInt32(dtMB["MemberGrade"]);
                                MBModel.Enable = item.enablelogin;
                                //MBModel.RemainingSum = dtMB["RemainingSum"].ToString();
                                //MBModel.InvoiceDate ="25";
                                MBModel.Type = isType ? 1 : 0;
                                MBModel.PaymentWay = 4;//提定支票
                                MBModel.Remark = item.remarks;

                                //备注处理
                                if (!string.IsNullOrWhiteSpace(item.btitle))
                                {
                                    if (!string.IsNullOrWhiteSpace(MBModel.Remark))
                                    {
                                        MBModel.Remark = item.btitle + "@" + MBModel.Remark;
                                    }
                                    else
                                    {
                                        MBModel.Remark = item.btitle;
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(item.hkid))
                                {
                                    if (!string.IsNullOrWhiteSpace(MBModel.Remark))
                                    {
                                        MBModel.Remark = item.hkid + "@" + MBModel.Remark;
                                    }
                                    else
                                    {
                                        MBModel.Remark = item.hkid;
                                    }
                                }
                                //开始处理
                                if (isType)
                                {
                                    #region 绑定值 公司

                                    //公司资料表
                                    var MC = new MemberComanyModel();
                                    MC.FullName_En = item.coname;
                                    //MC.BusinessType =
                                    MC.CIBRNO = item.corpid;
                                    MC.Address = item.addr;

                                    MBModel.MemberComanyModel = MC;
                                    //联系人信息
                                    //联系人信息
                                    List<Domain.ContactPersonModel> CP = new List<Domain.ContactPersonModel>();

                                    EMMS.Domain.ContactPersonModel CPModel = new EMMS.Domain.ContactPersonModel();

                                    CPModel.Surname = item.lname;
                                    CPModel.GivenNames = item.fname;
                                    CPModel.Salutation = item.title.Equals("Mr") ? "1" : "3";
                                    CPModel.MobilePhone = item.phone;
                                    CPModel.Email = item.email;
                                    CPModel.Fax = item.fax;
                                    CP.Add(CPModel);

                                    MBModel.ContactPersonModel = CP;
                                    #endregion
                                }
                                else
                                {
                                    #region 绑定值 个人

                                    MBModel.Surname = item.lname;
                                    MBModel.GivenNames = item.fname;
                                    MBModel.Salutation = item.title.Equals("Mr") ? "1" : "3";
                                    MBModel.MobilePhone = item.phone;
                                    MBModel.Email = item.email;
                                    MBModel.Address = item.addr;
                                    MBModel.Fax = item.fax;
                                    #endregion
                                }
                                string MemberID = string.Empty;
                                if (AddIndividual(MBModel, isType, out MemberID))
                                {
                                    icountSaveAll++;
                                    msg = string.Format("*****当前记录更新成功，公司ID：{0},注册ID：{1}.", item.acno, tmpName);
                                    logger.Debug(msg);
                                    SetMsg(o.f, o.cl, msg, true);
                                }
                                else
                                {
                                    ifailSaveAll++;
                                    msg = string.Format("*****当前记录更新出错，公司ID：{0},注册ID：{1}.", item.acno, tmpName);
                                    logger.Error(msg);
                                    SetMsg(o.f, o.cl, msg, true);
                                }
                            }
                            catch (Exception ex)
                            {
                                ifailSaveAll++;
                                msg = string.Format("*****当前更新出错，公司ID：{0},注册ID：{1},Error:{2}. 继续下一个。", item.acno, tmpName, ex);
                                logger.Error(msg);
                                SetMsg(o.f, o.cl, msg, true);
                                continue;
                            }
                        }
                        else
                        {
                            iSameNoSave++;
                            msg = string.Format("************************************用户名已存在：{0}.", item.login);
                            logger.Info(msg);
                            SetMsg(o.f, o.cl, msg, true);
                        }
                    }
                    else
                    {
                        ifailSaveAll++;
                        msg = string.Format("*****公司ID:{0} 不存在注册ID号。", item.acno);
                        logger.Error(msg);
                        SetMsg(o.f, o.cl, msg, true);
                    }

                }
                msg = string.Format("**旧数据库存在前端帐号及自动监察帐号总记录：" + tmpAllOldCust.Count + ",更新成功：" + icountSaveAll + " 条,失败：" + ifailSaveAll + " 条,已经存在：" + iSameNoSave + " 条. 结束时间：{0}", DateTime.Now);
                logger.Debug(msg);
                SetMsg(o.f, o.cl, msg, true);
                SetMsg(o.f, o.f, o.f.Tag.ToString(), false);
                SetEnable(o.f, o.btn, true);
                SetEnable(o.f, o.btn2, true);
                //return msg;
            }
            catch (Exception ex)
            {
                msg = string.Format("******更新出错：{0}.", ex);
                SetMsg(o.f, o.cl, msg, true);
                logger.ErrorFormat("******更新出错：{0}.", ex);
                SetEnable(o.f, o.btn, true);
                SetEnable(o.f, o.btn2, true);
                //return ex.Message;
                throw ex;
            }
        }
        /// <summary>
        /// 公司判定
        /// </summary>
        /// <param name="tmpen"></param>
        /// <param name="tmpzh"></param>
        /// <returns></returns>
        static bool checkType(string tmpen)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tmpen))
                {
                    return false;
                }
                return (tmpen.IndexOf("LIMITED", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                         tmpen.IndexOf("COMPANY", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                         tmpen.IndexOf("Ltd.", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                         tmpen.IndexOf("Co.", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                         tmpen.IndexOf("CO.,", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                         tmpen.IndexOf("公司", StringComparison.InvariantCultureIgnoreCase) > -1
                                        ) ? true : false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("*************{0}", ex);
                return false;
            }
        }

        /// <summary>
        /// 写入新数据库，公司及个人。
        /// </summary>
        /// <param name="memberM"></param>
        /// <param name="type">false:个人，true:公司</param>
        /// <param name="MemberID"></param>
        /// <returns></returns>
        public static bool AddIndividual(MemberModel memberM, bool type, out string MemberID)
        {
            bool rtnValue = false;
            try
            {
                memberM.Addtime = DateTime.Now;
                //加密
                if (!string.IsNullOrWhiteSpace(memberM.Password))
                {
                    memberM.Password = Encryption.Encode(memberM.Password);
                }
                MemberID = "";
                #region  个人
                if (!type)
                {
                    using (var scope = dbNewEmms.GetTransaction())
                    {
                        MemberID = dbNewEmms.Insert("m_Member", "MemberID", memberM).ToString();
                        scope.Complete();
                        rtnValue = true;
                    }
                }
                #endregion
                #region 公司
                else
                {
                    using (var scope = dbNewEmms.GetTransaction())
                    {
                        string MemberComanyModelID = dbNewEmms.Insert("m_MemberComany", "MemberComanyID", memberM.MemberComanyModel).ToString();
                        memberM.MemberComanyID = Convert.ToInt32(MemberComanyModelID);
                        for (int i = 0; i < memberM.ContactPersonModel.Count; i++)
                        {
                            memberM.ContactPersonModel[i].MemberComanyID = MemberComanyModelID;
                            dbNewEmms.Insert("m_ContactPerson", "ContactPersonID", memberM.ContactPersonModel[i]).ToString();
                        }

                        MemberID = dbNewEmms.Insert("m_Member", "MemberID", memberM).ToString();

                        scope.Complete();
                        rtnValue = true;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rtnValue;
        }
        public static bool updateOldFlag(DateTime tmpDate, objT o)
        {
            bool rtnValue = false;
            try
            {
                using (var scope = dbOld.GetTransaction())
                {
                    var tmpModel = new courtUpdateFlag();
                    tmpModel.Actiondate = tmpDate;
                    dbOld.Insert("courtUpdateFlag", "Actiondate", false, tmpModel).ToString();

                    var msg = string.Format("*************写入成功.案件表更新标记表 Actiondate:{0}.", tmpDate);
                    logger.Debug(msg);
                    SetMsg(o.f, o.cl, msg, true);
                    scope.Complete();
                    rtnValue = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rtnValue;
        }
        public static bool AddmCaseItems(List<m_Case_items> models, objT o)
        {
            bool rtnValue = false;
            try
            {
                //初始定义
                var tpmEntitID = getEntityCompanyName();
                var tpmInPersonEntitID = getEntityPersonName("In Person");
                var tpmUnKnownEntitID = getEntityPersonName("Unknown");

                var tmpGetComName = dbNewEmms.Fetch<MKeyWordModel>("select * from m_ComWord order by DspNo");

                //////////////////////////////

                using (var scope = dbNewEmms.GetTransaction())
                {
                    foreach (var caseItemModel in models)
                    {
                        caseItemModel.addtime = DateTime.Now;
                        caseItemModel.updtime = DateTime.Now;

                        var CaseID = "";

                        //var CaseID = dbNew.Insert("m_Case_items", "Tid", true, item).ToString();
                        //二期，解析（生成关系）--》直接写入线上数据


                        var webGetDllCase = new WebGetDll.model.m_Case()
                        {
                            tname = caseItemModel.tname,
                            CaseNo = caseItemModel.CaseNo,
                            CourtDay = caseItemModel.CourtDay.ToString("yyyy-MM-dd"),
                            Currency = caseItemModel.Currency,
                            Defendant = caseItemModel.Defendant,
                            Hearing = caseItemModel.Hearing,
                            Judge = caseItemModel.Judge,
                            Nature = caseItemModel.Nature,
                            OpenCourtTime = "",
                            Plaintiff = caseItemModel.PlainTiff,

                            Representation = caseItemModel.Representation,

                            Actiondate = "",//caseItemModel.

                            Amount = caseItemModel.Amount,
                            CheckField = "",//

                            Representation_D = caseItemModel.Representation_D,
                            Representation_P = caseItemModel.Representation_P,
                            Other = caseItemModel.Other,
                            Other1 = caseItemModel.Other1,
                            Parties = caseItemModel.Parties,
                            P_Address = caseItemModel.P_Address,
                            D_Address = caseItemModel.D_Address

                        };


                        var caseData = WebGetDll.Api.getData(webGetDllCase);
                        if (caseData.ReturnStatus < 0)
                        {
                            //解析失败的，写入离线爬虫
                            caseItemModel.Remark += "," + caseData.message;
                            logger.ErrorFormat("*****解析失败的，写入线下爬虫。Error:{0}. oldActonDate:{1}，CaseNo:{2},caseId:{3}.", caseData.message, caseItemModel.CourtDay, caseItemModel.CaseNo, caseItemModel.tkeyNo);
                            CaseID = dbWebGetData.Insert("m_Case_items", "Tid", true, caseItemModel).ToString();
                        }
                        else
                        {
                            try
                            {
                                //
                                CaseFormVm model = new CaseFormVm();
                                model.CaseItems = caseItemModel;
                                SetWebDllToCaseFormVm(model, caseData);

                                //香港特别行政区entityID处理
                                #region

                                if (tpmEntitID > 0)
                                {
                                    //原告
                                    foreach (var item in model.PlaintiffLists)
                                    {
                                        if (item.FullName_En.Equals("Government of HKSAR"))
                                        {
                                            item.Entityid = tpmEntitID;
                                        }
                                    }
                                    //被告
                                    foreach (var item in model.DefendantlLists)
                                    {
                                        if (item.FullName_En.Equals("Government of HKSAR"))
                                        {
                                            item.Entityid = tpmEntitID;
                                        }

                                    }
                                }
                                if (tpmUnKnownEntitID > 0)
                                {
                                    //原告
                                    foreach (var item in model.PlaintiffLists)
                                    {

                                        if (item.FullName_En.Equals("Unknown"))
                                        {
                                            item.Entityid = tpmUnKnownEntitID;
                                        }
                                    }
                                    //被告
                                    foreach (var item in model.DefendantlLists)
                                    {

                                        if (item.FullName_En.Equals("Unknown"))
                                        {
                                            item.Entityid = tpmUnKnownEntitID;
                                        }

                                    }
                                }
                                if (tpmInPersonEntitID > 0)
                                {
                                    //原告代表
                                    foreach (var item in model.RepresentationPLists)
                                    {
                                        if (item.FullName_En.Equals("In Person"))
                                        {
                                            item.Entityid = tpmInPersonEntitID;
                                        }
                                    }
                                    //被告代表
                                    foreach (var item in model.RepresentationDLists)
                                    {

                                        if (item.FullName_En.Equals("In Person"))
                                        {
                                            item.Entityid = tpmInPersonEntitID;
                                        }

                                    }
                                }
                                #endregion

                                //处理公司关建字
                                #region 处理公司关建字
                                if (tmpGetComName != null)
                                {
                                    try
                                    {
                                        if (tmpGetComName.Count > 0)
                                        {
                                            foreach (var itemkey in tmpGetComName)
                                            {
                                                //原告
                                                foreach (var item in model.PlaintiffLists)
                                                {
                                                    if (item.Type <= 1)
                                                    {
                                                        item.Type = reType(itemkey.WordName, item);
                                                    }
                                                }
                                                //被告
                                                foreach (var item in model.DefendantlLists)
                                                {
                                                    if (item.Type <= 1)
                                                    {
                                                        item.Type = reType(itemkey.WordName, item);
                                                    }

                                                }
                                                //原告代表
                                                foreach (var item in model.RepresentationPLists)
                                                {
                                                    if (item.Type <= 1)
                                                    {
                                                        item.Type = reType(itemkey.WordName, item);
                                                    }
                                                }
                                                //被告代表
                                                foreach (var item in model.RepresentationDLists)
                                                {
                                                    if (item.Type <= 1)
                                                    {
                                                        item.Type = reType(itemkey.WordName, item);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex);
                                    }

                                }
                                #endregion


                                //处理完，保存，实体及关系。
                                #region

                                model.CaseInfo.oldDBCourtID = caseItemModel.tkeyNo;
                                model.CaseInfo.Show = 2;
                                model.CaseInfo.AddUser = "0";
                                model.CaseInfo.AddDatetime = DateTime.Now;
                                model.CaseInfo.UpdUser = "0";
                                model.CaseInfo.UpdDatetime = DateTime.Now;
                                model.CaseInfo.Enable = 1;
                                //添加t_Case 案件
                                CaseID = dbNewEmms.Insert(model.CaseInfo).ToString();

                                #region 添加原告
                                foreach (var plaintiff in model.PlaintiffLists)
                                {
                                    //s_Entity
                                    var entityModel = new EntityModel()
                                    {
                                        Param1 = 0,//表示原告  
                                        Type = plaintiff.Type,
                                        Flag = 1
                                    };
                                    if (plaintiff.Entityid < 1)
                                    {

                                        dbNewEmms.Insert(entityModel);

                                        if (plaintiff.Type == 1)
                                        {
                                            //添加个人 
                                            dbNewEmms.Insert(new PersonModel()
                                           {
                                               FullName_Cn = plaintiff.FullName_Cn ?? "",
                                               FullName_En = plaintiff.FullName_En ?? "",
                                               Entityid = entityModel.Entityid,
                                               Show = model.CaseInfo.Show
                                           });
                                        }
                                        else
                                        {
                                            // 添加公司
                                            dbNewEmms.Insert(new CompanyModel()
                                           {
                                               FullName_Cn = plaintiff.FullName_Cn ?? "",
                                               FullName_En = plaintiff.FullName_En ?? "",
                                               Entityid = entityModel.Entityid,
                                               Show = model.CaseInfo.Show
                                           });

                                        }
                                    }
                                    else
                                    {
                                        entityModel.Entityid = plaintiff.Entityid;
                                    }

                                    //t_Plaintiff(原告)
                                    dbNewEmms.Insert(new PlaintiffModel()
                                   {
                                       Caseid = model.CaseInfo.CaseId,
                                       Entityid = entityModel.Entityid,
                                       GroupNO = plaintiff.GroupNO
                                   });


                                }
                                #endregion
                                #region 添加被告
                                foreach (var defendant in model.DefendantlLists)
                                {     //s_Entity
                                    var entityModel = new EntityModel()
                                    {
                                        Param1 = 1,//表示被告
                                        Type = defendant.Type,
                                        Flag = 1
                                    };
                                    if (defendant.Entityid < 1)
                                    {
                                        dbNewEmms.Insert(entityModel);

                                        if (defendant.Type == 1)
                                        {
                                            //添加个人 
                                            dbNewEmms.Insert(new PersonModel()
                                           {
                                               FullName_Cn = defendant.FullName_Cn ?? "",
                                               FullName_En = defendant.FullName_En ?? "",
                                               Entityid = entityModel.Entityid,
                                               Show = model.CaseInfo.Show
                                           });
                                        }
                                        else
                                        {
                                            // 添加公司
                                            dbNewEmms.Insert(new CompanyModel()
                                           {
                                               FullName_Cn = defendant.FullName_Cn ?? "",
                                               FullName_En = defendant.FullName_En ?? "",
                                               Entityid = entityModel.Entityid,
                                               Show = model.CaseInfo.Show
                                           });

                                        }
                                    }
                                    else
                                    {
                                        entityModel.Entityid = defendant.Entityid;
                                    }


                                    //t_Plaintiff(原告)
                                    dbNewEmms.Insert(new DefendantModel()
                                   {
                                       Caseid = model.CaseInfo.CaseId,
                                       Entityid = entityModel.Entityid
                                   });


                                }
                                #endregion
                                #region 添加原告代表
                                foreach (var representationP in model.RepresentationPLists)
                                {
                                    //s_Entity
                                    var entityModel = new EntityModel()
                                    {
                                        Param1 = 3,//原告律师
                                        Type = representationP.Type,
                                        Flag = 1
                                    };
                                    dbNewEmms.Insert(entityModel);

                                    if (representationP.Type == 1)
                                    {
                                        //添加个人 
                                        dbNewEmms.Insert(new PersonModel()
                                       {
                                           FullName_Cn = representationP.FullName_Cn ?? "",
                                           FullName_En = representationP.FullName_En ?? "",
                                           Entityid = entityModel.Entityid,
                                           Show = model.CaseInfo.Show
                                       });
                                    }
                                    else
                                    {
                                        // 添加公司
                                        dbNewEmms.Insert(new CompanyModel()
                                       {
                                           FullName_Cn = representationP.FullName_Cn ?? "",
                                           FullName_En = representationP.FullName_En ?? "",
                                           Entityid = entityModel.Entityid,
                                           Show = model.CaseInfo.Show
                                       });

                                    }
                                    //t_Plaintiff(原告)
                                    dbNewEmms.Insert(new RepresentationPModel()
                                   {
                                       Caseid = model.CaseInfo.CaseId,
                                       Entityid_R = entityModel.Entityid
                                   });


                                }
                                #endregion
                                #region 添加被告代表
                                foreach (var representationD in model.RepresentationDLists)
                                {
                                    //s_Entity
                                    var entityModel = new EntityModel()
                                    {
                                        Param1 = 4,//被告律师 
                                        Type = representationD.Type,
                                        Flag = 1
                                    };
                                    dbNewEmms.Insert(entityModel);

                                    if (representationD.Type == 1)
                                    {
                                        //添加个人 
                                        dbNewEmms.Insert(new PersonModel()
                                       {
                                           FullName_Cn = representationD.FullName_Cn ?? "",
                                           FullName_En = representationD.FullName_En ?? "",
                                           Entityid = entityModel.Entityid,
                                           Show = model.CaseInfo.Show
                                       });
                                    }
                                    else
                                    {
                                        // 添加公司
                                        dbNewEmms.Insert(new CompanyModel()
                                       {
                                           FullName_Cn = representationD.FullName_Cn ?? "",
                                           FullName_En = representationD.FullName_En ?? "",
                                           Entityid = entityModel.Entityid,
                                           Show = model.CaseInfo.Show
                                       });

                                    }
                                    //t_Plaintiff(原告)
                                    dbNewEmms.Insert(new RepresentationDModel()
                                   {
                                       Caseid = model.CaseInfo.CaseId,
                                       Entityid_R = entityModel.Entityid
                                   });


                                }
                                #endregion
                                #region 添加法官关系表
                                foreach (var judge in model.JudgeLists)
                                {
                                    //s_Entity
                                    var entityModel = new EntityModel()
                                    {
                                        Param1 = 2,//表示原告  
                                        Type = 3,
                                        Flag = 1
                                    };
                                    dbNewEmms.Insert(entityModel);


                                    //添加个人 
                                    dbNewEmms.Insert(new PersonModel()
                                   {
                                       FullName_Cn = judge.FullName_Cn ?? "",
                                       FullName_En = judge.FullName_En ?? "",
                                       Entityid = entityModel.Entityid,
                                       Show = model.CaseInfo.Show
                                   });

                                    //t_Plaintiff(原告)
                                    dbNewEmms.Insert(new JudgeModel()
                                   {
                                       Caseid = model.CaseInfo.CaseId,
                                       Entityid = entityModel.Entityid
                                   });


                                }
                                #endregion

                                //添加修改记录
                                dbNewEmms.Insert(new UpdateLogModel()
                                {
                                    Caseid = model.CaseInfo.CaseId,
                                    UpdateContent = "线下添加到线上",
                                    Updtime = DateTime.Now,
                                    Upduser = "0"
                                });
                                #endregion

                            }
                            catch (Exception ex)
                            {
                                //写入失败的，写入离线爬虫

                                caseItemModel.Remark += ",写入线上失败,写入线下爬虫.";
                                logger.ErrorFormat("*****写入线上失败，写入离线爬虫。Error:{0}. oldActonDate:{1}，CaseNo:{2},caseId:{3}.", caseData.message, caseItemModel.CourtDay, caseItemModel.CaseNo, caseItemModel.tkeyNo);

                                CaseID = dbWebGetData.Insert("m_Case_items", "Tid", true, caseItemModel).ToString();
                            }
                        }

                        var msg = string.Format("*************写入成功.CaseNO:{0},案件ActionDate:{1},案件ID:{2},原告@被告：{3}, 新数据库ID:{4}.", caseItemModel.CaseNo, caseItemModel.CourtDay, caseItemModel.tkeyNo, caseItemModel.PlainTiff, CaseID);
                        logger.Debug(msg);
                        SetMsg(o.f, o.cl, msg, true);

                    }

                    scope.Complete();
                    rtnValue = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rtnValue;
        }
        /// <summary>
        /// Government of HKSAR
        /// </summary>
        /// <param name="enname"></param>
        /// <returns></returns>
        public static long getEntityCompanyName(string enname = "Government of HKSAR")
        {
            var _sql = new Sql().Append("SELECT min(Entityid)  from s_Company x where x.FullName_En=@0", enname);
            var dd = dbNewEmms.FirstOrDefault<long>(_sql);
            return dd;
        }
        public static long getEntityPersonName(string enname = "In Person")
        {
            var _sql = new Sql().Append("SELECT min(Entityid)  from s_Person x where x.FullName_En=@0", enname);
            var dd = dbNewEmms.FirstOrDefault<long>(_sql);
            return dd;
        }
        private static int reType(string strkey, PersonInfo p)
        {
            try
            {

                return (p.FullName_En.IndexOf(strkey, StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                         p.FullName_Cn.IndexOf(strkey, StringComparison.InvariantCultureIgnoreCase) > -1
                                        ) ? 2 : 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }
        private static int reType(string strkey, string p)
        {
            try
            {
                return (p.IndexOf(strkey, StringComparison.InvariantCultureIgnoreCase) > -1) ? 2 : 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }
        private static void SetWebDllToCaseFormVm(CaseFormVm caseFormVm, WebGetDll.model.responCaseData caseData)
        {

            long caseTypeId = 0;
            long courtId = 0;
            if (!string.IsNullOrEmpty(caseData.Cases.t_Case.CaseTypeID))
            {
                var caseType = dbNewEmms.FirstOrDefault<CaseTypeModel>("WHERE CaseType=@0", caseData.Cases.t_Case.CaseTypeID);
                if (caseType == null)
                {
                    caseType = dbNewEmms.FirstOrDefault<CaseTypeModel>("WHERE CaseType=@0", "OTHUC");
                }
                if (caseType != null)
                {
                    caseTypeId = caseType.CaseTypeID;
                    courtId = caseType.CourtID;
                }
            }

            var caseInfo = new CaseModel()
            {
                Amount = string.IsNullOrEmpty(caseData.Cases.t_Case.Amount) ? 0 : Convert.ToInt32(caseData.Cases.t_Case.Amount),

                CaseNo = caseData.Cases.t_Case.CaseNo,
                CaseNo_Cn = caseData.Cases.t_Case.CaseNo_Cn,
                CaseNoNew = caseData.Cases.t_Case.CaseNoNew,
                CaseTypeID = caseTypeId,
                CourtDay = caseData.Cases.t_Case.CourtDay,
                CourtID = courtId,
                Currency = caseData.Cases.t_Case.Currency,
                D_Address = caseData.Cases.t_Case.D_Address,
                //DataGradeID=caseData.Cases.t_Case.DataGradeID,
                Defendant = caseData.Cases.t_Case.Defendant,
                Hearing = caseData.Cases.t_Case.Hearing,
                HtmlID = caseData.Cases.t_Case.HtmlID,
                Judge = caseData.Cases.t_Case.Judge,
                //Judgement=caseData.Cases.t_Case.Judgement,
                Nature = caseData.Cases.t_Case.Nature,
                NumberTimes = caseData.Cases.t_Case.NumberTimes,
                Other = caseData.Cases.t_Case.Other,
                Other1 = caseData.Cases.t_Case.Other1,
                P_Address = caseData.Cases.t_Case.P_Address,
                Parties = caseData.Cases.t_Case.Parties,
                Plaintiff = caseData.Cases.t_Case.Plaintiff,
                Representation = caseData.Cases.t_Case.Representation,
                Representation_D = caseData.Cases.t_Case.Representation_D,
                Representation_P = caseData.Cases.t_Case.Representation_P,
                SerialNo = caseData.Cases.t_Case.SerialNo,
                Show = 1,
                Year = caseData.Cases.t_Case.Year
            };
            caseFormVm.CaseInfo = caseInfo;
            foreach (var item in caseData.Cases.Defendants)
            {
                caseFormVm.DefendantlLists.Add(new PersonInfo()
                {
                    Id = caseData.Cases.Defendants.IndexOf(item),
                    FullName_Cn = item.FullName_Cn,
                    FullName_En = item.FullName_En,
                    Type = item.Type + 1
                });
            }
            foreach (var item in caseData.Cases.Plaintiffs)
            {
                caseFormVm.PlaintiffLists.Add(new PersonInfo()
                {
                    Id = caseData.Cases.Plaintiffs.IndexOf(item),
                    FullName_Cn = item.FullName_Cn,
                    FullName_En = item.FullName_En,
                    Type = item.Type + 1
                });
            }
            foreach (var item in caseData.Cases.Judges)
            {
                caseFormVm.JudgeLists.Add(new PersonInfo()
                {
                    Id = caseData.Cases.Judges.IndexOf(item),
                    FullName_Cn = item.FullName_Cn,
                    FullName_En = item.FullName_En,
                    Type = item.Type + 1
                });
            }
            foreach (var item in caseData.Cases.Defendants_Representations)
            {
                caseFormVm.RepresentationDLists.Add(new PersonInfo()
                {
                    Id = caseData.Cases.Defendants_Representations.IndexOf(item),
                    FullName_Cn = item.FullName_Cn,
                    FullName_En = item.FullName_En,
                    Type = item.Type + 1
                });
            }
            foreach (var item in caseData.Cases.Plaintiffs_Representations)
            {
                caseFormVm.RepresentationPLists.Add(new PersonInfo()
                {
                    Id = caseData.Cases.Plaintiffs_Representations.IndexOf(item),
                    FullName_Cn = item.FullName_Cn,
                    FullName_En = item.FullName_En,
                    Type = item.Type + 1
                });
            }
        }

        /// <summary>
        /// 判断是否有这个账户
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static bool IsMemberName(string memberName)
        {
            try
            {
                var tmpModel = dbNewEmms.SingleOrDefault<MemberModel>(@"SELECT top 1 MemberID,MemberName from m_Member where MemberName=@0", memberName);
                if (tmpModel == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }

        }
        #endregion

        #region court（案件表）旧数据迁移

        /// <summary>
        /// 案件表迁移规则说明
        /// 
        /// 旧数据库新增一个标记表：courtUpdateFlag，（courtid，flag），
        /// 
        /// 1.从旧数据库获取【标记表中最大的courtid加1，flag:为1.】，以courtid+1获
        ///   取court（案件表）courtid对应的记录（而标记表不存在）。
        ///   如果court（案件表）不存在记录，则courtid继续加1，继续获取，直到一条有效记录。
        ///   
        ///   如果标记表无记录，取court（案件表）courtid最小的一条记录。
        /// 2.获取1中记录A，取得 Caseno and Actiondate 为条件
        /// 再次从旧数据库获取记录：
        /// 
        /// 原告及被告合并规则：
        ///   如有多条记录：对应Plaintiff，Defendant
        ///   合为一条记录，两属性去重复。
        ///   如：P1,D1
        ///       P2,D1
        ///       P3,D1
        ///       P1,D2
        ///       P2,D2
        ///       P3,D2
        /// 合并后为：P1,P2,P3为原告。D1,D2为被告。
        /// 3.处理完合并，得到一条记录，更新到新数据库。
        /// 同时，写入标记表。规则
        ///   记录A的courtid，flag为1，如有多条记录，其它的courtid，flag为0，写入标记表中。
        ///       
        /// 循环，从1开始。。。。
        /// ********************************************************************
        /// 正式案件编号处理方法
        ///  旧数据记录读取顺序：值不存在，取下个。
        ///  plainref3--》Defref3--》Caseno
        ///
        ///  案件编号，3000年前的处理方式，待定
        ///  ********************************************************************
        ///  地址及律师处理方法
        ///   plainref2
        ///   只有一个时放地址中，/前为地址,/后为律师
        ///   用一些关键字区分,,无/符，看关键字区分。。。
        /// ********************************************************************
        ///   Defref2 只有地址
        /// </summary>
        /// <returns></returns>
        public static void UpdateCourtToNewDB(object t)
        {
            var msg = "";

            objT o = (objT)t;
            SetEnable(o.f, o.btn, false);
            SetEnable(o.f, o.btn2, false);
            SetEnable(o.f, o.btn3, false);
            SetMsg(o.f, o.cl, msg, true);

            msg = string.Format("*****************[案件表]开始处理更新，时间：{0}**************************", DateTime.Now);
            logger.Debug(msg);
            try
            {
                //案件表，最大的Actiondate
                var icourtMaxActiondate = dbOld.SingleOrDefault<DateTime?>("select max(Actiondate) from court");
                if (!icourtMaxActiondate.HasValue)
                {
                    msg = "$$$$$$$$$$********************************案件表不存在记录,无法更新数据1。";
                    logger.Error(msg);
                    SetMsg(o.f, o.cl, msg, true);
                    return;
                }
                //获取标记表中的flag:1的最大记录。
                //
                var isFirst = false;

                var getCourtActiondate = dbOld.SingleOrDefault<DateTime?>("select max(Actiondate) from courtUpdateFlag");

                msg = string.Format("*************【标记表】中获得的最大Actiondate:{0}.", getCourtActiondate);
                logger.Debug(msg);
                SetMsg(o.f, o.cl, msg, true);
                //判断
                //不存在，取取court（案件表）courtid最小的一条记录
                if (!getCourtActiondate.HasValue)
                {
                    getCourtActiondate = dbOld.SingleOrDefault<DateTime?>("select min(Actiondate) from court");
                    msg = string.Format("*************【案件表】中获得的最小Actiondate:{0}.", getCourtActiondate);
                    logger.Debug(msg);
                    SetMsg(o.f, o.cl, msg, true);
                    isFirst = true;
                }

                if (!getCourtActiondate.HasValue)
                {
                    msg = string.Format("$$$$$$$$$$********************************案件表不存在记录,无法更新数据2。");
                    logger.Error(msg);
                    SetMsg(o.f, o.cl, msg, true);
                    SetMsg(o.f, o.f, o.f.Tag.ToString(), false);
                    return;
                }
                msg = string.Format("*************获得的最大Actiondate:{0}.", getCourtActiondate);
                logger.Debug(msg);
                SetMsg(o.f, o.cl, msg, true);
                //加1获取记录。不是第一次运行。
                if (!isFirst)
                {
                    getCourtActiondate = getCourtActiondate.Value.AddDays(1);
                }
                // 取court（案件表）courtid对应的记录（而标记表不存在）。
                // 如果court（案件表）不存在记录，则courtid继续加1，继续获取，直到一条有效记录。
                var tmpMinCourt = new court();
                msg = string.Format("*************案件表，最大的Actiondate:{0}.", icourtMaxActiondate);
                logger.Debug(msg);
                SetMsg(o.f, o.cl, msg, true);
                var currRunDate = getCourtActiondate.Value;
                while (currRunDate <= icourtMaxActiondate.Value)
                {

                    msg = string.Format("*************开始取案件表记录A，Actiondate:{0}.", currRunDate);
                    logger.Debug(msg);
                    SetMsg(o.f, o.cl, msg, true);

                    tmpMinCourt = GetCourt(currRunDate);//旧数据库没有对应记录
                    if (tmpMinCourt != null)
                    {
                        //判断是否为标记表中记录。
                        var isFalgRow = IsCourtUpdateFlag(tmpMinCourt.ActionDate);
                        if (!isFalgRow)
                        {
                            //是否已更新记录
                            var isHasUpdate = IsCaseHasFlag(tmpMinCourt.courtid);
                            if (!isHasUpdate)
                            {
                                break;
                            }
                            else
                            {
                                msg = string.Format("*************新数库中已存在:案件表记录A，Actiondate:{0}.，取下一条:{1}", currRunDate, currRunDate.AddDays(1));
                                logger.Debug(msg);
                                logger.Info(msg);
                                SetMsg(o.f, o.cl, msg, true);
                            }
                        }
                        else
                        {
                            msg = string.Format("*************标记表中存在:案件表记录A，Actiondate:{0}.，取下一条:{1}", currRunDate, currRunDate.AddDays(1));
                            logger.Debug(msg);
                            SetMsg(o.f, o.cl, msg, true);
                        }
                    }
                    else
                    {
                        //旧数据库没有对应记录，取下一个新记录
                        DateTime tmpNextMin = DateTime.Now.AddYears(1);
                        try
                        {
                            tmpNextMin = dbOld.SingleOrDefault<DateTime>("select min(Actiondate) from court where Actiondate>@0", currRunDate);
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("****************取下一条大于当前Actiondate的最小值出错.{0}", ex);
                            tmpNextMin = DateTime.Now.AddYears(-300);
                        }

                        if (tmpNextMin > currRunDate)
                        {
                            msg = string.Format("*************案件表记录A，Actiondate:{0}.不存在，取下一条大于当前Actiondate的最小值:{1}", getCourtActiondate, tmpNextMin);
                            currRunDate = tmpNextMin.AddDays(-1);
                        }
                        else
                        {
                            tmpNextMin = currRunDate.AddDays(1);
                            msg = string.Format("*************案件表记录A，Actiondate:{0}.不存在，取下一条大于当前Actiondate的最小值:{1}", getCourtActiondate, tmpNextMin);
                        }
                        logger.Debug(msg);
                        SetMsg(o.f, o.cl, msg, true);
                    }
                    currRunDate = currRunDate.AddDays(1);
                }

                if (currRunDate > icourtMaxActiondate.Value)
                {
                    msg = string.Format("##########################*************已全部处理完成......案件表，最大的Actiondate:{0}.当前已经处理到：{1},。。。Time:{2}", icourtMaxActiondate, getCourtActiondate, DateTime.Now);
                    logger.Debug(msg);
                    SetMsg(o.f, o.cl, msg, true);
                    SetEnable(o.f, o.btn, true);
                    SetEnable(o.f, o.btn2, true);
                    SetEnable(o.f, o.btn3, true);
                    SetMsg(o.f, o.f, o.f.Tag.ToString(), false);
                    return;
                }

                //2.获取1中记录A，取得 Actiondate 为条件案件记录集
                var tmpAllCountList = dbOld.Fetch<court>(string.Format("select * from court where  Actiondate='{0}' ORDER BY courtid ", tmpMinCourt.ActionDate));

                SaveData(o, tmpMinCourt, tmpAllCountList);

                //循环，递归处理，下条。
                GC.Collect();
                ThreadPool.QueueUserWorkItem(new WaitCallback(DBlib.UpdateCourtToNewDB), t);
            }
            catch (Exception ex)
            {
                msg = string.Format("********************************当前更新出错*{0}.", ex);
                SetMsg(o.f, o.cl, msg, true);
                SetEnable(o.f, o.btn, true);
                SetEnable(o.f, o.btn2, true);
                SetEnable(o.f, o.btn3, true);
                logger.ErrorFormat("********************************当前更新出错*{0}.", ex);
                //throw ex;
                //循环，递归处理,下条。
                //UpdateCourtToNewDB(t);
                //throw ex;
            }
        }


        public static bool SaveData(objT o, court tmpMinCourt, List<court> tmpAllCountList)
        {
            var msg = "";
            try
            {
                if (tmpAllCountList == null)
                {
                    msg = string.Format("**************旧数据库获取的没有记录，ActionDate:{0}.", tmpMinCourt.ActionDate.Date);
                    SetMsg(o.f, o.cl, msg, true);
                    return true;
                }
                //初始要存入新数据库
                var getItemToSaveList = new List<m_Case_items>();
                //按Caseno分组
                var tmpAllGroupByCaseno = tmpAllCountList.GroupBy(e => e.Caseno).ToList();

                foreach (var item in tmpAllGroupByCaseno)
                {

                    //原告变量
                    var tmpStrPlaintiff = "";
                    //被告变量
                    var tmpStrDefendant = "";

                    var getCaseList = item.ToList();

                    msg = string.Format("*************合并整理,当前需合并记录数:{0} as {1}/{2}，Actiondate:{3},CaseNo:{4}。", getCaseList.Count, getItemToSaveList.Count, tmpAllGroupByCaseno.Count, tmpMinCourt.ActionDate.Date, item.Key);
                    logger.Debug(msg);
                    SetMsg(o.f, o.cl, msg, true);
                    try
                    {
                        tmpMinCourt = getCaseList[0];
                        //是否已更新记录
                        //var isHasUpdate = IsCaseHasFlag(tmpMinCourt.courtid);
                        //if (isHasUpdate)
                        //{
                        //    msg = string.Format("*************新数库中已存在:案件表记录A，Actiondate:{0}.", tmpMinCourt.ActionDate);
                        //    logger.Debug(msg);
                        //    logger.Info(msg);
                        //    SetMsg(o.f, o.cl, msg, true);

                        //    continue;
                        //}
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorFormat("********{0}", ex);
                        continue;
                    }

                    //合并原告，被告
                    foreach (var item2 in getCaseList)
                    {
                        var tmpGetPlaintiff = item2.Plaintiff + " " + item2.Cplaintiff;
                        var tmpGetDefendant = item2.Defendant + " " + item2.Cdefendant;


                        //合并原告，被告
                        if (!string.IsNullOrWhiteSpace(tmpGetPlaintiff))
                        {
                            tmpGetPlaintiff = tmpGetPlaintiff.Trim();
                            if (string.IsNullOrWhiteSpace(tmpStrPlaintiff))
                            {
                                tmpStrPlaintiff = tmpGetPlaintiff;
                            }
                            else
                            {
                                if (!tmpStrPlaintiff.Contains(tmpGetPlaintiff))
                                {
                                    tmpStrPlaintiff += "," + tmpGetPlaintiff;
                                }
                            }
                        }

                        //被告
                        if (!string.IsNullOrWhiteSpace(tmpGetDefendant))
                        {
                            tmpGetDefendant = tmpGetDefendant.Trim();
                            if (string.IsNullOrWhiteSpace(tmpStrDefendant))
                            {
                                tmpStrDefendant = tmpGetDefendant;
                            }
                            else
                            {
                                if (!tmpStrDefendant.Contains(tmpGetDefendant))
                                {
                                    tmpStrDefendant += "," + tmpGetDefendant;
                                }
                            }

                        }

                    }

                    //地址或律师 原告
                    var tmpAddr = "";
                    var tmpPlainRef2 = "";
                    if (!string.IsNullOrWhiteSpace(tmpMinCourt.PlainRef2))
                    {
                        if (tmpMinCourt.PlainRef2.IndexOf(" / ") > -1)
                        {
                            var v = tmpMinCourt.PlainRef2.Replace(" / ", "|");
                            var dd = v.Split('|');
                            tmpAddr = dd[0].Trim();
                            tmpPlainRef2 = dd[1].Trim();

                        }
                        else
                        {
                            if (checkCourt(tmpMinCourt.PlainRef2))
                            {
                                tmpPlainRef2 = tmpMinCourt.PlainRef2;
                            }
                            else
                            {
                                tmpAddr = tmpMinCourt.PlainRef2;
                            }
                        }
                    }

                    //律师 被告
                    //f.	Defref3包含爬虫数据的full case no/律师名/其他，“others”放在其他1项目中
                    var tmpDefref3 = "";
                    var tmpfullcaseno = "";
                    var tmpOther1 = "";
                    if (!string.IsNullOrWhiteSpace(tmpMinCourt.Defref3))
                    {
                        if (tmpMinCourt.Defref3.IndexOf(" / ") > -1)
                        {
                            var v = tmpMinCourt.Defref3.Replace(" / ", "|");
                            var dd = v.Split('|');

                            //caseno 检测
                            if (IsRegCaseNo(dd[0]))
                            {
                                tmpfullcaseno = dd[0];
                            }
                            else
                            {
                                tmpOther1 = dd[0].Trim();
                            }

                            tmpDefref3 = dd[1].Trim();

                            if (dd.Length > 1)
                            {
                                for (int i = 2; i < dd.Length; i++)
                                {
                                    if (IsRegCaseNo(dd[i]))
                                    {
                                        tmpfullcaseno = dd[i];
                                    }

                                    tmpOther1 += " / " + dd[i];
                                }
                            }

                        }
                        else
                        {
                            //caseno 检测
                            if (IsRegCaseNo(tmpMinCourt.Defref3))
                            {
                                tmpfullcaseno = tmpMinCourt.Defref3;
                            }
                            else
                            {
                                //律师名
                                if (checkCourt(tmpMinCourt.Defref3))
                                {
                                    tmpDefref3 = tmpMinCourt.Defref3;
                                }
                                else
                                {
                                    tmpOther1 = tmpMinCourt.Defref3;
                                }
                            }

                        }
                        //多个caseno 存other
                        if (IsRegCaseNoCount(tmpMinCourt.Defref3) > 1)
                        {
                            tmpOther1 = tmpMinCourt.Defref3;
                        }

                    }

                    //案件编号
                    // 正式案件编号处理方法
                    //  旧数据记录读取顺序：值不存在，取下个。
                    //  plainref3--》Defref3--》Caseno
                    //c.	案件编号不完整的部分，如果在其他列找不到full case no，在字母后加上XXXX，年份通过 ActionDate补充完整。例： H1234/3002---HXXX1234/1999
                    var tmpCaseNo = string.IsNullOrWhiteSpace(tmpMinCourt.PlainRef3) ? tmpfullcaseno : tmpMinCourt.PlainRef3.Trim();

                    //caseno 检测
                    if (!IsRegCaseNo(tmpCaseNo))
                    {
                        try
                        {
                            tmpCaseNo = tmpMinCourt.Caseno;
                            var dd = getStrRemoveNumber(tmpMinCourt.Caseno);
                            if (!string.IsNullOrWhiteSpace(dd))
                            {
                                var tmpnum = tmpMinCourt.Caseno.Substring(dd.Length).Trim();
                                if (string.IsNullOrEmpty(tmpnum))
                                {
                                    tmpnum = "001";
                                }
                                tmpCaseNo = dd + "XXXX".Substring(0, 4 - dd.Length) + tmpnum;
                                if (tmpMinCourt.ActionDate != null)
                                {
                                    tmpCaseNo += "/" + tmpMinCourt.ActionDate.ToString("yyyy");
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("#########案件编号出错。{0}", ex);
                            tmpCaseNo = tmpMinCourt.Caseno;
                        }
                    }

                    tmpCaseNo = tmpCaseNo.Replace('&', ',');
                    tmpCaseNo = regDoubleSpace(tmpCaseNo);
                    //赋值到正式表 
                    var getItem = new m_Case_items();

                    getItem.htmlID = 0;
                    getItem.Language = 0;
                    getItem.tIndex = 1;
                    getItem.tkeyNo = tmpMinCourt.courtid.ToString();

                    getItem.CaseNo = regChinese(tmpCaseNo);
                    //getItem.CaseTypeID = regDoubleSpace(tmpMinCourt.CaseType);
                    //getItem.Cause = item.Cause;
                    getItem.ClientIP = "";
                    getItem.CourtDay = tmpMinCourt.ActionDate;//.ToString("yyyy-MM-dd");
                    getItem.CourtID = "";
                    getItem.Year = tmpMinCourt.ActionDate.ToString("yyyy");
                    getItem.Hearing = "";
                    getItem.Nature = regDoubleSpace(tmpMinCourt.Cause);

                    getItem.Defendant = regDoubleSpace(tmpStrDefendant);
                    getItem.PlainTiff = regDoubleSpace(tmpStrPlaintiff);

                    //地址或律师
                    getItem.P_Address = regDoubleSpace(tmpAddr);
                    getItem.Representation_P = regDoubleSpace(tmpPlainRef2);

                    getItem.D_Address = regDoubleSpace(tmpMinCourt.Defref2);
                    getItem.Representation_D = regDoubleSpace(tmpDefref3);


                    //getItem.Remark = regDoubleSpace(tmpMinCourt.Remark);
                    //getItem.Representation = regDoubleSpace(tmpMinCourt.Representation);
                    //getItem.Representation_P = regDoubleSpace(tmpMinCourt.Representation_P);
                    //getItem.Representation_D = regDoubleSpace(tmpMinCourt.Representation_D);

                    //getItem.SerialNo = tmpMinCourt.SerialNo;
                    getItem.tname = "OldDB";
                    getItem.tStatus = 1;
                    getItem.ttype = "OldDB";
                    getItem.addtime = DateTime.Now;
                    getItem.updtime = DateTime.Now;

                    //getItem.Actiondate = item.Actiondate;
                    getItem.Currency = tmpMinCourt.amountcurrency;
                    getItem.Amount = tmpMinCourt.amount;
                    //getItem.CheckField = item.CheckField;

                    //关键字处理，放其他
                    if (!string.IsNullOrWhiteSpace(getItem.PlainTiff) || !string.IsNullOrWhiteSpace(getItem.Defendant))
                    {
                        getItem.Parties = getItem.PlainTiff + " @and@ " + getItem.Defendant;
                    }

                    if (checkPDtoOther(getItem.Parties))
                    {
                        getItem.Other = getItem.Parties;
                    }
                    getItem.Remark = tmpMinCourt.Caseno + "," + tmpMinCourt.PlainRef3 + "," + tmpfullcaseno;

                    if (!string.IsNullOrWhiteSpace(tmpOther1))
                    {
                        if (!string.IsNullOrWhiteSpace(getItem.Other))
                        {
                            getItem.Other += " " + tmpOther1 + " " + getItem.Remark;
                        }
                        else
                        {
                            getItem.Other = tmpOther1 + " " + getItem.Remark; ;
                        }
                    }
                    //解封令，不显示
                    //caseno： HCB+""
                    //Cause 中有 关键字
                    if (!string.IsNullOrWhiteSpace(getItem.CaseNo))
                    {
                        try
                        {
                            var tmpcaseNo = getItem.CaseNo.ToUpper().Trim();
                            var tmpcaseNoIndex = tmpcaseNo.IndexOf("HCB");
                            if (tmpcaseNoIndex > -1)
                            {
                                tmpcaseNo = tmpcaseNo.Substring(tmpcaseNoIndex);
                                //包含 Cause 关键字才处理
                                if (checkHCB(tmpMinCourt.Cause))
                                {
                                    if (tmpcaseNo.Length > 3)
                                    {
                                        var str4 = tmpcaseNo.Substring(3, 1);
                                        if (!str4.Equals("/"))
                                        {
                                            try
                                            {
                                                int tmp4 = -1;
                                                if (int.TryParse(str4, out tmp4))
                                                {
                                                    getItem.Enable = 0;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                msg = string.Format("****************** 解封令，不显示,caseno：HCB开始，不包其它字母,caseno:{0},Defref3:{1},ActionDate:{2},Error:{3}", tmpcaseNo, tmpMinCourt.Defref3, tmpMinCourt.ActionDate.Date, ex.Message);
                                                logger.Error(msg);
                                                SetMsg(o.f, o.cl, msg, true);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            msg = string.Format("****************** 解封令，不显示,caseno:{0},Defref3:{1},ActionDate:{2},Error:{3}", getItem.CaseNo, tmpMinCourt.Defref3, tmpMinCourt.ActionDate.Date, ex.Message);
                            logger.Error(msg);
                            SetMsg(o.f, o.cl, msg, true);
                        }

                    }

                    getItem.Remark = tmpMinCourt.Caseno + "," + tmpMinCourt.PlainRef3 + "," + tmpfullcaseno;


                    getItemToSaveList.Add(getItem);
                }




                ///////////保存
                if (AddmCaseItems(getItemToSaveList, o))
                {
                    //写新数据成功。
                    //更新标记
                    updateOldFlag(tmpMinCourt.ActionDate, o);
                    return true;
                }
                else
                {
                    msg = string.Format("******************更新失败。案件IDActiondate:{0}", tmpMinCourt.ActionDate.Date);
                    logger.Error(msg);
                    SetMsg(o.f, o.cl, msg, true);
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public static string ReplaceShort(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return str;
                }

                string shortKeys = @" HKSAR | Government of HKSAR @ HKSAR[\(\（]{1}| Government of HKSAR(@ Co\.[\,]? | Company @ Co\.\, | Company @ Ltd[\. ]? | Limited @ & | And @ \+ | Plus @ Pte[\.]? | Private ";
                shortKeys += @"";

                Regex reg1 = new Regex(@"\u3000", RegexOptions.IgnoreCase);
                Regex reg2 = new Regex(@"Government of HKSAR", RegexOptions.IgnoreCase);
                Regex reg3 = new Regex(@"[\-]", RegexOptions.IgnoreCase);
                Regex reg4 = new Regex(@"[\u3000\u0020]{2,}", RegexOptions.IgnoreCase);//去空格,两个以上空格改为一个，全角、半角

                string strReplaceu3000 = reg1.Replace(str, " ");
                strReplaceu3000 = reg3.Replace(strReplaceu3000, " ");//原告被告把横杠改为空格
                string strReplace = " " + reg2.Replace(strReplaceu3000, "HKSAR") + " ";

                var shortArr = shortKeys.Split('@');

                foreach (var item in shortArr)
                {
                    try
                    {
                        var tmpArr = item.Split('|');
                        Regex TmpRegex = new Regex(tmpArr[0], RegexOptions.IgnoreCase);
                        var getRegex = TmpRegex.Match(tmpArr[1]);
                        strReplace = TmpRegex.Replace(strReplace, tmpArr[1]);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
                strReplace = reg4.Replace(strReplace, " ");
                return strReplace.Trim();
            }
            catch (Exception)
            {
                return str;
            }
        }
        /// <summary>
        /// 去双空格
        /// </summary>
        /// <param name="strV"></param>
        /// <returns></returns>
        public static string regDoubleSpace(string strV)
        {
            var v = strV;
            try
            {
                var reg1 = @"[\f\n\r\t\v]";//去除换行符等
                var reg2 = @"[\u3000\u0020]{2,}";//去空格,两个以上空格改为一个，全角、半角


                v = ReplaceReg(reg1, v, " ");
                v = ReplaceReg(reg2, v, " ");

                //缩写处理
                v = ReplaceShort(v);

                return v.Trim();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("*************{0}", ex);
                return v;
            }
        }
        /// <summary>
        /// 去汉字
        /// </summary>
        /// <param name="strV"></param>
        /// <returns></returns>
        public static string regChinese(string strV)
        {
            var v = strV;
            try
            {
                var reg1 = @"[\u4e00-\u9fa5]+";//去除换行符等


                v = ReplaceReg(reg1, v, "");

                return v.Trim();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("*************{0}", ex);
                return v;
            }
        }
        /// <summary>
        /// 去除数字，返回字母
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static string getStrRemoveNumber(string v)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(v))
                {
                    return v;
                }
                var reg1 = @"[0-9]+";//去除多个数字，得到字母


                v = ReplaceReg(reg1, v, " ");

                return v;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("*************{0}", ex);
                return v;
            }
        }
        static string ReplaceReg(string patt, string input, string totxt)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return "";
                }
                Regex rgx = new Regex(patt, RegexOptions.IgnoreCase);
                return rgx.Replace(input.ToString(), totxt).Trim();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("*************{0}", ex);
                return input;
            }
        }
        /// <summary>
        /// 是否为caseNo
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsRegCaseNo(string input)
        {
            return IsReg(@"[0-9]{1,}\/[0-9]{2,}", input);
        }
        public static int IsRegCaseNoCount(string input)
        {
            return IsRegCount(@"[0-9]{1,}\/[0-9]{2,}", input);
        }
        /// <summary>
        /// 判断
        /// </summary>
        /// <param name="patt"></param>
        /// <param name="input"></param>
        /// <param name="totxt"></param>
        /// <returns></returns>
        public static bool IsReg(string patt, string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return false;
                }
                Regex rgx = new Regex(patt, RegexOptions.IgnoreCase);

                return rgx.IsMatch(input);

            }
            catch (Exception ex)
            {
                logger.ErrorFormat("*************{0}", ex);
                return false;
            }
        }
        /// <summary>
        /// 判断有多少个
        /// </summary>
        /// <param name="patt"></param>
        /// <param name="input"></param>
        /// <param name="totxt"></param>
        /// <returns></returns>
        public static int IsRegCount(string patt, string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return 0;
                }
                Regex rgx = new Regex(patt, RegexOptions.IgnoreCase);

                var m2 = rgx.Match(input);
                var tmpReturn = 0;
                while (m2.Success)
                {
                    tmpReturn++;
                    m2 = m2.NextMatch();
                }
                return tmpReturn;

            }
            catch (Exception ex)
            {
                logger.ErrorFormat("*************{0}", ex);
                return 0;
            }
        }

        /// <summary> 
        /// HCB 解封破产令
        /// </summary>
        /// <param name="cause"></param>
        /// <returns></returns>
        static bool checkHCB(string cause)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cause))
                {
                    return false;
                }
                return (
                    cause.IndexOf("Notice To Creditors By Trustee Under Bankruptcy Ordinance", StringComparison.InvariantCultureIgnoreCase) > -1
                    ) ? true : false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("*************{0}", ex);
                return false;
            }
        }
        static bool checkCourt(string tmpen)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tmpen))
                {
                    return false;
                }
                return (
                    tmpen.IndexOf("& co", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                    tmpen.IndexOf("associates", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                    tmpen.IndexOf("parters", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                    tmpen.IndexOf("sohicitors", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                    tmpen.IndexOf("hotar", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                    tmpen.IndexOf("LLP. In person", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                    tmpen.IndexOf("legal department", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                    tmpen.IndexOf("department of justice", StringComparison.InvariantCultureIgnoreCase) > -1
                    ) ? true : false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("*************{0}", ex);
                return false;
            }
        }
        /// <summary>
        /// h.	原被告互相告的案子通过以下关键字区分： 
        /// defend in counterclaim /claimment in counterclaim。 
        /// defend in counterclaim+原告名， 
        /// claimment in counterclaim+被告名
        /// 放在其他1项目中，并显示在前台的报告中
        /// </summary>
        /// <param name="tmpen"></param>
        /// <returns></returns>
        static bool checkPDtoOther(string tmpen)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tmpen))
                {
                    return false;
                }
                return (
                tmpen.IndexOf("defend in counterclaim", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                tmpen.IndexOf("claimment in counterclaim", StringComparison.InvariantCultureIgnoreCase) > -1
                ) ? true : false;

            }
            catch (Exception ex)
            {
                logger.ErrorFormat("*************{0}", ex);
                return false;
            }

        }
        /// <summary>
        /// 判断是否早已更新的记录,新数据库
        /// </summary>
        /// <param name="courtid"></param>
        /// <returns></returns>
        public static bool IsCaseHasFlag(long courtid)
        {
            try
            {
                var tmpModel = dbWebGetData.Fetch<string>(string.Format(@"SELECT top 1 tkeyNo from m_Case_items where tkeyNo=N'{0}'", courtid));

                var tmpModelNew = dbNewEmms.Fetch<string>(string.Format(@"SELECT top 1 oldDBCourtID from [dbo].[t_Case] where [oldDBCourtID]=N'{0}'", courtid));

                if (tmpModel.Count <= 0 && tmpModelNew.Count <= 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("courtid:{0},Error:{1}", courtid, ex);
                return false;
            }

        }
        /// <summary>
        /// 判断是否为标记表中的记录,旧数据库
        /// </summary>
        /// <param name="Actiondate"></param>
        /// <returns></returns>
        public static bool IsCourtUpdateFlag(DateTime Actiondate)
        {
            try
            {
                var tmpModel = dbOld.SingleOrDefault<courtUpdateFlag>(@"SELECT top 1 * from courtUpdateFlag where Actiondate=@0 ", Actiondate);
                if (tmpModel == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("ActionDate:{0},Error:{1}", Actiondate, ex);
                return false;
            }

        }
        /// <summary>
        /// 获取案件表，记录,旧数据库
        /// </summary>
        /// <param name="actiondate"></param>
        /// <returns></returns>
        public static court GetCourt(DateTime actiondate)
        {
            try
            {
                var tmpModel = dbOld.SingleOrDefault<court>(string.Format("SELECT top 1 * from court where Actiondate='{0}' ORDER BY courtid ", actiondate));
                return tmpModel;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("ActionDate:{0},Error:{1}", actiondate, ex);
                return null;
            }

        }
        #endregion

        #region PanamaDB数据处理
        /// <summary>
        /// PanamaDB数据
        /// </summary>
        /// <param name="t"></param>
        public static void UpdatePanamaDBToNewDB(object t)
        {
            var msg = "";

            objT o = (objT)t;
            SetEnable(o.f, o.btn, false);
            SetEnable(o.f, o.btn2, false);
            SetEnable(o.f, o.btn3, false);
            SetMsg(o.f, o.cl, msg, true);

            msg = string.Format("*****************[PanamaDB数据]开始处理更新，时间：{0}**************************", DateTime.Now);
            logger.Debug(msg);
            try
            {

                //处理逻辑，

                //A:先导入数据
                //1.公司及个人资料，根据公司关键字设定，判定是否为公司，存在对应表中（s_Company，s_Person）。
                //2.地址直接存入对应地址表中。

                //B:再生成关系【t_PublicRelation（公共关系表）】

                #region A导入数据
                toSavePananmaDataToNew(o);
                #endregion

            }
            catch (Exception ex)
            {
                msg = string.Format("********************************当前更新出错*{0}.", ex);
                SetMsg(o.f, o.cl, msg, true);
                SetEnable(o.f, o.btn, true);
                SetEnable(o.f, o.btn2, true);
                SetEnable(o.f, o.btn3, true);
                logger.ErrorFormat("********************************当前更新出错*{0}.", ex);

                //throw ex;
                //循环，递归处理,下条。
                //UpdateCourtToNewDB(t);
                //throw ex;
            }
        }
        static int CheckComp(string v, List<MKeyWordModel> tmpGetComName)
        {
            //处理公司关建字
            #region 处理公司关建字
            if (tmpGetComName != null)
            {
                try
                {
                    if (tmpGetComName.Count > 0)
                    {
                        foreach (var itemkey in tmpGetComName)
                        {
                            var tmpC = reType(itemkey.WordName, v);
                            if (tmpC > 1)
                            {
                                return tmpC;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    return 1;
                }

            }
            return 1;
            #endregion
        }
        static void toSavePananmaDataToNew(object t)
        {
           
            var msg = "";
            try
            {
                objT o = (objT)t;
                //获取公司关键字           

                if (_tmpGetComName.Count <= 0)
                {
                    _tmpGetComName = dbNewEmms.Fetch<MKeyWordModel>("select * from m_ComWord order by DspNo");

                    msg = string.Format("********************************获取公司关键字。记录数:{0}.", _tmpGetComName.Count);
                    SetMsg(o.f, o.cl, msg, true);
                }

                msg = string.Format("********************************开始取旧数据：{0} 条. Time:{1}", 300, DateTime.Now);
                SetMsg(o.f, o.cl, msg, true);
                logger.Debug(msg);

                var tmpEntityidTop300 = new List<EntitysAll>();

                if (!isRunRelite)
                {
                    tmpEntityidTop300 = dbPanamaDB.Fetch<EntitysAll>("SELECT top 300 * from EntitysAll where (Entityid is NULL or Entityid <=0)");
                    msg = string.Format("********************************取到旧数据：{0} 条. Time:{1}", tmpEntityidTop300.Count, DateTime.Now);
                    SetMsg(o.f, o.cl, msg, true);
                    logger.Debug(msg);
                }

                if (tmpEntityidTop300 != null)
                {
                    if (tmpEntityidTop300.Count > 0)
                    {

                        using (var scope = dbNewEmms.GetTransaction())
                        {
                            #region save to new
                            foreach (var item in tmpEntityidTop300)
                            {


                                item.nameDesc = item.nameDesc ?? "";
                                item.ttype = item.ttype ?? "";

                                item.nameDescSearch = item.nameDescSearch ?? item.nameDesc;

                                msg = string.Format("#*****************开始处理：id:{1},name:{0},url:{2}, Time:{3}",
                                    item.name,
                                    item.nameDescSearch,
                                    item.nameURL,
                                    DateTime.Now);
                                SetMsg(o.f, o.cl, msg, true);
                                logger.Debug(msg);


                                if (item.ttype.ToLower().Equals("address"))
                                {

                                    #region address

                                    var tmpAddress = new AddressModel()
                                    {
                                        Address = item.nameDescSearch
                                    };

                                    dbNewEmms.Insert(tmpAddress);

                                    //更新旧ID
                                    item.Entityid = tmpAddress.AddressID;

                                    #endregion
                                }
                                else
                                {
                                    #region no address
                                    var tmpType = 1;
                                    //Entity/Intermediary/Officer
                                    //officer 公司判断。
                                    //Entity/Intermediary为公司
                                    tmpType = CheckComp(item.nameDescSearch, _tmpGetComName);
                                    if (tmpType <= 1)
                                    {
                                        if (!item.ttype.ToLower().Equals("officer"))
                                        {
                                            tmpType = 2;
                                        }
                                        else
                                        {
                                            tmpType = 1;
                                        }
                                    }

                                    //s_Entity
                                    var entityModel = new EntityModel()
                                    {
                                        Param1 = 0,//表示原告  
                                        Type = tmpType,
                                        Flag = 5
                                    };
                                    dbNewEmms.Insert(entityModel);

                                    //更新旧ID
                                    item.Entityid = entityModel.Entityid;

                                    if (tmpType == 1)
                                    {
                                        var tmpP = new PersonModel()
                                        {
                                            FullName_En = item.nameDescSearch ?? "",
                                            Entityid = entityModel.Entityid,
                                            Show = 2,
                                            Remarks = item.name
                                        };
                                        if (tmpP.FullName_En.Length > 128)
                                        {
                                            tmpP.FullName_En = tmpP.FullName_En.Substring(0, 127);
                                            tmpP.Remarks += "," + item.nameDescSearch;
                                            if (tmpP.Remarks.Length > 255)
                                            {
                                                tmpP.Remarks = tmpP.Remarks.Substring(0, 255);
                                            }
                                        }

                                        //添加个人 
                                        dbNewEmms.Insert(tmpP);
                                    }
                                    else
                                    {
                                        var tmpC = new CompanyModel()
                                        {
                                            FullName_En = item.nameDescSearch ?? "",
                                            Entityid = entityModel.Entityid,
                                            Show = 2,
                                            DissolutionDate = item.DormDate ?? "",
                                            ActiveStatus = item.Status ?? "",

                                            Remarks = item.name,
                                            AddDatetime = DateTime.Now
                                        };
                                        if (tmpC.FullName_En.Length > 128)
                                        {
                                            tmpC.FullName_En = tmpC.FullName_En.Substring(0, 127);
                                            tmpC.Remarks += "," + item.nameDescSearch;
                                            if (tmpC.Remarks.Length > 255)
                                            {
                                                tmpC.Remarks = tmpC.Remarks.Substring(0, 255);
                                            }
                                        }

                                        // 添加公司
                                        dbNewEmms.Insert(tmpC);

                                    }
                                    #endregion

                                }


                            }
                            #endregion


                            scope.Complete();
                            msg = string.Format("********************************提交旧数据：{0} 条 到新数据库，成功。.", tmpEntityidTop300.Count);
                            SetMsg(o.f, o.cl, msg, true);
                            logger.Debug(msg);

                            //保存旧数据Entity
                            using (var scopeOld = dbPanamaDB.GetTransaction())
                            {
                                foreach (var item in tmpEntityidTop300)
                                {
                                    dbPanamaDB.Update(item, new List<string>() { "Entityid" });
                                }
                                scopeOld.Complete();

                                msg = string.Format("********************************更新旧数据更新标记：{0} 条，成功。.", tmpEntityidTop300.Count);
                                SetMsg(o.f, o.cl, msg, true);
                                logger.Debug(msg);

                            }
                        }

                        //循环一次，全部ok，开始跑下一300个******************
                        //重新新线程
                        GC.Collect();
                        ThreadPool.QueueUserWorkItem(new WaitCallback(DBlib.toSavePananmaDataToNew), t);

                    }
                    else
                    {
                        msg = string.Format("********************************当前基本数据已全部更新完成。*Time:{0}.", DateTime.Now);
                        SetMsg(o.f, o.cl, msg, true);
                        logger.Debug(msg);
                        logger.Info(msg);

                        isRunRelite = true;
                        //SetEnable(o.f, o.btn, true);
                        //SetEnable(o.f, o.btn2, true);
                        //SetEnable(o.f, o.btn3, true);
                        //logger.Error(msg);

                        //开始处理公共关系（t_PublicRelation）
                        //A:获取m_talbe 中 t_Panama 对应的 TableID
                        //存数据到t_Panama中，取PublicID
                        //关联关系：根据旧数据Connections，生成关系。Entityid
                        //地址类：与实体Entityid，对应关系（t_Address），其它为t_PublicRelation

                        var getTableID = dbNewEmms.FirstOrDefault<long>("select top 1 TableID from m_Table where NewTable='t_Panama'");

                        msg = string.Format("********************************获取t_Panama 对应的 TableID:{0}。*Time:{1}.", getTableID, DateTime.Now);
                        SetMsg(o.f, o.cl, msg, true);
                        logger.Debug(msg);
                        logger.Info(msg);

                        if (getTableID > 0)
                        {
                            //
                            var tmpEntityidTot_PanamaTop300 = dbPanamaDB.Fetch<EntitysAll>("SELECT top 300 * from EntitysAll where tStatus <> 5");

                            if (tmpEntityidTot_PanamaTop300.Count > 0)
                            {
                                #region 关系处理逻辑
                                using (var scope = dbNewEmms.GetTransaction())
                                {

                                    foreach (var item in tmpEntityidTot_PanamaTop300)
                                    {
                                        item.ttype = item.ttype ?? "";
                                        msg = string.Format("#************#开始处理关系：id:{1},name:{0},url:{2}, Time:{3}",
                                              item.name,
                                              item.nameDescSearch,
                                              item.nameURL,
                                              DateTime.Now);
                                        SetMsg(o.f, o.cl, msg, true);
                                        logger.Debug(msg);

                                        if (item.ttype.ToLower().Equals("address"))
                                        {
                                            msg = string.Format("#************#当前为地址，处理下个：Type:{0},id:{1},url:{2}, Time:{3}",
                                              item.ttype,
                                              item.name,
                                              item.nameURL,
                                              DateTime.Now);
                                            SetMsg(o.f, o.cl, msg, true);
                                            logger.Debug(msg);

                                            continue;
                                        }

                                        var tmpToSavet_Panama = new t_Panama()
                                        {
                                            Address = item.Address,
                                            CompanyType = item.CompanyType,
                                            Countries = item.Countries,
                                            DormDate = item.DormDate,
                                            Jurisdiction = item.Jurisdiction,
                                            Name_EN = item.nameDescSearch,
                                            Remark = item.Remark,
                                            Source = item.Source,
                                            Status = item.Status,
                                            Type = item.ttype,
                                            Show = 2,
                                            Enable = 1,
                                            DataGradeID = 0,
                                            Language = 0

                                        };
                                        dbNewEmms.Insert(tmpToSavet_Panama);
                                        #region 更新关系

                                        //获取关系
                                        var tmpGetConnect = dbPanamaDB.Fetch<ConnectionsVM>(string.Format("select b.Entityid as EntityidTo,b.ttype as TypeTo,c.Entityid as EntityidFrom,c.ttype as TypeFrom,a.* from Connections a  LEFT JOIN EntitysAll b on a.nameTo=b.name LEFT JOIN EntitysAll c on a.nameFrom=c.name where a.nameTo='{0}' or a.nameFrom='{1}'", item.name, item.name));
                                        var dicEntity = new Dictionary<long, string>();
                                        dicEntity.Add(item.Entityid, item.ttype);

                                        if (tmpGetConnect.Count > 0)
                                        {
                                            foreach (var tmpC in tmpGetConnect)
                                            {
                                                tmpC.TypeTo = tmpC.TypeTo ?? "";
                                                tmpC.TypeFrom = tmpC.TypeFrom ?? "";

                                                if (tmpC.EntityidTo > 0)
                                                {
                                                    if (!dicEntity.ContainsKey(tmpC.EntityidTo))
                                                    {
                                                        dicEntity.Add(tmpC.EntityidTo, tmpC.TypeTo);
                                                    }
                                                }
                                                if (tmpC.EntityidFrom > 0)
                                                {
                                                    if (!dicEntity.ContainsKey(tmpC.EntityidFrom))
                                                    {
                                                        dicEntity.Add(tmpC.EntityidFrom, tmpC.TypeFrom);
                                                    }
                                                }
                                            }
                                        }
                                        //保存关系到新数据库
                                        //地址类address：只更新当前EntityId对应的实体【公司、个人】中的地址关系：t_Address
                                        foreach (var itemR in dicEntity)
                                        {
                                            if (itemR.Value.ToLower().Equals("address"))
                                            {
                                                if (itemR.Key == item.Entityid)
                                                {
                                                    continue;
                                                }

                                                var tmpGetAddress = new AddressModel()
                                                {
                                                    AddressID = itemR.Key,
                                                    Entityid = item.Entityid
                                                };
                                                dbNewEmms.Update(tmpGetAddress, new List<string>() { "Entityid" });
                                            }
                                            else
                                            {
                                                var tmpRelatePublic = new PublicRelationModel()
                                                    {
                                                        Entityid = itemR.Key,
                                                        PublicID = tmpToSavet_Panama.PublicID,
                                                        TableID = getTableID
                                                    };
                                                dbNewEmms.Insert(tmpRelatePublic);
                                            }

                                        }

                                        #endregion
                                        ////更新标记
                                        item.tStatus = 5;
                                    }

                                    scope.Complete();
                                    msg = string.Format("********************************提交旧数据：{0} 条 到新数据库，成功。.", tmpEntityidTot_PanamaTop300.Count);
                                    SetMsg(o.f, o.cl, msg, true);
                                    logger.Debug(msg);

                                    //保存旧数据Entity,tStatus
                                    using (var scopeOld = dbPanamaDB.GetTransaction())
                                    {
                                        foreach (var item in tmpEntityidTot_PanamaTop300)
                                        {
                                            dbPanamaDB.Update(item, new List<string>() { "tStatus" });
                                        }
                                        scopeOld.Complete();

                                        msg = string.Format("********************************更新旧数据更新标记：{0} 条，成功。.", tmpEntityidTot_PanamaTop300.Count);
                                        SetMsg(o.f, o.cl, msg, true);
                                        logger.Debug(msg);

                                    }
                                }


                                #endregion
                                ///////完成一次
                                //循环一次，全部ok，开始跑下一300个******************
                                //重新新线程
                                GC.Collect();
                                ThreadPool.QueueUserWorkItem(new WaitCallback(DBlib.toSavePananmaDataToNew), t);
                            }
                            else
                            {
                                msg = string.Format("********************************当前关系已全部更新完成。*Time:{0}.", DateTime.Now);
                                SetMsg(o.f, o.cl, msg, true);
                                logger.Debug(msg);
                                logger.Info(msg);
                                SetEnable(o.f, o.btn, true);
                                SetEnable(o.f, o.btn2, true);
                                SetEnable(o.f, o.btn3, true);
                            }
                        }
                        else
                        {
                            msg = string.Format("*********************************新数据库没有找到m_talbe 中 t_Panama 对应的 TableID。*Time:{0}.", DateTime.Now);
                            SetMsg(o.f, o.cl, msg, true);
                            logger.Error(msg);
                            SetEnable(o.f, o.btn, true);
                            SetEnable(o.f, o.btn2, true);
                            SetEnable(o.f, o.btn3, true);
                        }
                    }
                }
                else
                {
                    msg = string.Format("*********************************旧数据库中没有找到要更新地记录/或已更新完成。*Time:{0}.", DateTime.Now);
                    SetMsg(o.f, o.cl, msg, true);
                    SetEnable(o.f, o.btn, true);
                    SetEnable(o.f, o.btn2, true);
                    SetEnable(o.f, o.btn3, true);
                    logger.Error(msg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


    }

    public class objT
    {
        public Form f { get; set; }
        public Control cl { get; set; }
        public Button btn { get; set; }
        public Button btn2 { get; set; }
        public Button btn3 { get; set; }
    }
}
