using Mall3s.Dependency;
using System;

namespace Mall3s.DataValidation
{
    /// <summary>
    /// 验证类型特性
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Enum)]
    public sealed class ValidationTypeAttribute : Attribute
    {
    }
}