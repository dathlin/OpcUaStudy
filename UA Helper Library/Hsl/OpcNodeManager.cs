using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Server;
using System.Runtime.Serialization;

namespace Opc.Ua.Hsl
{
    /// <summary>
    /// Defines constants for namespaces used by the application.
    /// </summary>
    public static partial class Namespaces
    {
        /// <summary>
        /// The namespace for the nodes provided by the server.
        /// </summary>
        public const string Empty = "http://opcfoundation.org/HslOpc";
    }

    /// <summary>
    /// Stores the configuration the data access node manager.
    /// </summary>
    [DataContract(Namespace = Namespaces.Empty)]
    public class CustomerServerConfiguration
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public CustomerServerConfiguration()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the object during deserialization.
        /// </summary>
        [OnDeserializing()]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }

        /// <summary>
        /// Sets private members to default values.
        /// </summary>
        private void Initialize()
        {
        }
        #endregion

        #region Public Properties
        #endregion

        #region Private Members
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
            base(server, configuration, Namespaces.Empty)
        {
            SystemContext.NodeIdFactory = this;

            // get the configuration for the node manager.
            m_configuration = configuration.ParseExtension<CustomerServerConfiguration>();

            // use suitable defaults if no configuration exists.
            if (m_configuration == null)
            {
                m_configuration = new CustomerServerConfiguration();
            }
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
                BaseObjectState Machines = new BaseObjectState(null);

                Machines.NodeId = new NodeId(1, NamespaceIndex);
                Machines.BrowseName = new QualifiedName("Machine", NamespaceIndex);
                Machines.DisplayName = Machines.BrowseName.Name;
                Machines.TypeDefinitionId = ObjectTypeIds.BaseObjectType;

                // ensure node can be found via the server object. 

                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out IList<IReference> references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }

                Machines.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
                references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, Machines.NodeId));


                // 添加第一个节点数据

                PropertyState<int> machineA = new PropertyState<int>(Machines);
                machineA.WriteMask = AttributeWriteMask.WriteMask;
                machineA.Value = 1000;

                Machines.AddChild(machineA);



                PropertyState property = new PropertyState(Machines);

                property.NodeId = new NodeId(2, NamespaceIndex);
                property.BrowseName = new QualifiedName("Machine A", NamespaceIndex);
                property.DisplayName = property.BrowseName.Name;
                property.TypeDefinitionId = VariableTypeIds.PropertyType;
                property.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                property.DataType = DataTypeIds.Int32;
                property.ValueRank = ValueRanks.Scalar;
                property.Value = 1000;


                Machines.AddChild(property);



                // 添加第一个节点数据
                property = new PropertyState(Machines);

                property.NodeId = new NodeId(3, NamespaceIndex);
                property.BrowseName = new QualifiedName("Matrix", NamespaceIndex);
                property.DisplayName = property.BrowseName.Name;
                property.TypeDefinitionId = VariableTypeIds.PropertyType;
                property.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                property.DataType = DataTypeIds.Int32;
                property.ValueRank = ValueRanks.TwoDimensions;
                property.ArrayDimensions = new ReadOnlyList<uint>(new uint[] { 2, 2 });

                Machines.AddChild(property);

                // save in dictionary. 
                AddPredefinedNode(SystemContext, Machines);

                ReferenceTypeState referenceType = new ReferenceTypeState();

                referenceType.NodeId = new NodeId(4, NamespaceIndex);
                referenceType.BrowseName = new QualifiedName("IsTriggerSource", NamespaceIndex);
                referenceType.DisplayName = referenceType.BrowseName.Name;
                referenceType.InverseName = new LocalizedText("IsSourceOfTrigger");
                referenceType.SuperTypeId = ReferenceTypeIds.NonHierarchicalReferences;

                if (!externalReferences.TryGetValue(ObjectIds.Server, out references))
                {
                    externalReferences[ObjectIds.Server] = references = new List<IReference>();
                }

                Machines.AddReference(referenceType.NodeId, false, ObjectIds.Server);
                references.Add(new NodeStateReference(referenceType.NodeId, true, Machines.NodeId));

                // save in dictionary. 
                AddPredefinedNode(SystemContext, referenceType);
            }
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
        #endregion

        #region Private Fields
        private CustomerServerConfiguration m_configuration;
        #endregion




    }
}
