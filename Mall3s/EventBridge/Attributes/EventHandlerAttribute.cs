using Mall3s.Dependency;
using System;

namespace Mall3s.EventBridge
{
    /// <summary>
    /// 事件处理程序特性
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class EventHandlerAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EventHandlerAttribute()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="category"></param>
        public EventHandlerAttribute(string category)
        {
            Category = category;
        }

        /// <summary>
        /// 分类名
        /// </summary>
        public string Category { get; set; }
    }
}