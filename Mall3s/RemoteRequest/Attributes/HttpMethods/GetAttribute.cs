using Mall3s.Dependency;
using System;
using System.Net.Http;

namespace Mall3s.RemoteRequest
{
    /// <summary>
    /// HttpGet 请求
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : HttpMethodBaseAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="requestUrl"></param>
        public GetAttribute(string requestUrl) : base(requestUrl, HttpMethod.Get)
        {
        }
    }
}