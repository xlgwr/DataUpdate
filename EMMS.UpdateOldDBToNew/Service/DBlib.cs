using Common.Logging;
using EMMS.Common;
using EMMS.Domain;
using EMMS.Domain.Member;
using EMMS.UpdateOldDBToNew.modelNew;
using EMMS.UpdateOldDBToNew.modelOld;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMMS.UpdateOldDBToNew.Service
{
    public class DBlib
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 创建数据库连接 旧数据库
        /// </summary>
        public static Database dbOld = new PetaPoco.Database("MSSQLOld");
        /// <summary>
        /// 创建数据库连接 新数据库
        /// </summary>
        public static Database dbNew = new PetaPoco.Database("MSSQLNew");
        /// <summary>
        /// 创建数据库连接 爬虫数据库
        /// </summary>
        public static Database dbWeb = new PetaPoco.Database("MSSQLOffline");
        public static Form _form1 = null;

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

                        if (string.IsNullOrEmpty(cl.Text))
                        {
                            cl.Text = msg;
                        }
                        else
                        {
                            cl.Text = msg + "\n" + cl.Text;
                        }

                        if (cl.Text.Length > 1024 * 100)
                        {
                            cl.Text = cl.Text.Substring(0, 1024 * 100) + "......................";
                        }
                    }));
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("**********{0}.", ex);
                cl.Invoke(new Action(delegate()
                {
                    if (string.IsNullOrEmpty(cl.Text))
                    {
                        cl.Text = msg;
                    }
                    else
                    {
                        cl.Text = msg + "\n" + cl.Text;
                    }
                }));
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
                logger.ErrorFormat("**********{0}.", ex);
                cl.Invoke(new Action(delegate()
                {
                    cl.Enabled = isenable;
                }));
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
                    if (!string.IsNullOrEmpty(item.login))
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
                                if (!string.IsNullOrEmpty(item.btitle))
                                {
                                    if (!string.IsNullOrEmpty(MBModel.Remark))
                                    {
                                        MBModel.Remark = item.btitle + "@" + MBModel.Remark;
                                    }
                                    else
                                    {
                                        MBModel.Remark = item.btitle;
                                    }
                                }
                                if (!string.IsNullOrEmpty(item.hkid))
                                {
                                    if (!string.IsNullOrEmpty(MBModel.Remark))
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
                                    logger.DebugFormat(msg);
                                    SetMsg(o.f, o.cl, msg, true);
                                }
                                else
                                {
                                    ifailSaveAll++;
                                    msg = string.Format("*****当前记录更新出错，公司ID：{0},注册ID：{1}.", item.acno, tmpName);
                                    logger.ErrorFormat(msg);
                                    SetMsg(o.f, o.cl, msg, true);
                                }
                            }
                            catch (Exception ex)
                            {
                                ifailSaveAll++;
                                msg = string.Format("*****当前更新出错，公司ID：{0},注册ID：{1},Error:{2}. 继续下一个。", item.acno, tmpName, ex);
                                logger.ErrorFormat(msg);
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
                        logger.ErrorFormat(msg);
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
            return (tmpen.IndexOf("LIMITED", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                     tmpen.IndexOf("COMPANY", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                     tmpen.IndexOf("Ltd.", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                     tmpen.IndexOf("Co.", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                     tmpen.IndexOf("CO.,", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                     tmpen.IndexOf("公司", StringComparison.InvariantCultureIgnoreCase) > -1
                                    ) ? true : false;
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
                if (!string.IsNullOrEmpty(memberM.Password))
                {
                    memberM.Password = Encryption.Encode(memberM.Password);
                }
                MemberID = "";
                #region  个人
                if (!type)
                {
                    using (var scope = dbNew.GetTransaction())
                    {
                        MemberID = dbNew.Insert("m_Member", "MemberID", memberM).ToString();
                        scope.Complete();
                        rtnValue = true;
                    }
                }
                #endregion
                #region 公司
                else
                {
                    using (var scope = dbNew.GetTransaction())
                    {
                        string MemberComanyModelID = dbNew.Insert("m_MemberComany", "MemberComanyID", memberM.MemberComanyModel).ToString();
                        memberM.MemberComanyID = Convert.ToInt32(MemberComanyModelID);
                        for (int i = 0; i < memberM.ContactPersonModel.Count; i++)
                        {
                            memberM.ContactPersonModel[i].MemberComanyID = MemberComanyModelID;
                            dbNew.Insert("m_ContactPerson", "ContactPersonID", memberM.ContactPersonModel[i]).ToString();
                        }

                        MemberID = dbNew.Insert("m_Member", "MemberID", memberM).ToString();

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
        public static bool updateOldFlag(Dictionary<long, int> model, objT o)
        {
            bool rtnValue = false;
            try
            {
                using (var scope = dbOld.GetTransaction())
                {
                    var tmpAllcountID = "";
                    foreach (var item in model)
                    {
                        var tmpModel = new courtUpdateFlag();
                        tmpModel.courtid = item.Key;
                        tmpModel.flag = item.Value;

                        dbOld.Insert("courtUpdateFlag", "courtid", false, tmpModel).ToString();

                        if (string.IsNullOrEmpty(tmpAllcountID))
                        {
                            tmpAllcountID = item.Key + "|" + item.Value;
                        }
                        else
                        {
                            tmpAllcountID += "," + item.Key + "|" + item.Value;
                        }
                    }
                    var msg = string.Format("*************写入成功.案件表更新标记表ID:{0}.", tmpAllcountID);
                    logger.DebugFormat(msg);
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
        public static bool AddmCaseItems(m_Case_items model, objT o)
        {
            bool rtnValue = false;
            try
            {
                model.addtime = DateTime.Now;
                model.updtime = DateTime.Now;

                using (var scope = dbNew.GetTransaction())
                {
                    var CaseID = dbNew.Insert("m_Case_items", "Tid", true, model).ToString();

                    var msg = string.Format("*************写入成功.案件ID:{0},原告@被告：{1}, 新数据库ID:{2}.", model.tkeyNo, model.PlainTiff, CaseID);
                    logger.DebugFormat(msg);
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

        /// <summary>
        /// 判断是否有这个账户
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static bool IsMemberName(string memberName)
        {
            try
            {
                var tmpModel = dbNew.SingleOrDefault<MemberModel>(@"SELECT top 1 MemberID,MemberName from m_Member where MemberName=@0", memberName);
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
        ///  案件编号，2000年前的处理方式，待定
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
            SetMsg(o.f, o.cl, msg, true);

            msg = string.Format("*****************[案件表]开始处理更新，时间：{0}**************************", DateTime.Now);
            logger.DebugFormat(msg);
            try
            {
                //案件表，最大的ID
                var icourtMax = dbOld.SingleOrDefault<long>("select isnull(max(courtid),0) from court");
                if (icourtMax == 0)
                {
                    msg = "$$$$$$$$$$********************************案件表不存在记录,无法更新数据1。";
                    logger.Error(msg);
                    SetMsg(o.f, o.cl, msg, true);
                    return;
                }
                //获取标记表中的flag:1的最大记录。
                //
                var isFirst = false;

                var getCourtid = dbOld.SingleOrDefault<long>("select isnull(max(courtid),0) from courtUpdateFlag where flag=1");

                msg = string.Format("*************【标记表】中获得的最大courtid:{0}.", getCourtid);
                logger.DebugFormat(msg);
                SetMsg(o.f, o.cl, msg, true);
                //判断
                //不存在，取取court（案件表）courtid最小的一条记录
                if (getCourtid == 0)
                {
                    getCourtid = dbOld.SingleOrDefault<long>("select isnull(min(courtid),0) from court");
                    msg = string.Format("*************【案件表】中获得的最小courtid:{0}.", getCourtid);
                    logger.DebugFormat(msg);
                    SetMsg(o.f, o.cl, msg, true);
                    isFirst = true;
                }

                if (getCourtid == 0)
                {
                    msg = string.Format("$$$$$$$$$$********************************案件表不存在记录,无法更新数据2。");
                    logger.Error(msg);
                    SetMsg(o.f, o.cl, msg, true);
                    SetMsg(o.f, o.f, o.f.Tag.ToString(), false);
                    return;
                }
                msg = string.Format("*************获得的最大courtid:{0}.", getCourtid);
                logger.DebugFormat(msg);
                SetMsg(o.f, o.cl, msg, true);
                //加1获取记录。
                if (!isFirst)
                {
                    getCourtid++;
                }
                // 取court（案件表）courtid对应的记录（而标记表不存在）。
                // 如果court（案件表）不存在记录，则courtid继续加1，继续获取，直到一条有效记录。
                var tmpMinCourt = new court();
                msg = string.Format("*************案件表，最大的ID:{0}.", icourtMax);
                logger.DebugFormat(msg);
                SetMsg(o.f, o.cl, msg, true);

                while (getCourtid <= icourtMax)
                {

                    msg = string.Format("*************开始取案件表记录A，courtid:{0}.", getCourtid);
                    logger.DebugFormat(msg);
                    SetMsg(o.f, o.cl, msg, true);

                    tmpMinCourt = GetCourt(getCourtid);
                    if (tmpMinCourt != null)
                    {
                        //判断是否为标记表中记录。
                        var isFalgRow = IsCourtUpdateFlag(tmpMinCourt.courtid);
                        if (!isFalgRow)
                        {
                            var isHasUpdate = IsCaseHasFlag(tmpMinCourt.courtid);
                            if (!isHasUpdate)
                            {
                                break;
                            }
                            else
                            {
                                msg = string.Format("*************新数库中已存在:案件表记录A，courtid:{0}.，取下一条:{1}", getCourtid, getCourtid + 1);
                                logger.DebugFormat(msg);
                                SetMsg(o.f, o.cl, msg, true);
                            }
                        }
                        else
                        {
                            msg = string.Format("*************标记表中存在:案件表记录A，courtid:{0}.，取下一条:{1}", getCourtid, getCourtid + 1);
                            logger.DebugFormat(msg);
                            SetMsg(o.f, o.cl, msg, true);
                        }
                    }
                    else
                    {
                        long tmpNextMin = 0;
                        try
                        {
                            tmpNextMin = dbOld.SingleOrDefault<long>("select isnull(min(courtid),0) from court where courtid>@0", getCourtid);
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorFormat("****************取下一条大于当前Id的最小值出错.{0}", ex);
                        }

                        if (tmpNextMin > getCourtid)
                        {
                            msg = string.Format("*************案件表记录A，courtid:{0}.不存在，取下一条大于当前Id的最小值:{1}", getCourtid, tmpNextMin);
                            getCourtid = tmpNextMin - 1;
                        }
                        else
                        {
                            tmpNextMin = getCourtid + 1;
                            msg = string.Format("*************案件表记录A，courtid:{0}.不存在，取下一条大于当前Id的最小值:{1}", getCourtid, tmpNextMin);
                        }

                        logger.DebugFormat(msg);
                        SetMsg(o.f, o.cl, msg, true);
                    }
                    getCourtid++;
                }

                if (getCourtid > icourtMax)
                {
                    msg = string.Format("##########################*************已全部处理完成......案件表，最大的ID:{0}.当前已经处理到：{1},。。。", icourtMax, getCourtid);
                    logger.DebugFormat(msg);
                    SetMsg(o.f, o.cl, msg, true);
                    SetEnable(o.f, o.btn, true);
                    SetEnable(o.f, o.btn2, true);
                    SetMsg(o.f, o.f, o.f.Tag.ToString(), false);
                    return;
                }

                //2.获取1中记录A，取得 Caseno and Actiondate 为条件案件记录集
                var tmpAllCountList = dbOld.Fetch<court>("select * from court where Caseno=@0 and Actiondate=@1", tmpMinCourt.Caseno, tmpMinCourt.ActionDate);
                //合并生成
                var tmpDicCountIdToSaveFlag = new Dictionary<long, int>();//更新标记变量
                tmpDicCountIdToSaveFlag.Add(tmpMinCourt.courtid, 1);

                //原告变量
                var tmpStrPlaintiff = "";
                //被告变量
                var tmpStrDefendant = "";

                //合并原告，被告
                foreach (var item2 in tmpAllCountList)
                {
                    //记录，更新标记
                    if (!tmpDicCountIdToSaveFlag.ContainsKey(item2.courtid))
                    {
                        tmpDicCountIdToSaveFlag.Add(item2.courtid, 0);
                    }

                    //合并原告，被告
                    if (string.IsNullOrEmpty(tmpStrPlaintiff))
                    {
                        tmpStrPlaintiff = item2.Plaintiff;
                    }
                    else
                    {
                        if (!tmpStrPlaintiff.Contains(item2.Plaintiff))
                        {
                            tmpStrPlaintiff += "," + item2.Plaintiff;
                        }
                    }

                    if (string.IsNullOrEmpty(tmpStrDefendant))
                    {
                        tmpStrDefendant = item2.Defendant;
                    }
                    else
                    {
                        if (!tmpStrDefendant.Contains(item2.Defendant))
                        {
                            tmpStrDefendant += "," + item2.Defendant;
                        }
                    }
                }

                //地址或律师 原告
                var tmpAddr = "";
                var tmpPlainRef2 = "";
                if (tmpMinCourt.PlainRef2.IndexOf('/') > -1)
                {
                    var dd = tmpMinCourt.PlainRef2.Split('/');
                    tmpAddr = dd[0];
                    tmpPlainRef2 = dd[1];

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
                //律师 被告
                //f.	Defref3包含爬虫数据的full case no/律师名/其他，“others”放在其他1项目中
                var tmpDefref3 = "";
                var tmpfullcaseno = "";
                var tmpOther1 = "";
                if (tmpMinCourt.Defref3.IndexOf('/') > -1)
                {
                    var dd3 = tmpMinCourt.Defref3.Split('/');
                    tmpfullcaseno = dd3[0].Trim();
                    tmpDefref3 = dd3[1].Trim();
                    if (dd3.Length > 2)
                    {
                        tmpOther1 = dd3[2].Trim();
                    }
                }
                else
                {
                    if (checkCourt(tmpMinCourt.Defref3))
                    {
                        tmpDefref3 = tmpMinCourt.Defref3;
                    }
                    else
                    {
                        tmpOther1 = tmpMinCourt.Defref3;
                    }
                }
                //案件编号
                // 正式案件编号处理方法
                //  旧数据记录读取顺序：值不存在，取下个。
                //  plainref3--》Defref3--》Caseno
                //c.	案件编号不完整的部分，如果在其他列找不到full case no，在字母后加上XXXX，年份通过 ActionDate补充完整。例： H1234/2002---HXXX1234/1999
                var tmpCaseNo = string.IsNullOrEmpty(tmpMinCourt.PlainRef3) ? tmpfullcaseno : tmpMinCourt.PlainRef3.Trim();
                if (string.IsNullOrEmpty(tmpCaseNo))
                {
                    try
                    {
                        tmpCaseNo = tmpMinCourt.Caseno.Substring(0, 1) + "XXX" + tmpMinCourt.Caseno.Substring(1);
                        if (tmpMinCourt.ActionDate.HasValue)
                        {
                            tmpCaseNo += "/" + tmpMinCourt.ActionDate.Value.ToString("yyyy");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorFormat("#########案件编号出错。{0}", ex);
                    }


                }

                //赋值到正式表 
                var getItem = new m_Case_items();

                getItem.htmlID = 0;
                getItem.Language = 0;
                getItem.tIndex = 1;
                getItem.tkeyNo = tmpMinCourt.courtid.ToString();

                getItem.CaseNo = regDoubleSpace(tmpCaseNo);
                //getItem.CaseTypeID = regDoubleSpace(tmpMinCourt.CaseType);
                //getItem.Cause = item.Cause;
                getItem.ClientIP = "";
                getItem.CourtDay = tmpMinCourt.ActionDate.HasValue ? tmpMinCourt.ActionDate.Value.ToString("yyyy-MM-dd") : "";
                getItem.CourtID = "";
                getItem.Year = "";
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
                getItem.Parties = getItem.PlainTiff + " @and@ " + getItem.Defendant;

                if (checkPDtoOther(getItem.Parties))
                {
                    getItem.Other = getItem.Parties;
                }
                if (!string.IsNullOrEmpty(tmpOther1))
                {
                    if (!string.IsNullOrEmpty(getItem.Other))
                    {
                        getItem.Other += " " + tmpOther1;
                    }
                    else
                    {
                        getItem.Other = tmpOther1;
                    }
                }
                ///////////保存
                if (AddmCaseItems(getItem, o))
                {
                    //写新数据成功。
                    //更新标记
                    updateOldFlag(tmpDicCountIdToSaveFlag, o);
                }
                else
                {
                    msg = string.Format("******************更新失败。案件IDCourtid:{0}", getCourtid);
                    logger.ErrorFormat(msg);
                    SetMsg(o.f, o.cl, msg, true);
                }

                //循环，递归处理，下条。
                UpdateCourtToNewDB(t);
            }
            catch (Exception ex)
            {
                msg = string.Format("********************************当前更新出错*{0}.", ex);
                SetMsg(o.f, o.cl, msg, true);
                SetEnable(o.f, o.btn, true);
                SetEnable(o.f, o.btn2, true);
                logger.ErrorFormat("********************************当前更新出错*{0}.", ex);
                //throw ex;
                //循环，递归处理,下条。
                //UpdateCourtToNewDB(t);
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

                return v;
            }
            catch (Exception)
            {
                return v;
            }
        }
        static string ReplaceReg(string patt, string input, string totxt)
        {
            try
            {
                if (string.IsNullOrEmpty(input))
                {
                    return "";
                }
                Regex rgx = new Regex(patt, RegexOptions.IgnoreCase);
                return rgx.Replace(input.ToString(), totxt).Trim();
            }
            catch (Exception ex)
            {

                return input;
            }
        }

        /// <summary> 
        /// 律师判定
        /// ；律师可通过以下关键字区分： 
        /// & co， associates，
        /// parters， sohicitors，
        /// hotar，
        /// LLP. In person=没有律师  
        /// department of justice =政府律政师 
        /// legal department=律政处  
        /// </summary>
        /// <param name="tmpen"></param>
        /// <returns></returns>
        static bool checkCourt(string tmpen)
        {
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
            return (
                tmpen.IndexOf("defend in counterclaim", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                tmpen.IndexOf("claimment in counterclaim", StringComparison.InvariantCultureIgnoreCase) > -1
                ) ? true : false;
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
                var tmpModel = dbNew.SingleOrDefault<m_Case_items>(@"SELECT top 1 * from m_Case_items where tkeyNo=@0", courtid);
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
        /// <summary>
        /// 判断是否为标记表中的记录,旧数据库
        /// </summary>
        /// <param name="courtid"></param>
        /// <returns></returns>
        public static bool IsCourtUpdateFlag(long courtid)
        {
            try
            {
                var tmpModel = dbOld.SingleOrDefault<courtUpdateFlag>(@"SELECT top 1 * from courtUpdateFlag where courtid=@0", courtid);
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
        /// <summary>
        /// 获取案件表，记录,旧数据库
        /// </summary>
        /// <param name="courtid"></param>
        /// <returns></returns>
        public static court GetCourt(long courtid)
        {
            try
            {
                var tmpModel = dbOld.SingleOrDefault<court>(@"SELECT top 1 * from court where courtid=@0", courtid);
                return tmpModel;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
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
    }
}
