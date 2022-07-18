using Mall3s.Common.Filter;
using Mall3s.VisualDev.Entitys;
using Mall3s.VisualDev.Entitys.Dto.VisualDevModelData;
using Mall3s.VisualDev.Entitys.Entity;
using Mall3s.VisualDev.Entitys.Model.VisualDevModelData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.VisualDev.Service
{
    /// <summary>
    /// 在线开发运行服务抽象
    /// </summary>
    public interface IRunCoreService
    {
        /// <summary>
        /// 模板数据转换
        /// </summary>
        /// <param name="fieldsModelList"></param>
        /// <returns></returns>
        List<FieldsModel> TemplateDataConversion(List<FieldsModel> fieldsModelList);

        /// <summary>
        /// 创建在线开发有表SQL
        /// </summary>
        /// <param name="templateEntity"></param>
        /// <param name="dataInput"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        Task<string> CreateHaveTableSql(VisualDevEntity templateEntity, VisualDevModelDataCrInput dataInput, string mainId);

        /// <summary>
        /// 修改在线开发有表sql
        /// </summary>
        /// <param name="templateEntity"></param>
        /// <param name="dataInput"></param>
        /// <param name="mainId"></param>
        /// <returns></returns>
        Task<string> UpdateHaveTableSql(VisualDevEntity templateEntity, VisualDevModelDataUpInput dataInput, string mainId);

        /// <summary>
        /// 列表数据处理
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="input"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        Task<PageResult<Dictionary<string, object>>> GetListResult(VisualDevEntity entity, VisualDevModelListQueryInput input, string actionType = "List");

        /// <summary>
        /// 获取模型数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<VisualDevModelDataEntity> GetInfo(string id);

        /// <summary>
        /// 获取无表详情转换
        /// </summary>
        /// <param name="entity">模板实体</param>
        /// <param name="data">真实数据</param>
        /// <returns></returns>
        Task<string> GetIsNoTableInfo(VisualDevEntity entity, string data);

        /// <summary>
        /// 获取无表信息详情
        /// </summary>
        /// <param name="entity">模板实体</param>
        /// <param name="data">真实数据</param>
        /// <returns></returns>
        Task<string> GetIsNoTableInfoDetails(VisualDevEntity entity, VisualDevModelDataEntity data);

        /// <summary>
        /// 获取有表详情转换
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="templateEntity">模板实体</param>
        /// <returns></returns>
        Task<string> GetHaveTableInfo(string id, VisualDevEntity templateEntity);

        /// <summary>
        /// 获取有表详情转换
        /// </summary>
        /// <param name="id"></param>
        /// <param name="templateEntity"></param>
        /// <returns></returns>
        Task<string> GetHaveTableInfoDetails(string id, VisualDevEntity templateEntity, bool isFlowTask = false);

        /// <summary>
        /// 生成系统自动生成字段
        /// </summary>
        /// <param name="fieldsModelList">模板数据</param>
        /// <param name="allDataMap">真实数据</param>
        /// <param name="IsCreate">创建与修改标识 true创建 false 修改</param>
        /// <returns></returns>
        Task<Dictionary<string, object>> GenerateFeilds(List<FieldsModel> fieldsModelList, Dictionary<string, object> allDataMap, bool IsCreate);

        /// <summary>
        /// 获取模板主键
        /// </summary>
        /// <param name="entity">模板实体</param>
        /// <returns></returns>
        Task<string> GetTablePrimary(VisualDevEntity entity);
    }
}
