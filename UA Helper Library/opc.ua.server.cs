using Opc.Ua.Configuration;
using Opc.Ua.Server;
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

            // load the application configuration.
            application.LoadApplicationConfiguration(false);
        }


        private void Initialization()
        {


            // check the application certificate.
            application.CheckApplicationInstanceCertificate(false, 0);

            // start the server.
            application.Start(new DataAccessServer());
        }


        #endregion


        private ApplicationInstance application = new ApplicationInstance();


    }

    /// <summary>
    /// Implements a basic UA Data Access Server.
    /// 实现一个基础的OPC UA数据访问服务器
    /// </summary>
    /// <remarks>
    /// Each server instance must have one instance of a StandardServer object which is
    /// responsible for reading the configuration file, creating the endpoints and dispatching
    /// incoming requests to the appropriate handler.
    /// 
    /// This sub-class specifies non-configurable metadata such as Product Name and initializes
    /// the DataAccessServerNodeManager which provides access to the data exposed by the Server.
    /// 
    /// 每个服务器实例必须有一个StandardServer对象的实例，该实例负责读取配置文件，创建端点并将传入的请求分派到相应的处理程序。
    ///
    /// 此子类指定不可配置的元数据（如产品名称），并初始化DataAccessServerNodeManager，该对象提供对服务器公开的数据的访问。
    /// 
    /// </remarks>
    public partial class DataAccessServer : StandardServer
    {
        #region Public Interface
        /// <summary>
        /// Returns the current server instance.
        /// </summary>
        public IServerInternal ServerInstance
        {
            get { return ServerInternal; }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Creates the node managers for the server.
        /// </summary>
        /// <remarks>
        /// This method allows the sub-class create any additional node managers which it uses. The SDK
        /// always creates a CoreNodeManager which handles the built-in nodes defined by the specification.
        /// Any additional NodeManagers are expected to handle application specific nodes.
        /// </remarks>
        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            Utils.Trace("Creating the Node Managers.");

            List<INodeManager> nodeManagers = new List<INodeManager>();

            // create the custom node managers.
            nodeManagers.Add(new DataAccessServerNodeManager(server, configuration));

            // create master node manager.
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

        /// <summary>
        /// Loads the non-configurable properties for the application.
        /// </summary>
        /// <remarks>
        /// These properties are exposed by the server but cannot be changed by administrators.
        /// </remarks>
        protected override ServerProperties LoadServerProperties()
        {
            ServerProperties properties = new ServerProperties();

            properties.ManufacturerName = "OPC Foundation";
            properties.ProductName = "Quickstart DataAccess Server";
            properties.ProductUri = "http://opcfoundation.org/Quickstart/DataAccessServer/v1.0";
            properties.SoftwareVersion = Utils.GetAssemblySoftwareVersion();
            properties.BuildNumber = Utils.GetAssemblyBuildNumber();
            properties.BuildDate = Utils.GetAssemblyTimestamp();

            // TBD - All applications have software certificates that need to added to the properties.

            return properties;
        }
        #endregion
    }
}
