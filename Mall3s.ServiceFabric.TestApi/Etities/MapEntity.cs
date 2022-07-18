using Mall3s.Common.Entity;
using SqlSugar;
namespace Mall3s.ServiceFabric.TestApi.Etities
{
    /// <summary>
    /// 地图管理
    /// 版 本：V3.2
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("VISUALDATA_MAP")]
    [Tenant("mall3s_system")]
    public class MapEntity : CLDEntityBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 地图数据
        /// </summary>
        [SugarColumn(ColumnName = "F_DATA")]
        public string Data { get; set; }

        /// <summary>
        /// 描述或说明
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
    }
}
