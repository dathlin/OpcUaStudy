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
            server = new OpcUaServer("opc.tcp://localhost:13567/OpcUaServer");
            StandardServer server2 = server.AppInstance.Server as StandardServer;
            serverDiagnosticsCtrl1.Initialize(server2, server.AppConfig);
        }




        public OpcUaServer server { get; set; }
        
    }
}
