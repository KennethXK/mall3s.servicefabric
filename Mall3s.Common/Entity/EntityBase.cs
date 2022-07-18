using Mall3s.Common.Extension;
using Mall3s.Dependency;
using SqlSugar;
using System;

namespace Mall3s.Common.Entity
{
    /// <summary>
    /// 实体类基类
    /// </summary>
    [SuppressSniffer]
    public abstract class EntityBase<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// 获取或设置 编号
        /// </summary>  
        [SugarColumn(ColumnName = "F_Id", ColumnDescription = "主键", IsPrimaryKey = true)]
        public TKey Id { get; set; }
    }
}
