using Mall3s.Common.Enum;
using Mall3s.Common.Util;
using Mall3s.Dependency;
using Mall3s.DynamicApiController;
using Mall3s.FriendlyException;
using Mall3s.System.Entitys.Dto.System.DictionaryType;
using Mall3s.System.Entitys.Entity.System;
using Mall3s.System.Entitys.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service.Impl
{
    public class DictionaryTypeCoreService : IDictionaryTypeCoreService
    {
        private readonly ISqlSugarRepository<DictionaryTypeEntity> _dictionaryTypeRepository;
        private readonly ISqlSugarRepository<DictionaryDataEntity> _dictionaryDataRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionaryTypeRepository"></param>
        /// <param name="dictionaryDataRepository"></param>
        public DictionaryTypeCoreService(ISqlSugarRepository<DictionaryTypeEntity> dictionaryTypeRepository, ISqlSugarRepository<DictionaryDataEntity> dictionaryDataRepository)
        {
            _dictionaryTypeRepository = dictionaryTypeRepository;
            _dictionaryDataRepository = dictionaryDataRepository;
        }

        #region PublicMethod
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">请求参数</param>
        /// <returns></returns>
        [NonAction]
        public async Task<DictionaryTypeEntity> GetInfo(string id)
        {
            var data = await _dictionaryTypeRepository.FirstOrDefaultAsync(x => (x.Id == id || x.EnCode == id) && x.DeleteMark == null);
            return data;
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<DictionaryTypeEntity>> GetList()
        {
            return await _dictionaryTypeRepository.Entities.Where(x => x.DeleteMark == null).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToListAsync();
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">请求参数</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(DictionaryTypeEntity entity)
        {
            return await _dictionaryTypeRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(DictionaryTypeEntity entity)
        {
            return await _dictionaryTypeRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(DictionaryTypeEntity entity)
        {
            return await _dictionaryTypeRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 递归获取所有分类
        /// </summary>
        /// <param name="id"></param>
        /// <param name="typeList"></param>
        /// <returns></returns>
        [NonAction]
        public async Task GetListAllById(string id, List<DictionaryTypeEntity> typeList)
        {
            var entity = await GetInfo(id);
            if (entity != null)
            {
                typeList.Add(entity);
                if (await _dictionaryTypeRepository.AnyAsync(x => x.ParentId == entity.Id && x.DeleteMark == null))
                {
                    var list = await _dictionaryTypeRepository.Entities.Where(x => x.ParentId == entity.Id && x.DeleteMark == null).ToListAsync();
                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            await GetListAllById(item.Id, typeList);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 重复判断(分类)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public bool IsExistType(DictionaryTypeEntity entity)
        {
            return _dictionaryTypeRepository.Any(x => (x.Id == entity.Id || x.EnCode == entity.EnCode || x.FullName == entity.FullName) && x.DeleteMark == null);
        }

        /// <summary>
        /// 重复判断(字典)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public bool IsExistData(DictionaryDataEntity entity)
        {
            //var typeFlag = _dictionaryTypeRepository.Any(x => x.Id == entity.DictionaryTypeId && x.DeleteMark == null);
            var dataFalg = _dictionaryDataRepository.Any(x => (x.EnCode == entity.EnCode || x.FullName == entity.FullName) && x.DictionaryTypeId == entity.DictionaryTypeId && x.DeleteMark == null);
            return dataFalg;
        }

        /// <summary>
        /// 是否存在上级
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public bool IsExistParent(List<DictionaryTypeEntity> entities)
        {
            foreach (var item in entities)
            {
                if (_dictionaryTypeRepository.Any(x => x.Id == item.ParentId && x.DeleteMark == null))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 是否可以删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<bool> AllowDelete(string id)
        {
            var flag = true;
            if (await _dictionaryTypeRepository.AnyAsync(o => o.ParentId.Equals(id) && o.DeleteMark == null))
                return false;
            if (await _dictionaryDataRepository.AnyAsync(p => p.DictionaryTypeId.Equals(id) && p.DeleteMark == null))
                return false;
            return flag;
        }
        #endregion
    }
}
