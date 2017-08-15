using System;
using System.Text;
using Opc.Ua;

namespace Opc.Ua.Hsl
{
    /// <summary>
    /// Stores information about a NodeId specified by the client.
    /// 存储客户端指定的NodeId信息
    /// </summary>
    /// <remarks>
    /// A NodeHandle is created when GetManagerHandle is called and will only contain
    /// information found by parsing the NodeId. The ValidateNode method is used to 
    /// verify that the NodeId refers to a real Node and find a NodeState object that 
    /// can be used to access the Node.
    /// 当调用GetManagerHandle时，NodeHandle将被创建，并且只包含
    /// 通过解析NodeId找到的信息。 ValidateNode方法用于验证NodeId是否指向真实的Node，并找到可用于访问Node的NodeState对象。
    /// </remarks>
    public class NodeHandle
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeHandle"/> class.
        /// </summary>
        public NodeHandle()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeHandle"/> class.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="node">The node.</param>
        public NodeHandle(NodeId nodeId, NodeState node)
        {
            this.NodeId = nodeId;
            this.Validated = true;
            this.Node = node;
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// The NodeId provided by the client.
        /// </summary>
        public NodeId NodeId { get; set; }

        /// <summary>
        /// A unique string identifier for the root of a complex object tree.
        /// </summary>
        public string RootId
        {
            get
            {
                if (ParsedNodeId != null)
                {
                    return ParsedNodeId.RootId;
                }

                return null;
            }
        }

        /// <summary>
        /// A path to a component within the tree identified by the root id.
        /// </summary>
        public string ComponentPath
        {
            get
            {
                if (ParsedNodeId != null)
                {
                    return ParsedNodeId.ComponentPath;
                }

                return null;
            }
        }

        /// <summary>
        /// The parsed identifier (must not be null if Validated == False).
        /// </summary>
        public ParsedNodeId ParsedNodeId { get; set; }

        /// <summary>
        /// An index associated with the handle.
        /// </summary>
        /// <remarks>
        /// This is used to keep track of the position in the complete list of Nodes provided by the Client.
        /// </remarks>
        public int Index { get; set; }

        /// <summary>
        /// Whether the handle has been validated.
        /// </summary>
        /// <remarks>
        /// When validation is complete the Node property must have a valid object.
        /// </remarks>
        public bool Validated { get; set; }

        /// <summary>
        /// An object that can be used to access the Node identified by the NodeId.
        /// </summary>
        /// <remarks>
        /// Not set until after the handle is validated.
        /// </remarks>
        public NodeState Node { get; set; }

        /// <summary>
        /// An object that can be used to manage the items which are monitoring the node.
        /// </summary>
        public MonitoredNode MonitoredNode { get; set; }
        #endregion
    }
}
