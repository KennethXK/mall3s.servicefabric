using Mall3s.System.Entitys.Entity.System;
using Mall3s.System.Entitys.System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service
{
    public interface IDictionaryTypeCoreService
    {
        #region PublicMethod
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">请求参数</param>
        /// <returns></returns>
        Task<DictionaryTypeEntity> GetInfo(string id);
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<DictionaryTypeEntity>> GetList();
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">请求参数</param>
        /// <returns></returns>
        Task<int> Create(DictionaryTypeEntity entity);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete(DictionaryTypeEntity entity);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Update(DictionaryTypeEntity entity);

        /// <summary>
        /// 递归获取所有分类
        /// </summary>
        /// <param name="id"></param>
        /// <param name="typeList"></param>
        /// <returns></returns>
        Task GetListAllById(string id, List<DictionaryTypeEntity> typeList);

        /// <summary>
        /// 重复判断(分类)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsExistType(DictionaryTypeEntity entity);

        /// <summary>
        /// 重复判断(字典)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsExistData(DictionaryDataEntity entity);

        /// <summary>
        /// 是否存在上级
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        bool IsExistParent(List<DictionaryTypeEntity> entities);
        #endregion

    }
}
