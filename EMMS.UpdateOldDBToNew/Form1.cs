using Common.Logging;
using EMMS.UpdateOldDBToNew.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMMS.UpdateOldDBToNew
{
    public partial class Form1 : Form
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string tittle = "EMMS系统旧数据迁移";
        public Form1()
        {
            InitializeComponent();

            this.Resize += Form1_Resize;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Tag = tittle;
            this.Text = tittle;
            this.AcceptButton = button2;
            this.button2.Focus();
           
            
        }

        void Form1_Resize(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            initWidth();
        }
        void initWidth()
        {
            richTextBox1.Width = this.Width - richTextBox1.Left * 3;
            richTextBox1.Height = this.Height - richTextBox1.Top - 50;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.Text = tittle + "-->正在 " + button1.Text;
                objT t = new objT() { f = this, cl = richTextBox1, btn = button1, btn2 = button2, btn3 = button3 };
                ThreadPool.QueueUserWorkItem(new WaitCallback(DBlib.UpdateCustOldToNew), t);
                //DBlib.UpdateCustOldToNew(o);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                this.Text = tittle + "-->正在 " + button2.Text;
                objT t = new objT() { f = this, cl = richTextBox1, btn = button2, btn2 = button1, btn3 = button3 };
                ThreadPool.QueueUserWorkItem(new WaitCallback(DBlib.UpdateCourtToNewDB), t);
                //DBlib.UpdateCourtToNewDB(o);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                this.Text = tittle + "-->正在 " + button3.Text;
                objT t = new objT() { f = this, cl = richTextBox1, btn = button3, btn3 = button2, btn2 = button1 };
                ThreadPool.QueueUserWorkItem(new WaitCallback(DBlib.UpdatePanamaDBToNewDB), t);
                //DBlib.UpdateCourtToNewDB(o);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
    }
}
