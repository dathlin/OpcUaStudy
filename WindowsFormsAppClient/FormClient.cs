﻿using Newtonsoft.Json.Linq;
using Opc.Ua.Client.Controls;
using Opc.Ua.Hsl;
using Opc.Ua;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppClient
{
    public partial class FormClient : Form
    {
        public FormClient()
        {
            InitializeComponent();
            
            // use a default appConfig object
            // 使用了一个默认的配置对象
            client = new OpcUaClient(false);
        }


        private OpcUaClient client { get; set; }

        private void FormClient_Load(object sender, EventArgs e)
        {
            textBox3.Text = "opc.tcp://localhost:14711/MyServer";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.ServerUrl = textBox3.Text;
            client.OpcStatusChange += Client_OpcStatusChange;
            client.SetLogOutPut();

            // if server need a username and password
            //client.UserIdentity = new UserIdentity("admin", "123456")
;
            client.Connect();

            button1.BackColor = Color.LimeGreen;
        }
        private void Client_OpcStatusChange(object sender, OpcStausEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<OpcStausEventArgs>(Client_OpcStatusChange), sender, e);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(e.IsError ? "[错误] " : "[正常] ");//[failed] and [success]
            sb.Append(e.OccurTime.ToString("HH:mm:ss"));
            sb.Append(" ");
            sb.Append(e.Status);
            sb.Append(Environment.NewLine);

            textBox1.AppendText(sb.ToString());
        }



        private void button2_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            //string value = client.ReadNode<string>("ns=2;s=Devices/Device B/Name");
            string value = client.ReadNode<string>("ns=2;s=1:Device B?Name");
            TimeSpan ts = DateTime.Now - dt;
            textBox2.AppendText("value: " + value + "   time: " + ts.TotalMilliseconds + "ms" + Environment.NewLine);
        }

        private void FormClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Disconnect();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            //bool result=client.WriteNode("s=Devices/Device B/Name",Guid.NewGuid().ToString("N"));
            bool result = client.WriteNode("ns=2;s=1:Device B?Name", Guid.NewGuid().ToString("N"));
            TimeSpan ts = DateTime.Now - dt;
            textBox2.AppendText("value: " + result.ToString() + "   time: " + ts.TotalMilliseconds + "ms" + Environment.NewLine);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            client.MonitorValue<string>("s=Devices/Device B/Name", (m, unsubscribe) =>
             {
                 textBox2.BeginInvoke(new Action(() => {
                     textBox2.AppendText("value: " + m + Environment.NewLine);
                 }));
             });

            button4.BackColor = Color.LimeGreen;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string url = new DiscoverServerDlg().ShowDialog(client.AppConfiguration, null);
            if(!string.IsNullOrEmpty(url))
            {
                textBox3.Text = url;
            }
        }
    }
}
