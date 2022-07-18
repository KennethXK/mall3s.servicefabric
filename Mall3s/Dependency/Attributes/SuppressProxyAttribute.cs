using System;

namespace Mall3s.Dependency
{
    /// <summary>
    /// 跳过全局代理
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Class)]
    public class SuppressProxyAttribute : Attribute
    {
    }
}