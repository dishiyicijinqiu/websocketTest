using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using WebSocket4Net;

namespace WClient
{
    public partial class Form1 : Form
    {
        AutoResetEvent OpenedEvent = new AutoResetEvent(false);
        WebSocket4Net.WebSocket webSocket =
       new WebSocket(string.Format("ws://127.0.0.1:{0}/websocket", 2012),
           "basic", WebSocketVersion.None);
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            webSocket.Closed -= webSocket_Closed;
            webSocket.Closed += webSocket_Closed;
            webSocket.Opened -= webSocket_Opened;
            webSocket.Opened += webSocket_Opened;
            webSocket.DataReceived -= webSocket_DataReceived;
            webSocket.DataReceived += webSocket_DataReceived;
        }
        void webSocket_Closed(object sender, EventArgs e)
        {
        }
        void webSocket_Opened(object sender, EventArgs e)
        {
            OpenedEvent.Set();
        }
        void webSocket_DataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                var data = FormatterHelper.BytesToObject(e.Data) as object[];
                if (data == null)
                    return;
                MessageBox.Show("Test");
            }
            catch (Exception ex)
            {
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                MessageBox.Show("连接成功");
                return;
            }
            if (webSocket.State != WebSocketState.Open)
            {
                webSocket.Open();
                if (OpenedEvent.WaitOne(2000))
                {
                    if (webSocket.State == WebSocketState.Open)
                    {
                        MessageBox.Show("连接成功");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("连接失败");
                        return;
                    }
                }
            }
            MessageBox.Show("连接成功");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var data = FormatterHelper.ObjectToBytes(new object[] { "Login", "how are you " });
            webSocket.Send(data, 0, data.Length);
        }
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
