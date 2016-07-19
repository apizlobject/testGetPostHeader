using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace testGetPostHeader
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 接收header
        /// </summary>
        public static string rxHeader = "";
        /// <summary>
        /// 发送header
        /// </summary>
        public static string txHeader = "";

        /// <summary>
        /// cookies
        /// </summary>
        public static string cookies = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            string type = comboBoxType.Text;
            string url = textBoxUrl.Text;
            string data = textBoxData.Text;
            if (type == "GET")
            {
                textBoxContent.Text = HttpGetConnectToServer(url, data);
            }
            if (type == "POST")
            {
                HttpPostConnectToServer(url, data);
            }
            textBoxRx.Text = rxHeader;
            textBoxTx.Text = txHeader;
            textBoxCookies.Text = cookies;
        }
        /// <summary>
        /// post提交
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private static string HttpPostConnectToServer(string serverUrl, string postData)
        {
            var dataArray = Encoding.UTF8.GetBytes(postData);
            //创建请求  
            var request = (HttpWebRequest)HttpWebRequest.Create(serverUrl);
            request.Method = "POST";
            request.ContentLength = dataArray.Length;
            request.Headers["cookie"] = cookies;//设置cookies
            //设置上传服务的数据格式  
            request.ContentType = "application/x-www-form-urlencoded";
            //请求的身份验证信息为默认  
            request.Credentials = CredentialCache.DefaultCredentials;
            //请求超时时间  
            request.Timeout = 10000;
            //创建输入流  
            Stream dataStream;
            HeaderString(request.Headers, 1);
            //using (var dataStream = request.GetRequestStream())  
            //{  
            //    dataStream.Write(dataArray, 0, dataArray.Length);  
            //    dataStream.Close();  
            //}  
            try
            {
                dataStream = request.GetRequestStream();
            }
            catch (Exception)
            {
                return null;//连接服务器失败  
            }
            //发送请求  
            dataStream.Write(dataArray, 0, dataArray.Length);
            dataStream.Close();
            //读取返回消息  
            string res;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                HeaderString(response.Headers, 2);
                var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                //var result = new ServerResult();
                return "{\"error\":\"connectToServer\",\"error_description\":\"" + ex.Message + "\"}";//连接服务器失败  
            }
            return res;
        }

        /// <summary>
        /// get提交
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private static string HttpGetConnectToServer(string serverUrl, string postData)
        {
            //创建请求  
            var request = (HttpWebRequest)HttpWebRequest.Create(serverUrl + "?" + postData);
            request.Method = "GET";
            request.Headers["cookie"] = cookies;//设置cookies
            //设置上传服务的数据格式  
            request.ContentType = "application/x-www-form-urlencoded";
            //请求的身份验证信息为默认  
            request.Credentials = CredentialCache.DefaultCredentials;
            //请求超时时间  
            request.Timeout = 10000;

            HeaderString(request.Headers, 1);
            //读取返回消息  
            string res;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                HeaderString(response.Headers, 2);
                CookieCollection cookie = response.Cookies;
                var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                //textBoxContent.Text =;
                res = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return res;
        }

        public static string HeaderString(WebHeaderCollection header, int type = 1)
        {
            string str = "";
            foreach (string v in header)
            {
                if (v == "Set-Cookie")
                {
                    cookies = header[v];
                }
                if (str != "")
                {
                    str = str + "\r\n" + v + ":" + header[v];
                }
                else
                {
                    str = v + ":" + header[v];
                }
            }
            if (type == 1)
            {
                txHeader = str;
            }
            if (type == 2)
            {
                rxHeader = str;
            }
            return str;
        }
    }
}
