using Mall3s.Dependency;
using System;

namespace Mall3s.Common.Entity
{
    /// <summary>
    /// 定义逻辑删除功能
    /// </summary>
    public interface IDeleteTime
    {
        /// <summary>
        /// 获取或设置 数据逻辑删除时间
        /// </summary>
        DateTime? DeleteTime { get; set; }
    }
}
