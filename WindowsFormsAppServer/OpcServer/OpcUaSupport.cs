using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsAppServer
{
    /// <summary>
    /// Implements a basic UA Data Access Server.
    /// </summary>
    /// <remarks>
    /// Each server instance must have one instance of a StandardServer object which is
    /// responsible for reading the configuration file, creating the endpoints and dispatching
    /// incoming requests to the appropriate handler.
    /// 
    /// This sub-class specifies non-configurable metadata such as Product Name and initializes
    /// the DataAccessServerNodeManager which provides access to the data exposed by the Server.
    /// </remarks>
    public partial class DataAccessServer : StandardServer
    {
        #region Constructors
        
        public DataAccessServer()
        {
            // 添加允许登录的用户名
            DictionaryIdentity.Add("admin", "123456");
            DictionaryIdentity.Add("hushaolin", "1234567890");
        }
        
        #endregion

        #region Public Interface
        /// <summary>
        /// Returns the current server instance.
        /// </summary>
        public IServerInternal ServerInstance
        {
            get { return this.ServerInternal; }
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

            CurrentNodeManager = new OpcNodeManager(server, configuration);
            // create the custom node managers.
            nodeManagers.Add(CurrentNodeManager);

            // create master node manager.
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

        /// <summary>
        /// Loads the non-configurable properties for the application.
        /// 加载一些不需要配置的属性
        /// </summary>
        /// <remarks>
        /// These properties are exposed by the server but cannot be changed by administrators.
        /// </remarks>
        protected override ServerProperties LoadServerProperties()
        {
            ServerProperties properties = new ServerProperties();

            properties.ManufacturerName = "江苏汇博机器人技术股份有限公司";
            properties.ProductName = "Opc Ua 服务器";
            properties.ProductUri = "http://www.huiborobot.com/index.html";
            properties.SoftwareVersion = Utils.GetAssemblySoftwareVersion();
            properties.BuildNumber = Utils.GetAssemblyBuildNumber();
            properties.BuildDate = Utils.GetAssemblyTimestamp();

            // TBD - All applications have software certificates that need to added to the properties.

            return properties;
        }



        /// <summary>
        /// 服务器启动之后将会调用的方法
        /// </summary>
        /// <param name="server"></param>
        protected override void OnServerStarted(IServerInternal server)
        {
            base.OnServerStarted(server);

            // request notifications when the user identity is changed. all valid users are accepted by default.
            // 更改用户身份时请求通知。 默认情况下接受所有有效用户
            server.SessionManager.ImpersonateUser += new ImpersonateEventHandler(SessionManager_ImpersonateUser);


        }
        


        /// <summary>
        /// Called when a client tries to change its user identity.
        /// 当客户端尝试更改其用户身份时调用
        /// </summary>
        private void SessionManager_ImpersonateUser(Session session, ImpersonateEventArgs args)
        {
            // check for a user name token.
            // 检查用户名令牌
            UserNameIdentityToken userNameToken = args.NewIdentity as UserNameIdentityToken;

            if (userNameToken != null)
            {
                VerifyPassword(userNameToken.UserName, userNameToken.DecryptedPassword);
                args.Identity = new UserIdentity(userNameToken);
                return;
            }
        }

        /// <summary>
        /// Validates the password for a username token.
        /// </summary>
        private void VerifyPassword(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
            {
                // an empty username is not accepted.
                throw ServiceResultException.Create(StatusCodes.BadIdentityTokenInvalid,
                    "Security token is not a valid username token. An empty username is not accepted.");
            }

            if (String.IsNullOrEmpty(password))
            {
                // 空的密码不被允许
                throw ServiceResultException.Create(StatusCodes.BadIdentityTokenRejected,
                    "Security token is not a valid username token. An empty password is not accepted.");
            }


            if (!DictionaryIdentity.ContainsKey(userName))
            {
                // 账户名验证失败
                throw ServiceResultException.Create(StatusCodes.BadUserAccessDenied,
                    "您输入的账户不存在，禁止登录");
            }


            if(DictionaryIdentity[userName]!=password)
            {
                // 密码验证失败
                throw ServiceResultException.Create(StatusCodes.BadUserAccessDenied,
                    "您输入的账户不存在，禁止登录");
            }
            
        }

        private Dictionary<string, string> DictionaryIdentity = new Dictionary<string, string>();





        #endregion


        #region 公开的数据


        /// <summary>
        /// 公开的节点数据
        /// </summary>
        public OpcNodeManager CurrentNodeManager { get; set; }
        
        #endregion
    }




    public class OpcNodeManager : CustomNodeManager2
    {

        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public OpcNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        :
            base(server, configuration, Namespaces.Huibo)
        {
            SystemContext.NodeIdFactory = this;

            timer1 = new System.Timers.Timer(500);
            timer1.Elapsed += Timer1_Elapsed;
            timer1.Start();
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TBD
            }
        }
        #endregion

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            return node.NodeId;
        }
        #endregion


        #region Timer Tick

        private System.Timers.Timer timer1 = null;



        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (list != null)
            {
                lock (Lock)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].Value = list[i].Value + 1;
                        // 下面这行代码非常的关键，涉及到更改之后会不会通知到客户端
                        list[i].ClearChangeMasks(SystemContext, false);
                    }
                }
            }
        }

        #endregion


        #region INodeManager Members
        /// <summary>
        /// Does any initialization required before the address space can be used.
        /// </summary>
        /// <remarks>
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.  
        /// </remarks>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                LoadPredefinedNodes(SystemContext, externalReferences);

                IList<IReference> references = null;

                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }

                FolderState rootMy = CreateFolder(null, "Machines");
                rootMy.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, rootMy.NodeId));
                rootMy.EventNotifier = EventNotifiers.SubscribeToEvents;
                AddRootNotifier(rootMy);

                string[] machines = new string[] { "Machine A", "Machine B", "Machine C" };
                list = new List<BaseDataVariableState<int>>();

                for (int i = 0; i < machines.Length; i++)
                {
                    FolderState myFolder = CreateFolder(rootMy, machines[i]);

                    CreateVariable(myFolder, "Name", DataTypeIds.String, ValueRanks.Scalar, "测试数据").Description = "设备的名称";
                    CreateVariable(myFolder, "IsFault", DataTypeIds.Boolean, ValueRanks.Scalar, true).Description = "设备是否启动";
                    CreateVariable(myFolder, "TestValueFloat", DataTypeIds.Float, ValueRanks.Scalar, 100.5f);
                    CreateVariable(myFolder, "AlarmTime", DataTypeIds.DateTime, ValueRanks.Scalar, DateTime.Now);
                    list.Add(CreateVariable(myFolder, "TestValueInt", DataTypeIds.Int32, ValueRanks.Scalar, 47123));




                    #region Add Method

                    MethodState addMethod = CreateMethod(myFolder, "Calculate");
                    // set input arguments
                    addMethod.InputArguments = new PropertyState<Argument[]>(addMethod)
                    {
                        NodeId = new NodeId(addMethod.BrowseName.Name + "InArgs", NamespaceIndex),
                        BrowseName = BrowseNames.InputArguments
                    };
                    addMethod.InputArguments.DisplayName = addMethod.InputArguments.BrowseName.Name;
                    addMethod.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    addMethod.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    addMethod.InputArguments.DataType = DataTypeIds.Argument;
                    addMethod.InputArguments.ValueRank = ValueRanks.OneDimension;

                    addMethod.InputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "Int32 value", Description = "Int32 value",  DataType = DataTypeIds.Int32, ValueRank = ValueRanks.Scalar },
                        new Argument() { Name = "Int32 value", Description = "Int32 value",  DataType = DataTypeIds.Int32, ValueRank = ValueRanks.Scalar }
                    };

                    // set output arguments
                    addMethod.OutputArguments = new PropertyState<Argument[]>(addMethod);
                    addMethod.OutputArguments.NodeId = new NodeId(addMethod.BrowseName.Name + "OutArgs", NamespaceIndex);
                    addMethod.OutputArguments.BrowseName = BrowseNames.OutputArguments;
                    addMethod.OutputArguments.DisplayName = addMethod.OutputArguments.BrowseName.Name;
                    addMethod.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                    addMethod.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                    addMethod.OutputArguments.DataType = DataTypeIds.Argument;
                    addMethod.OutputArguments.ValueRank = ValueRanks.OneDimension;

                    addMethod.OutputArguments.Value = new Argument[]
                    {
                        new Argument() { Name = "Operater Result", Description = "Operater Result",  DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar }
                    };
                    addMethod.OnCallMethod = new GenericMethodCalledEventHandler(OnAddCall);

                    #endregion

                }





                AddPredefinedNode(SystemContext, rootMy);
            }
        }


        private List<BaseDataVariableState<int>> list = null;



        /// <summary>
        /// 更改前触发，可以禁止更改掉，返回 Bad 即可
        /// </summary>
        /// <param name="context"></param>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private ServiceResult OnWriteMyNode(ISystemContext context, NodeState node, ref object value)
        {
            // System.Windows.Forms.MessageBox.Show("Received '" + value.ToString() + "'.");
            return ServiceResult.Good;
        }

        /// <summary>
        /// Creates a new folder.
        /// </summary>
        private FolderState CreateFolder(NodeState parent, string name)
        {
            FolderState folder = new FolderState(parent);

            folder.SymbolicName = name;
            folder.ReferenceTypeId = ReferenceTypes.Organizes;
            folder.TypeDefinitionId = ObjectTypeIds.FolderType;
            if (parent == null)
            {
                folder.NodeId = new NodeId(name, NamespaceIndex);
            }
            else
            {
                folder.NodeId = new NodeId(parent.NodeId.ToString() + "/" + name);
            }
            folder.BrowseName = new QualifiedName(name, NamespaceIndex);
            folder.DisplayName = new LocalizedText(name);
            folder.WriteMask = AttributeWriteMask.None;
            folder.UserWriteMask = AttributeWriteMask.None;
            folder.EventNotifier = EventNotifiers.None;

            if (parent != null)
            {
                parent.AddChild(folder);
            }

            return folder;
        }

        /// <summary>
        /// Creates a new readonly variable.
        /// </summary>
        private BaseDataVariableState<T> CreateReadonlyVariable<T>(NodeState parent, string name, NodeId dataType, int valueRank, T defaultValue)
        {
            BaseDataVariableState<T> variableState = CreateVariable(parent, name, dataType, valueRank, defaultValue);
            variableState.AccessLevel = AccessLevels.CurrentRead;
            return variableState;
        }

        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState<T> CreateVariable<T>(NodeState parent, string name, NodeId dataType, int valueRank, T defaultValue)
        {
            BaseDataVariableState<T> variable = new BaseDataVariableState<T>(parent);

            variable.SymbolicName = name;
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            if (parent == null)
            {
                variable.NodeId = new NodeId(name, NamespaceIndex);
            }
            else
            {
                variable.NodeId = new NodeId(parent.NodeId.ToString() + "/" + name);
            }
            variable.BrowseName = new QualifiedName(name, NamespaceIndex);
            variable.DisplayName = new LocalizedText(name);
            variable.WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            variable.UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            variable.DataType = dataType;
            variable.ValueRank = valueRank;
            variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Historizing = false;
            variable.Value = defaultValue;
            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.Now;

            if (parent != null)
            {
                parent.AddChild(variable);
            }

            return variable;
        }





        /// <summary>
        /// Creates a new method.
        /// </summary>
        private MethodState CreateMethod(NodeState parent, string name)
        {
            MethodState method = new MethodState(parent);

            method.SymbolicName = name;
            method.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            if (parent == null)
            {
                method.NodeId = new NodeId(name, NamespaceIndex);
            }
            else
            {
                method.NodeId = new NodeId(parent.NodeId.ToString() + "/" + name);
            }
            method.BrowseName = new QualifiedName(name, NamespaceIndex);
            method.DisplayName = new LocalizedText(name);
            method.WriteMask = AttributeWriteMask.None;
            method.UserWriteMask = AttributeWriteMask.None;
            method.Executable = true;
            method.UserExecutable = true;

            if (parent != null)
            {
                parent.AddChild(method);
            }

            return method;
        }




        /// <summary>
        /// Frees any resources allocated for the address space.
        /// </summary>
        public override void DeleteAddressSpace()
        {
            lock (Lock)
            {
                // TBD
            }
        }

        ///// <summary>
        ///// Returns a unique handle for the node.
        ///// </summary>
        //protected override NodeHandle GetManagerHandle(ServerSystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        //{
        //    lock (Lock)
        //    {
        //        // quickly exclude nodes that are not in the namespace. 
        //        if (!IsNodeIdInNamespace(nodeId))
        //        {
        //            return null;
        //        }

        //        NodeState node = null;

        //        if (!PredefinedNodes.TryGetValue(nodeId, out node))
        //        {
        //            return null;
        //        }

        //        NodeHandle handle = new NodeHandle();

        //        handle.NodeId = nodeId;
        //        handle.Node = node;
        //        handle.Validated = true;

        //        return handle;
        //    }
        //}

        ///// <summary>
        ///// Verifies that the specified node exists.
        ///// </summary>
        //protected override NodeState ValidateNode(
        //    ServerSystemContext context,
        //    NodeHandle handle,
        //    IDictionary<NodeId, NodeState> cache)
        //{
        //    // not valid if no root.
        //    if (handle == null)
        //    {
        //        return null;
        //    }

        //    // check if previously validated.
        //    if (handle.Validated)
        //    {
        //        return handle.Node;
        //    }

        //    // TBD

        //    return null;
        //}
        #endregion

        #region Overridden Methods

        /// <summary>
        /// Returns a unique handle for the node.
        /// </summary>
        protected override NodeHandle GetManagerHandle(ServerSystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        {
            lock (Lock)
            {
                // quickly exclude nodes that are not in the namespace. 
                if (!IsNodeIdInNamespace(nodeId))
                {
                    return null;
                }

                NodeState node = null;

                if (!PredefinedNodes.TryGetValue(nodeId, out node))
                {
                    return null;
                }

                NodeHandle handle = new NodeHandle();

                handle.NodeId = nodeId;
                handle.Node = node;
                handle.Validated = true;

                return handle;
            }
        }

        /// <summary>
        /// Verifies that the specified node exists.
        /// </summary>
        protected override NodeState ValidateNode(
            ServerSystemContext context,
            NodeHandle handle,
            IDictionary<NodeId, NodeState> cache)
        {
            // not valid if no root.
            if (handle == null)
            {
                return null;
            }

            // check if previously validated.
            if (handle.Validated)
            {
                return handle.Node;
            }

            // TBD

            return null;
        }


        #endregion

        #region Private Fields


        #endregion



        private ServiceResult OnAddCall(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {

            // all arguments must be provided.
            if (inputArguments.Count < 2)
            {
                return StatusCodes.BadArgumentsMissing;
            }

            try
            {
                Int32 floatValue = (Int32)inputArguments[0];
                Int32 uintValue = (Int32)inputArguments[1];

                // set output parameter
                outputArguments[0] = "我也不知道刚刚发生了什么，调用设备为：" + method.Parent.DisplayName;
                return ServiceResult.Good;
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return new ServiceResult(StatusCodes.BadInvalidArgument);
            }
        }

    }


    /// <summary>
    /// Defines constants for namespaces used by the application.
    /// </summary>
    public static partial class Namespaces
    {
        /// <summary>
        /// The namespace for the nodes provided by the server.
        /// </summary>
        public const string Huibo = "http://www.huiborobot.com/index.html";
    }
}
