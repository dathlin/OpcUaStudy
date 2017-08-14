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
using System.Reflection;

namespace Opc.Ua
{
    /// <summary>
    /// 一个定义了OPC UA应用程序使用的常量的类。
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.CodeGenerator", "1.0.0.0")]
    public static partial class Attributes
    {
        /// <summary>
        /// 节点的标准标识符
        /// </summary>
        public const uint NodeId = 1;

        /// <summary>
        /// The class of the node.
        /// 节点的类标记
        /// </summary>
        public const uint NodeClass = 2;

        /// <summary>
        /// A non-localized, human readable name for the node.
        /// 一个非本地化的，可读取的名称的节点
        /// </summary>
        public const uint BrowseName = 3;

        /// <summary>
        /// A localized, human readable name for the node.
        /// 一个已经本地化的，可读取的名称
        /// </summary>
        public const uint DisplayName = 4;

        /// <summary>
        /// A localized description for the node.
        /// 节点的本地化描述名称
        /// </summary>
        public const uint Description = 5;

        /// <summary>
        /// Indicates which attributes are writable.
        /// 指出哪些属性是可写的
        /// </summary>
        public const uint WriteMask = 6;

        /// <summary>
        /// Indicates which attributes are writable by the current user.
        /// 指示当前用户可写入哪些属性
        /// </summary>
        public const uint UserWriteMask = 7;

        /// <summary>
        /// Indicates that a type node may not be instantiated.
        /// 表示类型节点可能未被实例化
        /// </summary>
        public const uint IsAbstract = 8;

        /// <summary>
        /// Indicates that forward and inverse references have the same meaning.
        /// 表示正向和反向引用具有相同的含义
        /// </summary>
        public const uint Symmetric = 9;

        /// <summary>
        /// The browse name for an inverse reference.
        /// 反向参考的浏览名称
        /// </summary>
        public const uint InverseName = 10;

        /// <summary>
        /// Indicates that following forward references within a view will not cause a loop.
        /// 表示视图中的以下转发引用不会导致循环
        /// </summary>
        public const uint ContainsNoLoops = 11;

        /// <summary>
        /// Indicates that the node can be used to subscribe to events.
        /// 表示该节点可用于订阅事件
        /// </summary>
        public const uint EventNotifier = 12;

        /// <summary>
        /// The value of a variable.
        /// 一个变量的值
        /// </summary>
        public const uint Value = 13;

        /// <summary>
        /// The node id of the data type for the variable value.
        /// 变量值的数据类型的节点ID
        /// </summary>
        public const uint DataType = 14;

        /// <summary>
        /// The number of dimensions in the value.
        /// 数值中的维数
        /// </summary>
        public const uint ValueRank = 15;

        /// <summary>
        /// The length for each dimension of an array value.
        /// 数组值的每个维度的长度
        /// </summary>
        public const uint ArrayDimensions = 16;

        /// <summary>
        /// How a variable may be accessed.
        /// 如何访问变量
        /// </summary>
        public const uint AccessLevel = 17;

        /// <summary>
        /// How a variable may be accessed after taking the user's access rights into account.
        /// 在考虑用户访问权限后，如何访问变量
        /// </summary>
        public const uint UserAccessLevel = 18;

        /// <summary>
        /// Specifies (in milliseconds) how fast the server can reasonably sample the value for changes.
        /// 指定（以毫秒为单位）服务器可以怎样的速度合理地对更改值进行抽样
        /// </summary>
        public const uint MinimumSamplingInterval = 19;

        /// <summary>
        /// Specifies whether the server is actively collecting historical data for the variable.
        /// 指定服务器是否正在积极收集变量的历史数据
        /// </summary>
        public const uint Historizing = 20;

        /// <summary>
        /// Whether the method can be called.
        /// 是否可以调用该方法
        /// </summary>
        public const uint Executable = 21;

        /// <summary>
        /// Whether the method can be called by the current user.
        /// 该方法是否可以由当前用户调用
        /// </summary>
        public const uint UserExecutable = 22;
    }
}