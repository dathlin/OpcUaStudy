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

namespace Opc.Ua.Hsl
{
    /// <summary>
    /// Defines the possible tag data types
    /// 定义可能的标签数据类型
    /// </summary>
    public enum UnderlyingSystemDataType
    {
        /// <summary>
        /// A 1-byte integer value.
        /// 一个字节的整型数据
        /// </summary>
        Integer1 = 0,

        /// <summary>
        /// A 2-byte integer value.
        /// 两个字节的整型数据
        /// </summary>
        Integer2 = 1,

        /// <summary>
        /// A 4-byte integer value.
        /// 四个字节的整型数据
        /// </summary>
        Integer4 = 2,

        /// <summary>
        /// A 4-byte floating point value.
        /// 四个字节的浮点数
        /// </summary>
        Real4 = 3,

        /// <summary>
        /// A string value.
        /// 一个字符串数据
        /// </summary>
        String = 4
    }
}
