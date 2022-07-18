using Mall3s.System.Entitys.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service
{
    public interface IDictionaryDataCoreService
    {
        #region PulicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="dictionaryTypeId">类别主键</param>
        /// <returns></returns>
        
        Task<List<DictionaryDataEntity>> GetList(string dictionaryTypeId);

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        
        Task<List<DictionaryDataEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        
        Task<DictionaryDataEntity> GetInfo(string id);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        
        Task<int> Delete(DictionaryDataEntity entity);
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        
        Task<int> Create(DictionaryDataEntity entity);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        Task<int> Update(DictionaryDataEntity entity);
        #endregion
    }
}
