using Mall3s.Core.System.Service;
using Mall3s.VisualDev.Entitys;
using Mall3s.WorkFlow.Entitys;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.VisualDev.Service.Impl
{
    /// <summary>
    /// 可视化开发基础
    /// </summary>
    public class VisualDevCoreService : IVisualDevCoreService
    {
        private readonly ISqlSugarRepository<VisualDevEntity> _visualDevRepository;
        private readonly IDictionaryDataCoreService _dictionaryDataService;

        /// <summary>
        /// 初始化一个<see cref="VisualDevService"/>类型的新实例
        /// </summary>
        public VisualDevCoreService(ISqlSugarRepository<VisualDevEntity> visualDevRepository, IDictionaryDataCoreService dictionaryDataService)
        {
            _visualDevRepository = visualDevRepository;
            _dictionaryDataService = dictionaryDataService;
        }

        #region PublicMethod

        /// <summary>
        /// 获取功能信息
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<VisualDevEntity> GetInfoById(string id)
        {
            return await _visualDevRepository.FirstOrDefaultAsync(v => v.Id == id && v.DeleteMark == null);
        }

        /// <summary>
        /// 判断功能ID是否存在
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> GetDataExists(string id)
        {
            return await _visualDevRepository.AnyAsync(it => it.Id == id && it.DeleteMark == null);
        }

        /// <summary>
        /// 判断是否存在编码、名称相同的数据
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> GetDataExists(string enCode, string fullName)
        {
            return await _visualDevRepository.AnyAsync(it => it.EnCode == enCode && it.FullName == fullName && it.DeleteMark == null);
        }

        /// <summary>
        /// 新增导入数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [NonAction]
        public async Task CreateImportData(VisualDevEntity input)
        {
            try
            {
                //开启事务
                _visualDevRepository.Ado.BeginTran();

                if (input.WebType == 3)
                {
                    var categoryData = await _dictionaryDataService.GetInfo(input.Category);
                    var flowEngine = new FlowEngineEntity();
                    flowEngine.FlowTemplateJson = input.FlowTemplateJson;
                    flowEngine.EnCode = "#visualDev" + input.EnCode;
                    flowEngine.Type = 1;
                    flowEngine.FormType = 2;
                    flowEngine.FullName = input.FullName;
                    flowEngine.Category = categoryData.EnCode;
                    flowEngine.VisibleType = 0;
                    flowEngine.Icon = "icon-ym icon-ym-node";
                    flowEngine.IconBackground = "#008cff";
                    flowEngine.Tables = input.Tables;
                    flowEngine.DbLinkId = input.DbLinkId;
                    flowEngine.FormTemplateJson = input.FormData;
                    //添加流程引擎
                    var engineEntity = await _visualDevRepository.Context.Insertable<FlowEngineEntity>(flowEngine).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
                    input.FlowId = engineEntity.Id;
                    input.Id = engineEntity.Id;
                }

                var visualDev = await _visualDevRepository.Context.Insertable(input).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();

                //关闭事务
                _visualDevRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _visualDevRepository.Ado.RollbackTran();
                throw;
            }
        }

        #endregion
    }
}
