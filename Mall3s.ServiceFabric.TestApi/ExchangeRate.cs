using Mall3s.Common.Const;
using SqlSugar;
using System;

namespace Mall3s.Demo.BaseData.Entitys
{
    /// <summary>
    /// Demo
    /// </summary>
    [Tenant("mall3s_demo")]
    public class ExchangeRate
    {
        /// <summary>
        /// ID
        /// </summary>
        [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
        public string Id { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "F_CreatorTime")]        
        public DateTime? CreatorTime { get; set; }
        
        /// <summary>
        /// 创建用户ID
        /// </summary>
        [SugarColumn(ColumnName = "F_CreatorUserId")]        
        public string CreatorUserId { get; set; }
        
        /// <summary>
        /// 汇率日期
        /// </summary>     
        public DateTime? ExchangeDate { get; set; }
        
        /// <summary>
        /// 源币种
        /// </summary>
        [SugarColumn(ColumnName = "from")]        
        public string From { get; set; }
        
        /// <summary>
        /// 转换币种
        /// </summary>
        [SugarColumn(ColumnName = "to")]        
        public string To { get; set; }
        
        /// <summary>
        /// 汇率
        /// </summary>
        [SugarColumn(ColumnName = "rate")]        
        public decimal Rate { get; set; }
        
    }
}