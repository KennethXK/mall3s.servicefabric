using Mall3s.Dependency;
using System;

namespace Mall3s.RemoteRequest
{
    /// <summary>
    /// 代理参数基类特性
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterBaseAttribute : Attribute
    {
    }
}