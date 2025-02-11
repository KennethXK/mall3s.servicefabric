﻿using Mall3s.Dependency;
using Mall3s.UnifyResult;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 状态码中间件拓展
    /// </summary>
    [SuppressSniffer]
    public static class UnifyResultMiddlewareExtensions
    {
        /// <summary>
        /// 添加状态码拦截中间件
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseUnifyResultStatusCodes(this IApplicationBuilder builder)
        {
            // 注册中间件
            builder.UseMiddleware<UnifyResultStatusCodesMiddleware>();

            return builder;
        }
    }
}