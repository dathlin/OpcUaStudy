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

namespace WindowsFormsAppServer
{
    /// <summary>
    /// Defines the possible tag types
    /// 定义可能的表情类型
    /// </summary>
    public enum UnderlyingSystemTagType
    {
        /// <summary>
        /// The tag has no special characteristics.
        /// 标签没有特殊的特征
        /// </summary>
        Normal = 0,

        /// <summary>
        /// The tag is an analog value with a high and low range.
        /// 标签是具有高和低范围的模拟值。
        /// </summary>
        Analog = 1,

        /// <summary>
        /// The tag is a digital value with a true and false state.
        /// 标签是具有真假状态的数字值。
        /// </summary>
        Digital = 2,

        /// <summary>
        /// The tag is a enumerated value with set of names states.
        /// 标签是一个枚举值，具有一组名称状态。
        /// </summary>
        Enumerated = 3
    }
}
