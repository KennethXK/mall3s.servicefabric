﻿using Mall3s.Dependency;

namespace Mall3s.UnifyResult.Internal
{
    /// <summary>
    /// 异常元数据
    /// </summary>
    [SuppressSniffer]
    public sealed class ExceptionMetadata
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int StatusCode { get; internal set; }

        /// <summary>
        /// 错误码
        /// </summary>
        public object ErrorCode { get; internal set; }

        /// <summary>
        /// 错误对象（信息）
        /// </summary>
        public object Errors { get; internal set; }
    }
}
