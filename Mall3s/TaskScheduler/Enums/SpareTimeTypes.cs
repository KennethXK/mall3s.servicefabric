using Mall3s.Dependency;
using System.ComponentModel;

namespace Mall3s.TaskScheduler
{
    /// <summary>
    /// 任务类型
    /// </summary>
    [SuppressSniffer]
    public enum SpareTimeTypes
    {
        /// <summary>
        /// 间隔方式
        /// </summary>
        [Description("间隔方式")]
        Interval,

        /// <summary>
        /// Cron 表达式
        /// </summary>
        [Description("Cron 表达式")]
        Cron
    }
}