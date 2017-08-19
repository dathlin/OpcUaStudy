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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;

#pragma warning disable 0618

namespace Opc.Ua.Server
{
    /// <summary>
    /// The interface that a server exposes to objects that it contains.
    /// 服务器暴露给它包含的对象的接口
    /// </summary>
    public interface IServerInternal
    {
        /// <summary>
        /// The endpoint addresses used by the server.
        /// 服务器使用的端点地址
        /// </summary>
        /// <value>The endpoint addresses.</value>
        IEnumerable<Uri> EndpointAddresses { get; }

        /// <summary>
        /// The context to use when serializing/deserializing extension objects.
        /// 序列化/反序列化扩展对象时使用的上下文
        /// </summary>
        /// <value>The message context.</value>
        ServiceMessageContext MessageContext { get; }

        /// <summary>
        /// The default system context for the server.
        /// 服务器的默认系统上下文
        /// </summary>
        /// <value>The default system context.</value>
        ServerSystemContext DefaultSystemContext { get; }

        /// <summary>
        /// The table of namespace uris known to the server.
        /// 服务器已知的命名空间uris表
        /// </summary>
        /// <value>The namespace URIs.</value>
        NamespaceTable NamespaceUris { get; }

        /// <summary>
        /// The table of remote server uris known to the server.
        /// 服务器已知的远程服务器uris表
        /// </summary>
        /// <value>The server URIs.</value>
        StringTable ServerUris { get; }

        /// <summary>
        /// The factory used to create encodeable objects that the server understands.
        /// 工厂用于创建服务器可理解的可编码对象
        /// </summary>
        /// <value>The factory.</value>
        EncodeableFactory Factory { get; }

        /// <summary>
        /// The datatypes, object types and variable types known to the server.
        /// 服务器已知的数据类型，对象类型和变量类型
        /// </summary>
        /// <value>The type tree.</value>
        /// <remarks>
        /// The type tree table is a global object that all components of a server have access to.
        /// Node managers must populate this table with all types that they define. 
        /// This object is thread safe.
        /// 
        /// 类型树表是服务器的所有组件都可以访问的全局对象。
        /// 节点管理器必须使用它们定义的所有类型填充此表。
        /// 他的对象是线程安全的。
        /// 
        /// </remarks>
        TypeTable TypeTree { get; }

#if LEGACY_CORENODEMANAGER
        /// <summary>
        /// Returns the source for a types that has shared components defined.
        /// </summary>
        /// <value>The type sources.</value>
        /// <remarks>
        /// Some types define shared components which are used by all instances of the type. This
        /// table contains sources for those shared components. The namespace qualified browse name
        /// is assumed to be a unique identifier for a type.
        /// </remarks>
        TypeSourceTable TypeSources { get; }
#endif

        /// <summary>
        /// The master node manager for the server.
        /// 服务器的主节点管理器
        /// </summary>
        /// <value>The node manager.</value>
        MasterNodeManager NodeManager { get; }

        /// <summary>
        /// The internal node manager for the servers.
        /// 服务器的内部节点管理器
        /// </summary>
        /// <value>The core node manager.</value>
        CoreNodeManager CoreNodeManager { get; }

        /// <summary>
        /// Returns the node manager that managers the server diagnostics.
        /// 返回管理服务器诊断的节点管理器
        /// </summary>
        /// <value>The diagnostics node manager.</value>
        DiagnosticsNodeManager DiagnosticsNodeManager { get; }

        /// <summary>
        /// The manager for events that all components use to queue events that occur.
        /// 所有组件用于排队发生事件的事件的管理器
        /// </summary>
        /// <value>The event manager.</value>
        EventManager EventManager { get; }

        /// <summary>
        /// A manager for localized resources that components can use to localize text.
        /// 组件可用于本地化文本的本地化资源管理器
        /// </summary>
        /// <value>The resource manager.</value>
        ResourceManager ResourceManager { get; }

