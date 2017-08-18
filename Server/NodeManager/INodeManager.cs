/* ========================================================================
 * Copyright (c) 2005-2017 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Opc.Ua.Server
{
    /// <summary>
    /// An interface to an object that manages a set of nodes in the address space.
    /// 用于管理地址空间中的一组节点的对象的接口。
    /// </summary>
    public interface INodeManager
    {
        /// <summary>
        /// Returns the NamespaceUris for the Nodes belonging to the NodeManager.
        /// 返回属于NodeManager的节点的NamespaceUris
        /// </summary>
        /// <remarks>
        /// By default the MasterNodeManager uses the namespaceIndex to determine who owns an Node.
        /// 
        /// Servers that do not wish to partition their address space this way must provide their own
        /// implementation of MasterNodeManager.GetManagerHandle().
        /// 
        /// NodeManagers which depend on a custom partitioning scheme must return a null value.
        /// 
        /// 
        /// 默认情况下，MasterNodeManager使用namespaceIndex来确定谁拥有一个节点。
        ///
        /// 不希望以这种方式分配地址空间的服务器必须自己实现MasterNodeManager.GetManagerHandle（）。
        ///
        /// 依赖于自定义分区方案的NodeManager必须返回一个空值。
        /// 
        /// </remarks>
        IEnumerable<string> NamespaceUris { get; }

        /// <summary>
        /// Creates the address space by loading any configuration information an connecting to an underlying system (if applicable).
        /// 通过加载连接到底层系统的任何配置信息（如果适用）来创建地址空间。
        /// </summary>
        /// <returns>A table of references that need to be added to other node managers.
        /// 需要添加到其他节点管理器的引用表。
        /// </returns>
        /// <remarks>
        /// A node manager owns a set of nodes. These nodes may be known in advance or they may be stored in an
        /// external system are retrived on demand. These nodes may have two way references to nodes that are owned 
        /// by other node managers. In these cases, the node managers only manage one half of those references. The
        /// other half of the reference should be returned to the MasterNodeManager.
        /// 节点管理器拥有一组节点。 可以预先知道这些节点，或者它们可以被存储在外部系统中。 
        /// 这些节点可能具有对由其他节点管理器拥有的节点的双向引用。 在这些情况下，节点管理器只管理这些引用的一半。 
        /// 引用的另一半应该返回到MasterNodeManager。
        /// </remarks>
        void CreateAddressSpace(IDictionary<NodeId,IList<IReference>> externalReferences);

        /// <summary>
        /// Deletes the address by releasing all resources and disconnecting from any underlying system.
        /// 通过释放所有资源并与任何底层系统断开连接来删除该地址。
        /// </summary>
        void DeleteAddressSpace();

        /// <summary>
        /// Returns an opaque handle identifying to the node to the node manager.
        /// 返回标识到节点管理器的节点的不透明句柄。
        /// </summary>
        /// <returns>A node handle, null if the node manager does not recognize the node id.</returns>
        /// <remarks>
        /// The method must not block by querying an underlying system. If the node manager wraps an 
        /// underlying system then it must check to see if it recognizes the syntax of the node id. 
        /// The handle in this case may simply be a partially parsed version of the node id. 
        /// 该方法不能通过查询底层系统来阻止。 如果节点管理器封装了底层系统，那么它必须检查它是否识别节点id的语法。 
        /// 在这种情况下，句柄可能只是节点ID的部分解析版本。
        /// </remarks>
        object GetManagerHandle(NodeId nodeId);

        /// <summary>
        /// Adds references to the node manager.
        /// 添加对节点管理器的引用
        /// </summary>
        /// <remarks>
        /// The node manager checks the dictionary for nodes that it owns and ensures the associated references exist.
        /// 节点管理器检查字典中是否拥有它的节点，并确保相关联的引用存在。
        /// </remarks>
        void AddReferences(IDictionary<NodeId,IList<IReference>> references);
               
        /// <summary>
        /// Deletes a reference.
        /// 删除一个引用
        /// </summary>
        ServiceResult DeleteReference(
            object         sourceHandle, 
            NodeId         referenceTypeId,
            bool           isInverse, 
            ExpandedNodeId targetId, 
            bool           deleteBidirectional);

        /// <summary>
        /// Returns the metadata associated with the node.
        /// 返回与节点相关联的元数据
        /// </summary>
        /// <remarks>
        /// Returns null if the node does not exist.
        /// 如果节点不存在就返回空
        /// </remarks>
        NodeMetadata GetNodeMetadata(
            OperationContext context,
            object           targetHandle,
            BrowseResultMask resultMask);

        /// <summary>
        /// Returns the set of references that meet the filter criteria.
        /// 返回符合过滤条件的引用集合。
        /// </summary>
        /// <param name="context">在处理请求时使用的上下文。</param>
        /// <param name="continuationPoint">存储浏览操作状态的连续点。</param>
        /// <param name="references">符合过滤条件的引用列表。</param>     
        /// <remarks>
        /// NodeManagers will likely have references to other NodeManagers which means they will not be able
        /// to apply the NodeClassMask or fill in the attributes for the target Node. In these cases the 
        /// NodeManager must return a ReferenceDescription with the NodeId and ReferenceTypeId set. The caller will
        /// be responsible for filling in the target attributes. 
        /// The references parameter may already contain references when the method is called. The implementer must 
        /// include these references when calculating whether a continutation point must be returned.
        /// 
        /// NodeManager可能会引用其他NodeManagers，这意味着它们将无法应用NodeClassMask或填充目标节点的属性。 
        /// 在这些情况下，NodeManager必须返回一个ReferenceDescription，并设置NodeId和ReferenceTypeId。 来电者将负责填写目标属性。
        /// 当调用该方法时，references参数可能已经包含引用。 在计算是否必须返回连接点时，实现者必须包括这些引用。
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if the context, continuationPoint or references parameters are null.</exception>
        /// <exception cref="ServiceResultException">Thrown if an error occurs during processing.</exception>
        void Browse(
            OperationContext            context,
            ref ContinuationPoint       continuationPoint,
            IList<ReferenceDescription> references);

        /// <summary>
        /// Finds the targets of the relative path from the source node.
        /// 查找源节点的相对路径的目标
        /// </summary>
        /// <param name="context">The context to used when processing the request.</param>
        /// <param name="sourceHandle">The handle for the source node.</param>
        /// <param name="relativePath">The relative path to follow.</param>
        /// <param name="targetIds">The NodeIds for any target at the end of the relative path.</param>
        /// <param name="unresolvedTargetIds">The NodeIds for any local target that is in another NodeManager.</param>
        /// <remarks>
        /// A null context indicates that the server's internal logic is making the call.
        /// The first target in the list must be the target that matches the instance declaration (if applicable).
        /// Any local targets that belong to other NodeManagers are returned as unresolvedTargetIds. 
        /// The caller must check the BrowseName to determine if it matches the relativePath.
        /// The implementor must not throw an exception if the source or target nodes do not exist.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if the sourceHandle, relativePath or targetIds parameters are null.</exception>
        void TranslateBrowsePath(
            OperationContext      context,
            object                sourceHandle, 
            RelativePathElement   relativePath, 
            IList<ExpandedNodeId> targetIds,
            IList<NodeId>         unresolvedTargetIds);

        /// <summary>
        /// Reads the attribute values for a set of nodes.
        /// 读取一组节点的属性值
        /// </summary>
        /// <remarks>
        /// The MasterNodeManager pre-processes the nodesToRead and ensures that:
        ///    - the AttributeId is a known attribute.
        ///    - the IndexRange, if specified, is valid.
        ///    - the DataEncoding and the IndexRange are not specified if the AttributeId is not Value.
        ///
        /// The MasterNodeManager post-processes the values by:
        ///    - sets values[ii].StatusCode to the value of errors[ii].Code
        ///    - creates a instance of DataValue if one does not exist and an errors[ii] is bad.
        ///    - removes timestamps from the DataValue if the client does not want them.
        /// 
        /// The node manager must ignore ReadValueId with the Processed flag set to true.
        /// The node manager must set the Processed flag for any ReadValueId that it processes.
        /// </remarks>
        void Read(
            OperationContext     context,
            double               maxAge,
            IList<ReadValueId>   nodesToRead,
            IList<DataValue>     values,
            IList<ServiceResult> errors);

        /// <summary>
        /// Reads the history of a set of items.
        /// 阅读一组项目的历史
        /// </summary>
        void HistoryRead(
            OperationContext          context,
            HistoryReadDetails        details, 
            TimestampsToReturn        timestampsToReturn, 
            bool                      releaseContinuationPoints, 
            IList<HistoryReadValueId> nodesToRead, 
            IList<HistoryReadResult>  results, 
            IList<ServiceResult>      errors);

        /// <summary>
        /// Writes a set of values.
        /// 写一组值
        /// </summary>
        /// <remarks>
        /// Each node manager should only process node ids that it recognizes. If it processes a value it
        /// must set the Processed flag in the WriteValue structure.
        /// 每个节点管理器应该只处理它识别的节点ID。 如果它处理一个值，它必须在WriteValue结构中设置Processed标志。
        /// </remarks>
        void Write(
            OperationContext     context,
            IList<WriteValue>    nodesToWrite, 
            IList<ServiceResult> errors);
        
        /// <summary>
        /// Updates the history for a set of nodes.
        /// </summary>
        void HistoryUpdate(
            OperationContext            context,
            Type                        detailsType,
            IList<HistoryUpdateDetails> nodesToUpdate, 
            IList<HistoryUpdateResult>  results, 
            IList<ServiceResult>        errors);

        /// <summary>
        /// Calls a method defined on a object.
        /// 调用一个对象中定义的方法
        /// </summary>
        void Call(
            OperationContext         context,
            IList<CallMethodRequest> methodsToCall,
            IList<CallMethodResult>  results,
            IList<ServiceResult>     errors);

        /// <summary>
        /// Tells the NodeManager to report events from the specified notifier.
        /// 告诉NodeManager从指定的通知器报告事件
        /// </summary>
        /// <remarks>
        /// This method may be called multiple times for the name monitoredItemId if the
        /// context for that MonitoredItem changes (i.e. UserIdentity and/or Locales).
        /// 如果该监视项目的上下文（即UserIdentity和/或语言环境）发生更改，则该方法可能被监视项目调用多次。
        /// </remarks>
        ServiceResult SubscribeToEvents(            
            OperationContext    context,
            object              sourceId,
            uint                subscriptionId,
            IEventMonitoredItem monitoredItem,
            bool                unsubscribe);

        /// <summary>
        /// Tells the NodeManager to report events all events from all sources.
        /// 告诉NodeManager从所有来源报告所有事件
        /// </summary>
        /// <remarks>
        /// This method may be called multiple times for the name monitoredItemId if the
        /// context for that MonitoredItem changes (i.e. UserIdentity and/or Locales).
        /// </remarks>
        ServiceResult SubscribeToAllEvents(            
            OperationContext   context,
            uint                subscriptionId,
            IEventMonitoredItem monitoredItem,
            bool                unsubscribe);

        /// <summary>
        /// Tells the NodeManager to refresh any conditions.
        /// 告诉NodeManager刷新任何条件
        /// </summary>
        ServiceResult ConditionRefresh(            
            OperationContext           context,
            IList<IEventMonitoredItem> monitoredItems);

        /// <summary>
        /// Creates a set of monitored items.
        /// 创建一组受监视的项目
        /// </summary>
        void CreateMonitoredItems(
            OperationContext                  context,
            uint                              subscriptionId,
            double                            publishingInterval,
            TimestampsToReturn                timestampsToReturn,
            IList<MonitoredItemCreateRequest> itemsToCreate,
            IList<ServiceResult>              errors,
            IList<MonitoringFilterResult>     filterErrors,
            IList<IMonitoredItem>             monitoredItems,
            ref long                          globalIdCounter);

        /// <summary>
        /// Modifies a set of monitored items.
        /// 修改一组受监视的项目
        /// </summary>
        void ModifyMonitoredItems(
            OperationContext                  context,
            TimestampsToReturn                timestampsToReturn,
            IList<IMonitoredItem>             monitoredItems,
            IList<MonitoredItemModifyRequest> itemsToModify,
            IList<ServiceResult>              errors,
            IList<MonitoringFilterResult>     filterErrors);

        /// <summary>
        /// Deletes a set of monitored items.
        /// 删除一组受监视的项目。
        /// </summary>
        void DeleteMonitoredItems(
            OperationContext      context,
            IList<IMonitoredItem> monitoredItems, 
            IList<bool>           processedItems,
            IList<ServiceResult>  errors);

        /// <summary>
        /// Changes the monitoring mode for a set of monitored items.
        /// 更改一组监视项目的监控模式
        /// </summary>
        void SetMonitoringMode(
            OperationContext      context,
            MonitoringMode        monitoringMode,
            IList<IMonitoredItem> monitoredItems, 
            IList<bool>           processedItems,
            IList<ServiceResult>  errors);
    }

    /// <summary>
    /// An interface to an object that manages a set of nodes in the address space.
    /// 用于管理地址空间中的一组节点的对象的接口
    /// </summary>
    public interface INodeManager2 : INodeManager
    {
        /// <summary>
        /// Called when the session is closed.
        /// 当会话关闭时调用
        /// </summary>
        void SessionClosing(OperationContext context, NodeId sessionId, bool deleteSubscriptions);

        /// <summary>
        /// Returns true if the node is in the view.
        /// 如果该节点在视图里，就返回空
        /// </summary>
        bool IsNodeInView(OperationContext context, NodeId viewId, object nodeHandle);
    }

    /// <summary>
    /// Stores metadata required to process requests related to a node.
    /// 存储处理与节点相关的请求所需的元数据。
    /// </summary>
    public class NodeMetadata
    {
        #region Constructors
        /// <summary>
        /// Initializes the object with its handle and NodeId.
        /// </summary>
        public NodeMetadata(object handle, NodeId nodeId)
        {
            m_handle = handle;
            m_nodeId = nodeId;
        }
        #endregion
        
        #region Public Properties
        /// <summary>
        /// The handle assigned by the NodeManager that owns the Node.
        /// </summary>
        public object Handle
        {
            get { return m_handle; }
        }

        /// <summary>
        /// The canonical NodeId for the Node.
        /// </summary>
        public NodeId NodeId
        {
            get { return m_nodeId; }
        }        

        /// <summary>
        /// The NodeClass for the Node.
        /// </summary>
        public NodeClass NodeClass
        {
            get { return m_nodeClass;  }
            set { m_nodeClass = value; }
        }

        /// <summary>
        /// The BrowseName for the Node.
        /// </summary>
        public QualifiedName BrowseName
        {
            get { return m_browseName;  }
            set { m_browseName = value; }
        }

        /// <summary>
        /// The DisplayName for the Node.
        /// </summary>
        public LocalizedText DisplayName
        {
            get { return m_displayName;  }
            set { m_displayName = value; }
        }

        /// <summary>
        /// The type definition for the Node (if one exists).
        /// </summary>
        public ExpandedNodeId TypeDefinition
        {
            get { return m_typeDefinition;  }
            set { m_typeDefinition = value; }
        }

        /// <summary>
        /// The modelling for the Node (if one exists).
        /// </summary>
        public NodeId ModellingRule
        {
            get { return m_modellingRule;  }
            set { m_modellingRule = value; }
        }

        /// <summary>
        /// Specifies which attributes are writeable.
        /// </summary>
        public AttributeWriteMask WriteMask
        {
            get { return m_writeMask;  }
            set { m_writeMask = value; }
        }

        /// <summary>
        /// Whether the Node can be used with event subscriptions or for historial event queries.
        /// </summary>
        public byte EventNotifier
        {
            get { return m_eventNotifier;  }
            set { m_eventNotifier = value; }
        }
        
        /// <summary>
        /// Whether the Node can be use to read or write current or historical values.
        /// </summary>
        public byte AccessLevel
        {
            get { return m_accessLevel;  }
            set { m_accessLevel = value; }
        }

        /// <summary>
        /// Whether the Node is a Method that can be executed.
        /// </summary>
        public bool Executable
        {
            get { return m_executable;  }
            set { m_executable = value; }
        }

        /// <summary>
        /// The DataType of the Value attribute for Variable or VariableType nodes.
        /// </summary>
        public NodeId DataType
        {
            get { return m_dataType;  }
            set { m_dataType = value; }
        }

        /// <summary>
        /// The ValueRank for the Value attribute for Variable or VariableType nodes.
        /// </summary>
        public int ValueRank
        {
            get { return m_valueRank;  }
            set { m_valueRank = value; }
        }

        /// <summary>
        /// The ArrayDimensions for the Value attribute for Variable or VariableType nodes.
        /// </summary>
        public IList<uint> ArrayDimensions
        {
            get { return m_arrayDimensions;  }
            set { m_arrayDimensions = value; }
        }
        #endregion
        
        #region Private Fields
        private object m_handle;
        private NodeId m_nodeId;
        private NodeClass m_nodeClass;
        private QualifiedName m_browseName;
        private LocalizedText m_displayName;
        private ExpandedNodeId m_typeDefinition;
        private NodeId m_modellingRule;
        private AttributeWriteMask m_writeMask;
        private byte m_eventNotifier;
        private byte m_accessLevel;
        private bool m_executable;
        private NodeId m_dataType;
        private int m_valueRank;
        private IList<uint> m_arrayDimensions;
        #endregion
    }
}
