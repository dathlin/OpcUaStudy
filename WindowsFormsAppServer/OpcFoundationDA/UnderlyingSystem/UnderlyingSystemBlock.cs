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
using Opc.Ua;

namespace WindowsFormsAppServer
{
    /// <summary>
    /// This class simulates a block in the system.
    /// 该类模拟系统中的块
    /// </summary>
    public class UnderlyingSystemBlock
    {
        #region Public Members
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderlyingSystemBlock"/> class.
        /// </summary>
        public UnderlyingSystemBlock()
        {
            m_tags = new List<UnderlyingSystemTag>();
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Gets or sets the unique identifier for the block.
        /// 获取或设置块的唯一标识符
        /// </summary>
        /// <value>The unique identifier for the block.</value>
        public string Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the block.
        /// 获取或设置块的名称
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        /// <summary>
        /// Gets or sets the type of the block.
        /// 获取或设置块的类型
        /// </summary>
        /// <value>The type of the block.</value>
        public string BlockType
        {
            get { return m_blockType; }
            set { m_blockType = value; }
        }

        /// <summary>
        /// Gets or sets the time when the block structure last changed.
        /// 获取或设置块结构最后更改的时间
        /// </summary>
        /// <value>When the block structure last changed.</value>
        public DateTime Timestamp
        {
            get { return m_timestamp; }
            set { m_timestamp = value; }
        }

        /// <summary>
        /// Creates the tag.
        /// 新建一个标签
        /// </summary>
        /// <param name="tagName">标签的名称</param>
        /// <param name="dataType">标签的数据类型</param>
        /// <param name="tagType">标签的类型</param>
        /// <param name="engineeringUnits">标签的工程单位</param>
        /// <param name="writeable">如果设置为 <c>true</c> 标签是可写入的</param>
        public void CreateTag(
            string tagName,
            UnderlyingSystemDataType dataType,
            UnderlyingSystemTagType tagType,
            string engineeringUnits,
            bool writeable)
        {
            // 新建标签


            UnderlyingSystemTag tag = new UnderlyingSystemTag()
            {
                Block = this,
                Name = tagName,
                Description = null,
                EngineeringUnits = engineeringUnits,
                DataType = dataType,
                TagType = tagType,
                IsWriteable = writeable,
                Labels = null,
                EuRange = null
            };

            
            switch (tagType)
            {
                case UnderlyingSystemTagType.Analog:
                    {
                        tag.Description = "An analog value.";
                        tag.TagType = UnderlyingSystemTagType.Analog;
                        tag.EuRange = new double[] { 100, 0 };
                        break;
                    }

                case UnderlyingSystemTagType.Digital:
                    {
                        tag.Description = "A digital value.";
                        tag.TagType = UnderlyingSystemTagType.Digital;
                        tag.Labels = new string[] { "Online", "Offline" };
                        break;
                    }

                case UnderlyingSystemTagType.Enumerated:
                    {
                        tag.Description = "An enumerated value.";
                        tag.TagType = UnderlyingSystemTagType.Enumerated;
                        tag.Labels = new string[] { "Red", "Yellow", "Green" };
                        break;
                    }

                default:
                    {
                        tag.Description = "A generic value.";
                        break;
                    }
            }

            // set an initial value.
            switch (tag.DataType)
            {
                case UnderlyingSystemDataType.Integer1: { tag.Value = (sbyte)0; break; }
                case UnderlyingSystemDataType.Integer2: { tag.Value = (short)0; break; }
                case UnderlyingSystemDataType.Integer4: { tag.Value = 0; break; }
                case UnderlyingSystemDataType.Real4: { tag.Value = (float)0; break; }
                case UnderlyingSystemDataType.String: { tag.Value = String.Empty; break; }
                case UnderlyingSystemDataType.Boolean: { tag.Value = false; break; }
                case UnderlyingSystemDataType.Byte: { tag.Value = (byte)0; break; }
                case UnderlyingSystemDataType.SByte: { tag.Value = (sbyte)0; break; }
                case UnderlyingSystemDataType.Int16: { tag.Value = (short)0; break; }
                case UnderlyingSystemDataType.UInt16: { tag.Value = (ushort)0; break; }
                case UnderlyingSystemDataType.Int32: { tag.Value = 0; break; }
                case UnderlyingSystemDataType.UInt32: { tag.Value = (uint)0; break; }
                case UnderlyingSystemDataType.Int64: { tag.Value = (long)0; break; }
                case UnderlyingSystemDataType.UInt64: { tag.Value = (ulong)0; break; }
                case UnderlyingSystemDataType.Float: { tag.Value = (float)0; break; }
                case UnderlyingSystemDataType.Double: { tag.Value = (double)0; break; }
                case UnderlyingSystemDataType.Decimal128: { tag.Value = (decimal)0; break; }
                case UnderlyingSystemDataType.DateTime: { tag.Value = DateTime.Now; break; }
                case UnderlyingSystemDataType.Guid: { tag.Value = Guid.NewGuid(); break; }
            }

            lock (m_tags)
            {
                m_tags.Add(tag);
                m_timestamp = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Returns a snapshot of the tags belonging to the block.
        /// 返回属于该块的标签的快照
        /// </summary>
        /// <returns>The list of tags. Null if the block does not exist.</returns>
        public IList<UnderlyingSystemTag> GetTags()
        {
            lock (m_tags)
            {
                // create snapshots of the tags.
                UnderlyingSystemTag[] tags = new UnderlyingSystemTag[m_tags.Count];

                for (int ii = 0; ii < m_tags.Count; ii++)
                {
                    tags[ii] = m_tags[ii].CreateSnapshot();
                }

                return tags;
            }
        }

        /// <summary>
        /// Starts the monitoring.
        /// 开始监视
        /// </summary>
        /// <param name="callback">报告更改时使用的回调方法</param>
        public void StartMonitoring(TagsChangedEventHandler callback)
        {
            lock (m_tags)
            {
                OnTagsChanged = callback;
            }
        }

        /// <summary>
        /// Writes the tag value.
        /// 写入一个标签值
        /// </summary>
        /// <param name="tagName">标签的名称</param>
        /// <param name="value">值</param>
        /// <returns>操作后的状态码</returns>
        public uint WriteTagValue(string tagName, object value)
        {
            UnderlyingSystemTag tag = null;
            TagsChangedEventHandler onTagsChanged = null;

            lock (m_tags)
            {
                onTagsChanged = OnTagsChanged;

                // find the tag.
                // 寻找指定名称的标签
                tag = FindTag(tagName);

                if (tag == null)
                {
                    return StatusCodes.BadNodeIdUnknown;
                }

                // cast value to correct type.
                // 将值转化到正确的类型
                try
                {
                    switch (tag.DataType)
                    {
                        case UnderlyingSystemDataType.Integer1:
                            {
                                tag.Value = (sbyte)value;
                                break;
                            }

                        case UnderlyingSystemDataType.Integer2:
                            {
                                tag.Value = (short)value;
                                break;
                            }

                        case UnderlyingSystemDataType.Integer4:
                            {
                                tag.Value = (int)value;
                                break;
                            }

                        case UnderlyingSystemDataType.Real4:
                            {
                                tag.Value = (float)value;
                                break;
                            }

                        case UnderlyingSystemDataType.String:
                            {
                                tag.Value = (string)value;
                                break;
                            }

                        case UnderlyingSystemDataType.Boolean:
                            {
                                tag.Value = (Boolean)value;
                                break;
                            }

                        case UnderlyingSystemDataType.Byte:
                            {
                                tag.Value = (byte)value;
                                break;
                            }

                        case UnderlyingSystemDataType.SByte:
                            {
                                tag.Value = (sbyte)value;
                                break;
                            }

                        case UnderlyingSystemDataType.Int16:
                            {
                                tag.Value = (short)value;
                                break;
                            }

                        case UnderlyingSystemDataType.UInt16:
                            {
                                tag.Value = (ushort)value;
                                break;
                            }

                        case UnderlyingSystemDataType.Int32:
                            {
                                tag.Value = (int)value;
                                break;
                            }

                        case UnderlyingSystemDataType.UInt32:
                            {
                                tag.Value = (uint)value;
                                break;
                            }

                        case UnderlyingSystemDataType.Int64:
                            {
                                tag.Value = (long)value;
                                break;
                            }

                        case UnderlyingSystemDataType.UInt64:
                            {
                                tag.Value = (ulong)value;
                                break;
                            }

                        case UnderlyingSystemDataType.Float:
                            {
                                tag.Value = (float)value;
                                break;
                            }

                        case UnderlyingSystemDataType.Double:
                            {
                                tag.Value = (double)value;
                                break;
                            }

                        case UnderlyingSystemDataType.Decimal128:
                            {
                                tag.Value = (decimal)value;
                                break;
                            }

                        case UnderlyingSystemDataType.DateTime:
                            {
                                tag.Value = (DateTime)value;
                                break;
                            }

                        case UnderlyingSystemDataType.Guid:
                            {
                                tag.Value = (Guid)value;
                                break;
                            }
                    }
                }
                catch
                {
                    // 除上述以外其他乱七八糟的值
                    return StatusCodes.BadTypeMismatch;
                }

                // updated the timestamp.
                // 更新时间戳
                tag.Timestamp = DateTime.UtcNow;
            }

            // raise notification.
            // 引发一个通知

            if (tag != null)
            {
                onTagsChanged?.Invoke(new UnderlyingSystemTag[] { tag });
            }

            return StatusCodes.Good;
        }

        /// <summary>
        /// Stops monitoring.
        /// 停止监视
        /// </summary>
        public void StopMonitoring()
        {
            lock (m_tags)
            {
                OnTagsChanged = null;
            }
        }

        /// <summary>
        /// Simulates a block by updating the state of the tags belonging to the condition.
        /// 通过更新属于该条件的标签的状态来模拟块
        /// </summary>
        /// <param name="counter">已经过去的模拟循环次数</param>
        /// <param name="index">系统内的块的索引</param>
        /// <param name="generator">生成随机数据的对象</param>
        public void DoSimulation(long counter, int index, Opc.Ua.Test.DataGenerator generator)
        {
            try
            {
                TagsChangedEventHandler onTagsChanged = null;
                List<UnderlyingSystemTag> snapshots = new List<UnderlyingSystemTag>();

                // update the tags.
                lock (m_tags)
                {
                    onTagsChanged = OnTagsChanged;

                    // do nothing if not monitored.
                    if (onTagsChanged == null)
                    {
                        return;
                    }

                    for (int ii = 0; ii < m_tags.Count; ii++)
                    {
                        UnderlyingSystemTag tag = m_tags[ii];
                        UpdateTagValue(tag, generator);

                        DataValue value = new DataValue()
                        {
                            Value = tag.Value,
                            StatusCode = StatusCodes.Good,
                            SourceTimestamp = tag.Timestamp
                        };
                        if (counter % (8 + (index % 4)) == 0)
                        {
                            UpdateTagMetadata(tag, generator);
                        }

                        snapshots.Add(tag.CreateSnapshot());
                    }
                }

                // report any tag changes after releasing the lock.
                onTagsChanged?.Invoke(snapshots);
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error running simulation for block {0}", m_name);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Finds the tag identified by the name.
        /// 通过标签的名称来寻找标签
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>The tag if null; otherwise null.</returns>
        private UnderlyingSystemTag FindTag(string tagName)
        {
            lock (m_tags)
            {
                // look up tag.
                for (int ii = 0; ii < m_tags.Count; ii++)
                {
                    UnderlyingSystemTag tag = m_tags[ii];

                    if (tag.Name == tagName)
                    {
                        return tag;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Updates the value of an tag.
        /// 更新标签的值
        /// </summary>
        private bool UpdateTagValue(
            UnderlyingSystemTag tag,
            Opc.Ua.Test.DataGenerator generator)
        {
            // don't update writeable tags.
            // 不支持更新的情况
            if (tag.IsWriteable)
            {
                return false;
            }

            // check if a range applies to the value.
            int high = 0;
            int low = 0;

            switch (tag.TagType)
            {
                case UnderlyingSystemTagType.Analog:
                    {
                        if (tag.EuRange != null && tag.EuRange.Length >= 2)
                        {
                            high = (int)tag.EuRange[0];
                            low = (int)tag.EuRange[1];
                        }

                        break;
                    }

                case UnderlyingSystemTagType.Digital:
                    {
                        high = 1;
                        low = 0;
                        break;
                    }

                case UnderlyingSystemTagType.Enumerated:
                    {
                        if (tag.Labels != null && tag.Labels.Length > 0)
                        {
                            high = tag.Labels.Length - 1;
                            low = 0;
                        }

                        break;
                    }
            }

            // select a value in the range.
            int value = -1;

            if (high > low)
            {
                value = (generator.GetRandomUInt16() % (high - low + 1)) + low;
            }

            // cast value to correct type or generate a random value.
            // 如果value仍然是-1的，则代表随便取值
            switch (tag.DataType)
            {
                case UnderlyingSystemDataType.Integer1:
                    {
                        if (value == -1)
                        {
                            tag.Value = generator.GetRandomSByte();
                        }
                        else
                        {
                            tag.Value = (sbyte)value;
                        }

                        break;
                    }

                case UnderlyingSystemDataType.Integer2:
                    {
                        if (value == -1)
                        {
                            tag.Value = generator.GetRandomInt16();
                        }
                        else
                        {
                            tag.Value = (short)value;
                        }

                        break;
                    }

                case UnderlyingSystemDataType.Integer4:
                    {
                        if (value == -1)
                        {
                            tag.Value = generator.GetRandomInt32();
                        }
                        else
                        {
                            tag.Value = (int)value;
                        }

                        break;
                    }

                case UnderlyingSystemDataType.Real4:
                    {
                        if (value == -1)
                        {
                            tag.Value = generator.GetRandomFloat();
                        }
                        else
                        {
                            tag.Value = (float)value;
                        }

                        break;
                    }

                case UnderlyingSystemDataType.String:
                    {
                        tag.Value = generator.GetRandomString();
                        break;
                    }
            }

            tag.Timestamp = DateTime.UtcNow;
            return true;
        }

        /// <summary>
        /// Updates the metadata for a tag.
        /// 更新标签的元数据
        /// </summary>
        private bool UpdateTagMetadata(
            UnderlyingSystemTag tag,
            Opc.Ua.Test.DataGenerator generator)
        {
            switch (tag.TagType)
            {
                case UnderlyingSystemTagType.Analog:
                    {
                        if (tag.EuRange != null)
                        {
                            double[] range = new double[tag.EuRange.Length];

                            for (int ii = 0; ii < tag.EuRange.Length; ii++)
                            {
                                range[ii] = tag.EuRange[ii] + 1;
                            }

                            tag.EuRange = range;
                        }

                        break;
                    }

                case UnderlyingSystemTagType.Digital:
                case UnderlyingSystemTagType.Enumerated:
                    {
                        if (tag.Labels != null)
                        {
                            string[] labels = new string[tag.Labels.Length];

                            for (int ii = 0; ii < tag.Labels.Length; ii++)
                            {
                                labels[ii] = generator.GetRandomString();
                            }

                            tag.Labels = labels;
                        }

                        break;
                    }

                default:
                    {
                        return false;
                    }
            }

            return true;
        }
        #endregion

        #region Private Fields
        private object m_lock = new object();
        private string m_id;
        private string m_name;
        private string m_blockType;
        private DateTime m_timestamp;
        private List<UnderlyingSystemTag> m_tags;
        private TagsChangedEventHandler OnTagsChanged;
        #endregion
    }

    /// <summary>
    /// Used to receive events when the state of an tag changes.
    /// </summary>
    public delegate void TagsChangedEventHandler(IList<UnderlyingSystemTag> tags);
}
