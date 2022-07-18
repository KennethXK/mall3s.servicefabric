using Mall3s.Core.System.Manager;
using Mall3s.Core.System.Service;
using Mall3s.Core.WorkFlow.Repositories;
using Mall3s.WorkFlow.Entitys;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.WorkFlow.Service.Impl
{
    /// <summary>
    /// 流程引擎
    /// 版 本：V3.2
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public class FlowEngineCoreService : IFlowEngineCoreService
    {
        private readonly ISqlSugarRepository<FlowEngineEntity> _flowEngineRepository;
        private readonly ISqlSugarRepository<FlowEngineVisibleEntity> _flowEngineVisibleRepository;
        private readonly IUserCoreManager _userManager;
        private readonly SqlSugarScope db;// 核心对象：拥有完整的SqlSugar全部功能
        private readonly IFlowTaskCoreRepository _flowTaskRepository;
        private readonly IDictionaryDataCoreService _dictionaryDataService;
        private readonly IFileCoreService _fileService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowEngineRepository"></param>
        /// <param name="flowEngineVisibleRepository"></param>
        /// <param name="userManager"></param>
        /// <param name="flowTaskRepository"></param>
        /// <param name="dictionaryDataService"></param>
        /// <param name="fileService"></param>
        public FlowEngineCoreService(ISqlSugarRepository<FlowEngineEntity> flowEngineRepository, 
            ISqlSugarRepository<FlowEngineVisibleEntity> flowEngineVisibleRepository,
            IUserCoreManager userManager,
            IFlowTaskCoreRepository flowTaskRepository,
            IDictionaryDataCoreService dictionaryDataService,
            IFileCoreService fileService)
        {
            _flowEngineRepository = flowEngineRepository;
            _flowEngineVisibleRepository = flowEngineVisibleRepository;
            _userManager = userManager;
            db = flowEngineRepository.Context;
            _flowTaskRepository = flowTaskRepository;
            _dictionaryDataService = dictionaryDataService;
            _fileService = fileService;
        }

        #region PublicMethod
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="visibleList"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowEngineEntity> Create(FlowEngineEntity entity, List<FlowEngineVisibleEntity> visibleList)
        {
            try
            {
                db.BeginTran();
                entity.VisibleType = visibleList.Count == 0 ? 0 : 1;
                var result = await _flowEngineRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
                if (result == null)
                    throw new Exception();
                foreach (var item in visibleList)
                {
                    item.FlowId = entity.Id;
                    item.SortCode = visibleList.IndexOf(item);
                }
                if (visibleList.Count > 0)
                    await _flowEngineVisibleRepository.Context.Insertable(visibleList).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                db.CommitTran();
                return result;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return null;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(FlowEngineEntity entity)
        {
            try
            {
                db.BeginTran();
                await _flowEngineVisibleRepository.DeleteAsync(a => a.FlowId == entity.Id);
                var isOk = await _flowEngineRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
                db.CommitTran();
                return isOk;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowEngineEntity> GetInfo(string id)
        {
            return await _flowEngineRepository.FirstOrDefaultAsync(a => a.Id == id && a.DeleteMark == null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowEngineEntity> GetInfoByEnCode(string enCode)
        {
            return await _flowEngineRepository.FirstOrDefaultAsync(a => a.EnCode == enCode && a.EnabledMark == 1 && a.DeleteMark == null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<FlowEngineEntity>> GetList()
        {
            return await _flowEngineRepository.Entities.Where(a => a.DeleteMark == null).OrderBy(a => a.SortCode).OrderBy(o => o.CreatorTime).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<FlowEngineVisibleEntity>> GetVisibleFlowList(string userId)
        {
            return await db.Queryable<FlowEngineVisibleEntity, UserRelationEntity>((a, b) => new JoinQueryInfos(JoinType.Left, a.OperatorId == b.ObjectId)).Select((a, b) => new { Id = a.Id, FlowId = a.FlowId, OperatorType = a.OperatorType, OperatorId = a.OperatorId, SortCode = a.SortCode, CreatorTime = a.CreatorTime, CreatorUserId = a.CreatorUserId, UserId = b.UserId }).MergeTable().Where(a => a.OperatorId == _userManager.UserId || a.UserId == _userManager.UserId).Select<FlowEngineVisibleEntity>().ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="visibleList"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(string id, FlowEngineEntity entity, List<FlowEngineVisibleEntity> visibleList)
        {
            try
            {
                db.BeginTran();
                entity.VisibleType = visibleList.Count == 0 ? 0 : 1;
                await _flowEngineVisibleRepository.DeleteAsync(a => a.FlowId == entity.Id);
                foreach (var item in visibleList)
                {
                    item.FlowId = entity.Id;
                    item.SortCode = visibleList.IndexOf(item);
                }
                if (visibleList.Count > 0)
                    await _flowEngineVisibleRepository.Context.Insertable(visibleList).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                var isOk = await _flowEngineRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                db.CommitTran();
                return isOk;
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(string id, FlowEngineEntity entity)
        {
            return await _flowEngineRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<FlowEngineListOutput>> GetFlowFormList()
        {
            var list = (await GetOutList()).FindAll(x => x.enabledMark == 1 && x.type == 0);
            if (_userManager.User.IsAdministrator == 0)
            {
                var data = new List<FlowEngineListOutput>();
                //部分看见
                var flowVisibleData = await GetVisibleFlowList(_userManager.UserId);
                //去重
                var ids = new List<string>();
                foreach (var item in flowVisibleData)
                {
                    FlowEngineListOutput flowEngineEntity = list.Find(m => m.id == item.FlowId);
                    if (flowEngineEntity != null && !ids.Contains(flowEngineEntity.id))
                    {
                        data.Add(flowEngineEntity);
                        ids.Add(flowEngineEntity.id);
                    }
                }
                ////全部看见
                foreach (FlowEngineListOutput flowEngineEntity in list.FindAll(m => m.visibleType == 0))
                {
                    data.Add(flowEngineEntity);
                }
                return data;
            }
            else
            {
                return list;
            }
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 解析流程可见参数
        /// </summary>
        /// <param name="josnStr"></param>
        /// <returns></returns>
        private List<FlowEngineVisibleEntity> GetFlowEngineVisibleList(string josnStr)
        {
            var output = new List<FlowEngineVisibleEntity>();
            var jobj = JSON.Deserialize<FlowTemplateJsonModel>(josnStr).properties;
            var initiator = jobj["initiator"] as JArray;
            var initiatePos = jobj["initiatePos"] as JArray;
            var initiateRole = jobj["initiateRole"] as JArray;
            if (initiator != null && initiator.Count != 0)
            {
                foreach (var item in initiator)
                {
                    var entity = new FlowEngineVisibleEntity();
                    entity.OperatorId = item.ToString();
                    entity.OperatorType = "user";
                    output.Add(entity);
                }
            }
            if (initiatePos != null && initiatePos.Count != 0)
            {
                foreach (var item in initiatePos)
                {
                    var entity = new FlowEngineVisibleEntity();
                    entity.OperatorId = item.ToString();
                    entity.OperatorType = "Position";
                    output.Add(entity);
                }
            }
            if (initiateRole != null && initiateRole.Count != 0)
            {
                foreach (var item in initiateRole)
                {
                    var entity = new FlowEngineVisibleEntity();
                    entity.OperatorId = item.ToString();
                    entity.OperatorType = "Role";
                    output.Add(entity);
                }
            }
            return output;
        }

        /// <summary>
        /// 流程列表(功能流程不显示)
        /// </summary>
        /// <param name="type">0:流程设计，1：下拉列表</param>
        /// <returns></returns>
        private async Task<List<FlowEngineListOutput>> GetOutList(int type = 0)
        {
            return await db.Queryable<FlowEngineEntity, UserEntity, UserEntity, DictionaryDataEntity>((a, b, c, d) => new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId, JoinType.Left, c.Id == a.LastModifyUserId, JoinType.Left, a.Category == d.EnCode)).Select((a, b, c, d) => new
            {
                category = a.Category,
                id = a.Id,
                description = a.Description,
                creatorTime = a.CreatorTime,
                creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                enCode = a.EnCode,
                enabledMark = a.EnabledMark,
                flowTemplateJson = a.FlowTemplateJson,
                formData = a.FormTemplateJson,
                fullName = a.FullName,
                formType = a.FormType,
                icon = a.Icon,
                iconBackground = a.IconBackground,
                lastModifyTime = a.LastModifyTime,
                lastModifyUser = SqlFunc.MergeString(c.RealName, "/", c.Account),
                sortCode = a.SortCode,
                type = a.Type,
                visibleType = a.VisibleType,
                deleteMark = a.DeleteMark,
                parentId = d.Id,
                d.DictionaryTypeId
            }).MergeTable().Where(x => x.deleteMark == null && x.DictionaryTypeId == "507f4f5df86b47588138f321e0b0dac7")
             .Where(x => !(x.formType == 2 && x.type == 1)).WhereIF(type != 0, x => x.type != 1).Select<FlowEngineListOutput>().OrderBy(x => x.sortCode).OrderBy(x => x.creatorTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task ImportData(FlowEngineImportModel model)
        {
            try
            {
                db.BeginTran();
                var stor = db.Storageable(model.flowEngine).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stor.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stor.AsUpdateable.ExecuteCommandAsync(); //执行更新　
                var stor1 = db.Storageable(model.visibleList).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stor1.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stor1.AsUpdateable.ExecuteCommandAsync(); //执行更新
                db.CommitTran();
            }
            catch (Exception ex)
            {
                db.RollbackTran();
                throw Mall3sException.Oh(ErrorCode.D3006);
            }
        }
        #endregion
    }
}