        /// <summary>
        /// A manager for outstanding requests that allows components to receive notifications if the timeout or are cancelled.
        /// 未完成的请求的管理，允许组件在超时或取消时接收通知
        /// </summary>
        /// <value>The request manager.</value>
        RequestManager RequestManager { get; }

        /// <summary>
        /// A manager for aggregate calculators supported by the server.
        /// 由服务器支持的聚合计算器的管理
        /// </summary>
        /// <value>The aggregate manager.</value>
        AggregateManager AggregateManager { get; }

        /// <summary>
        /// The manager for active sessions.
        /// 活动会话的管理
        /// </summary>
        /// <value>The session manager.</value>
        ISessionManager SessionManager { get; }

        /// <summary>
        /// The manager for active subscriptions.
        /// 活动订阅的管理
        /// </summary>
        ISubscriptionManager SubscriptionManager { get; }
        
        /// <summary>
        /// Whether the server is currently running.
        /// 只是系统当前是否在运行中
        /// </summary>
        /// <value>
        /// 	<c>true</c> 如果示例处于运行中; 否则, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This flag is set to false when the server shuts down. Threads running should check this flag whenever
        /// they return from a blocking operation. If it is false the thread should clean up and terminate.
        /// </remarks>
        bool IsRunning { get; }

        /// <summary>
        /// Returns the status object for the server.
        /// 返回服务器的状态对象
        /// </summary>
        /// <value>The status.</value>
        [Obsolete("No longer thread safe. Must not use.")]
        ServerStatusValue Status { get; }

        /// <summary>
        /// Gets or sets the current state of the server.
        /// 获取或设置服务器的当前状态，主要有运行，关机，挂起，等等
        /// </summary>
        /// <value>The state of the current.</value>
        ServerState CurrentState { get; set; }

        /// <summary>
        /// Used to synchronize access to the server diagnostics.
        /// 用于同步对服务器诊断的访问
        /// </summary>
        /// <value>The diagnostics lock.</value>
        object DiagnosticsLock { get; }

        /// <summary>
        /// Returns the diagnostics structure for the server.
        /// 返回服务器的诊断结构
        /// </summary>
        /// <value>The server diagnostics.</value>
        ServerDiagnosticsSummaryDataType ServerDiagnostics { get; }

#if LEGACY_CORENODEMANAGER
        /// <summary>
        /// Returns the diagnostics object for the server.
        /// </summary>
        /// <value>The diagnostics.</value>
        [Obsolete("No longer thread safe. Replaced by ServerDiagnostics.")]
        ServerDiagnostics Diagnostics { get; }
#endif

        /// <summary>
        /// Whether the server is collecting diagnostics.
        /// 服务器是否正在收集诊断
        /// </summary>
        /// <value><c>true</c> if diagnostics is enabled; otherwise, <c>false</c>.</value>
        bool DiagnosticsEnabled { get; }

        /// <summary>
        /// Closes the specified session.
        /// 关闭指定的会话，需要指定会话的节点标识
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="deleteSubscriptions">if set to <c>true</c> subscriptions are to be deleted.</param>
        void CloseSession(OperationContext context, NodeId sessionId, bool deleteSubscriptions);

        /// <summary>
        /// Deletes the specified subscription.
        /// 删除指定的订阅信息
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        void DeleteSubscription(uint subscriptionId);

        /// <summary>
        /// Called by any component to report a global event.
        /// 由任何组件调用来报告全局事件
        /// </summary>
        /// <param name="e">The event.</param>
        void ReportEvent(IFilterTarget e);

        /// <summary>
        /// Called by any component to report a global event.
        /// 由任何组件调用来报告全局事件
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="e">The event.</param>
        void ReportEvent(ISystemContext context, IFilterTarget e);

        /// <summary>
        /// Refreshes the conditions for the specified subscription.
        /// 刷新指定订阅的条件
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        void ConditionRefresh(OperationContext context, uint subscriptionId);
    }
}
