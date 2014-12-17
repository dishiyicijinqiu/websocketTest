using SuperSocket.SocketBase.Config;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace WServer
{
    public partial class Form1 : Form
    {
        private WebSocketServer m_WebSocketServer;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_WebSocketServer = new WebSocketServer();
            m_WebSocketServer.NewDataReceived += m_WebSocketServer_NewDataReceived;
            var rootConfig = new RootConfig { DisablePerformanceDataCollector = true };
            var config = new ServerConfig();
            config.Port = 2012;
            config.Ip = "Any";
            config.MaxConnectionNumber = 100;
            config.MaxRequestLength = 100000;
            config.Name = "SuperWebSocket Server";
            var ret = m_WebSocketServer.Setup(rootConfig, config, null, null,
                null, null, null);
        }
        void m_WebSocketServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            try
            {
                var data = FormatterHelper.BytesToObject(value) as object[];
                if (data == null)
                    return;
                if (data[0].ToString() == "Login")
                {
                    var ResponseData = FormatterHelper.ObjectToBytes
                        (new object[] { "LoginResult", "Sucess" });
                    session.Send(ResponseData, 0, ResponseData.Length);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (m_WebSocketServer.Start())
            {
                MessageBox.Show("启动成功");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_WebSocketServer.Stop();
        }

        public class FormatterHelper
        {
            /// <summary>
            /// 将一个object对象序列化，返回一个byte[]
            /// </summary>
            /// <param name="obj">能序列化的对象</param>
            /// <returns></returns>
            public static byte[] ObjectToBytes(object obj)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ms, obj);
                    return ms.GetBuffer();
                }
            }
            /// <summary>
            /// 将一个序列化后的byte[]数组还原      
            /// </summary>
            /// <param name="Bytes"></param>
            /// <returns></returns>
            public static object BytesToObject(byte[] Bytes)
            {
                using (MemoryStream ms = new MemoryStream(Bytes))
                {
                    IFormatter formatter = new BinaryFormatter(); return formatter.Deserialize(ms);
                }
            }
        }
    }
}
