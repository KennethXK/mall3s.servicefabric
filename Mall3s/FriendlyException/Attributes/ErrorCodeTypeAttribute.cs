using Mall3s.Dependency;
using System;

namespace Mall3s.FriendlyException
{
    /// <summary>
    /// 错误代码类型特性
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Enum)]
    public sealed class ErrorCodeTypeAttribute : Attribute
    {
    }
}