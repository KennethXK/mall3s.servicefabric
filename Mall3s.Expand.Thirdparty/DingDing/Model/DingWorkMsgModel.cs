using Mall3s.Dependency;

namespace Mall3s.Expand.Thirdparty.DingDing
{
    /// <summary>
    /// 钉钉消息
    /// </summary>
    [SuppressSniffer]
    public class DingWorkMsgModel
    {
        /// <summary>
        /// 接收人
        /// </summary>
        public string toUsers { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 微应用的AgentID
        /// </summary>
        public string agentId { get; set; }
    }
}
