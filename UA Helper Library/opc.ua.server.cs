using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opc.Ua.Hsl
{
   public class OpcUaServer
    {
        #region 构造方法

        public OpcUaServer()
        {
            application.ApplicationType = ApplicationType.Server;
            application.ConfigSectionName = "OpcUaHslServer";

        }


        #endregion


        private ApplicationInstance application = new ApplicationInstance();


    }
}
