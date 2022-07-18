using Mall3s.Dependency;

namespace Mall3s.Common.Filter
{
    /// <summary>
    /// 关键字输入
    /// </summary>
    [SuppressSniffer]
    public class KeywordInput
    {
        /// <summary>
        /// 查询关键字
        /// </summary>
        /// <example>测试</example>
        public string keyword { get; set; }
    }
}
