using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Opc.Ua.Server;
using Opc.Ua.Hsl;

namespace WindowsFormsAppServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            server = new OpcUaServer("opc.tcp://localhost:62547/DataAccessServer",new DataAccessServer());//117.48.203.204
            server.AppConfig.TraceConfiguration.OutputFilePath = Application.StartupPath + @"\Logs\Opc.Ua.Huibo.txt";
            StandardServer server2 = server.AppInstance.Server as StandardServer;
            dataAccess = server.AppInstance.Server as DataAccessServer;
            serverDiagnosticsCtrl1.Initialize(server2, server.AppConfig);
        }




        public OpcUaServer server { get; set; }
        private DataAccessServer dataAccess { get; set; }

        private void writeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //server.WriteNode("ns=2;s=1:Device B?Name", Guid.NewGuid().ToString("N"));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            dataAccess.CurrentNodeManager.SetStatusChange(checkBox1.Checked);
        }
    }
}
