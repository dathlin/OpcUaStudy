using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opc.Ua.Hsl
{




    /// <summary>
    /// 报告Opc运行状态时的消息
    /// </summary>
    public class OpcStausEventArgs : EventArgs
    {
        /// <summary>
        /// 指示该状态是否为一个错误状态
        /// </summary>
        public bool IsError { get; set; }
        /// <summary>
        /// 指示状态发生的时间
        /// </summary>
        public DateTime OccurTime { get; set; }
        /// <summary>
        /// 指示状态发生的文本描述
        /// </summary>
        public string Status { get; set; }
    }



}
