﻿using Mall3s.Dependency;
using System;
using System.Net.Http;

namespace Mall3s.RemoteRequest
{
    /// <summary>
    /// HttpPost 请求
    /// </summary>
    [SuppressSniffer, AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : HttpMethodBaseAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="requestUrl"></param>
        public PostAttribute(string requestUrl) : base(requestUrl, HttpMethod.Post)
        {
        }
    }
}