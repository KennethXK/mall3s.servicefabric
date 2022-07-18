using Mall3s.Dependency;
using System;

namespace Mall3s.DynamicApiController
{
    /// <summary>
    /// 动态 WebApi 特性
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Class)]
    public sealed class DynamicApiControllerAttribute : Attribute
    {
    }
}