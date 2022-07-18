using Mall3s.Dependency;
using System;

namespace Mall3s.RemoteRequest
{
    /// <summary>
    /// 配置序列化选项
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class JsonSerializerOptionsAttribute : Attribute
    {
    }
}