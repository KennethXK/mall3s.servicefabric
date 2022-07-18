using Mall3s.Common.Enum;
using Mall3s.Common.Util;
using Mall3s.Dependency;
using Mall3s.DynamicApiController;
using Mall3s.FriendlyException;
using Mall3s.JsonSerialization;
using Mall3s.System.Entitys.Dto.System.DictionaryData;
using Mall3s.System.Entitys.System;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service.Impl
{
    public class DictionaryDataCoreService : IDictionaryDataCoreService
    {
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;
        private readonly IDictionaryTypeCoreService _dictionaryTypeService;
        private readonly IFileCoreService _fileService;
        private readonly SqlSugarScope db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionaryDataRepository"></param>
        /// <param name="dictionaryTypeService"></param>
        /// <param name="fileService"></param>
        public DictionaryDataCoreService(ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository, IDictionaryTypeCoreService dictionaryTypeService, IFileCoreService fileService)
        {
            _dictionaryDataRepository = dictionaryDataRepository;
            _dictionaryTypeService = dictionaryTypeService;
            _fileService = fileService;
            db = dictionaryDataRepository.Context;
        }

        #region PulicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="dictionaryTypeId">类别主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<DictionaryDataEntity>> GetList(string dictionaryTypeId)
        {
            var entity = await _dictionaryTypeService.GetInfo(dictionaryTypeId);
            return await _dictionaryDataRepository.Entities.Where(d => d.DictionaryTypeId == entity.Id && d.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<DictionaryDataEntity>> GetList()
        {
            return await _dictionaryDataRepository.Entities.Where(d => d.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<DictionaryDataEntity> GetInfo(string id)
        {
            return await _dictionaryDataRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(DictionaryDataEntity entity)
        {
            return await _dictionaryDataRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(DictionaryDataEntity entity)
        {
            return await _dictionaryDataRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(DictionaryDataEntity entity)
        {
            return await _dictionaryDataRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        private async Task ImportData(DictionaryDataExportInput inputList)
        {
            try
            {
                #region 剔除重复数据
                var typeList = inputList.list.FindAll(x => !_dictionaryTypeService.IsExistType(x));
                var dataList = inputList.modelList.FindAll(x => !_dictionaryTypeService.IsExistData(x));
                #endregion

                #region 插入数据
                db.BeginTran();
                var typeStor = db.Storageable(typeList).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                typeStor.AsInsertable.ExecuteCommand(); //执行插入
                typeStor.AsUpdateable.ExecuteCommand(); //执行更新　
                var dataStor = db.Storageable(dataList).Saveable().ToStorage();//存在更新不存在插入 根据主键
                dataStor.AsInsertable.ExecuteCommand(); //执行插入
                dataStor.AsUpdateable.ExecuteCommand(); //执行更新　
                db.CommitTran();
                #endregion
            }
            catch (Exception ex)
            {

                db.RollbackTran();
            }
        }
        #endregion
    }
}
