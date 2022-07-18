using Mall3s.Dependency;
using System;

namespace Mall3s.DynamicApiController
{
    /// <summary>
    /// 将 Action 所有参数 [FromQuery] 化
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Method)]
    public sealed class QueryParametersAttribute : Attribute
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public QueryParametersAttribute()
        {
        }
    }
}