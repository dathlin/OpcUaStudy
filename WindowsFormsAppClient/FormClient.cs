using Opc.Ua.Client.Controls;
using Opc.Ua.Hsl;
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

            Icon = ClientUtils.GetAppIcon();

            client = new OpcUaClient(false);
        }


        private OpcUaClient client { get; set; }

        private void FormClient_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.ServerUrl = "opc.tcp://localhost:14711/MyServer";
            client.OpcStatusChange += Client_OpcStatusChange;
            client.SetLogOutPut();



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
            if (e.IsError)
            {
                sb.Append("[错误] ");
            }
            else
            {
                sb.Append("[正常] ");
            }

            sb.Append(e.OccurTime.ToString("HH:mm:ss"));
            sb.Append(" ");
            sb.Append(e.Status);
            sb.Append(Environment.NewLine);

            textBox1.AppendText(sb.ToString());
        }



        private void button2_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            string value = client.ReadNode<string>("ns=2;s=Devices/Device B/Name");

            TimeSpan ts = DateTime.Now - dt;
            textBox2.AppendText("值：" + value + "   耗时：" + ts.TotalMilliseconds + "毫秒" + Environment.NewLine);
        }

        private void FormClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Disconnect();
        }
    }
}
