using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMMS.UpdateOldDBToNew.modelOld
{
    /// <summary>
    /// court（案件表） court/court_copy/court_temp
    /// </summary>
    [Serializable]
    [PetaPoco.TableName("court")]
    [PetaPoco.PrimaryKey("courtid")]
    public class court
    {
        /// <summary>
        /// 法院ID
        /// </summary>
        public virtual long courtid { get; set; }


        private string _Caseno = "";
        /// <summary>
        /// 案件编号
        /// H1234/2002
        /// 旧的案件编号，不完整，当手动（PlainRef3）和爬虫（Defref3）都没有时，再处理(HXXX123/1999)
        /// 
        /// </summary>
        public virtual string Caseno
        {
            get
            {
                return _Caseno;
            }
            set
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        _Caseno = value.ToUpper().Trim();
                    }
                    else
                    {
                        _Caseno = "";
                    }
                }
                catch (Exception)
                {
                    _Caseno = value;
                }

            }
        }
        /// <summary>
        /// 原告
        /// </summary>
        public virtual string Plaintiff { get; set; }
        /// <summary>
        /// 原告中文名
        /// </summary>
        public virtual string Cplaintiff { get; set; }
        /// <summary>
        /// 原告陪员1-->其他原告
        /// </summary>
        public virtual string plainRef1 { get; set; }
        /// <summary>
        /// 原告陪员2-->只有一个时放地址中，/前为地址,/后为律师,
        /// 用一些关键字区分,,,,
        /// 无/符，看关键字。。。放地址或律师
        /// </summary>
        public virtual string PlainRef2 { get; set; }
        /// <summary>
        /// 原告陪员3--》手动打的案件编号，当正式库的caseno。
        /// </summary>
        public virtual string PlainRef3 { get; set; }
        /// <summary>
        /// 被告
        /// </summary>
        public virtual string Defendant { get; set; }
        /// <summary>
        /// 被告中文名
        /// </summary>
        public virtual string Cdefendant { get; set; }
        /// <summary>
        /// 被告陪员1--》其他被告
        /// </summary>
        public virtual string Defref1 { get; set; }
        /// <summary>
        /// 被告陪员2--》多个地址
        /// </summary>
        public virtual string Defref2 { get; set; }
        /// <summary>
        /// 被告陪员3--》爬虫爬下来的案件编号/律师名字/其它
        /// </summary>
        public virtual string Defref3 { get; set; }
        /// <summary>
        /// 原由
        /// </summary>
        public virtual string Cause { get; set; }
        /// <summary>
        /// 币别
        /// </summary>
        public virtual string amountcurrency { get; set; }
        /// <summary>
        /// 总计
        /// </summary>
        public virtual string amount { get; set; }
        /// <summary>
        /// 案件日期
        /// </summary>
        private DateTime _ActionDate;
        public virtual DateTime ActionDate
        {
            get
            {
                return _ActionDate;
            }
            set
            {
                try
                {
                    _ActionDate = value.Date;
                }
                catch (Exception)
                {
                    _ActionDate = value;
                }
            }
        }
        /// <summary>
        /// 创建日期
        /// </summary>
        public virtual string createdate { get; set; }

    }
}
