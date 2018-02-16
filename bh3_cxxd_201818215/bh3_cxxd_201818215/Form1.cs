using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace bh3_cxxd_201818215
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class RootObject
        {
            public int retcode { get; set; }
            public string msg { get; set; }
        }

        public Byte[] data; //数据包
        public int successCount; //成功次数

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Interval = Convert.ToInt32(textBox5.Text);
            this.successCount = 0;  //重置成功次数
            var postData = "auth_key=" + textBox2.Text;
            postData += "&sign=" + textBox1.Text;
            postData += "&action=1";
            postData += "&type="+comboBox1.Text;
            postData += "&price="+comboBox3.Text;
            postData += "&quantity=1";
            this.data = Encoding.ASCII.GetBytes(postData);

            timer1.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            textBox3.Text += "\r\n" + DateTime.Now.ToString() + " 已手动停止";
            textBox3.Focus();//获取焦点
            textBox3.Select(this.textBox3.TextLength, 0);//光标定位到文本最后
            textBox3.ScrollToCaret();//滚动到光标处
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //访问
            var request = (HttpWebRequest)WebRequest.Create("https://event.bh3.com/bh3_2018spring_festival/trade.php");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            RootObject rb = JsonConvert.DeserializeObject<RootObject>(responseString);
            if(rb.retcode==0){
                this.successCount += 1;  //成功次数+1
                if (this.successCount == Convert.ToInt32(textBox4.Text))
                {
                    //完成指定数量任务 --》结束
                    timer1.Enabled = false;
                    outline("指定任务已经完成");
                }
                else {
                    this.Text += " -已完成"+this.successCount+"/"+textBox4.Text;
                }
            }else{
                outline(DateTime.Now.ToString() + " Code:" + rb.retcode + " Msg:" + rb.msg);
            }
        }

        public void outline(string str) {
            textBox3.Text += "\r\n " + str;
            textBox3.Focus();//获取焦点
            textBox3.Select(this.textBox3.TextLength, 0);//光标定位到文本最后
            textBox3.ScrollToCaret();//滚动到光标处
        }


    }
}
