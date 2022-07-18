using Mall3s.Dependency;
using System;
using System.Net.Http;

namespace Mall3s.RemoteRequest
{
    /// <summary>
    /// HttpPatch 请求
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Method)]
    public class PatchAttribute : HttpMethodBaseAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="requestUrl"></param>
        public PatchAttribute(string requestUrl) : base(requestUrl, HttpMethod.Patch)
        {
        }
    }
}