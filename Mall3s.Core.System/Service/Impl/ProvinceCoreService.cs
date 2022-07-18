using Mall3s.System.Entitys.System;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service.Impl
{
    /// <summary>
        /// 行政区划
        /// 版 本：V3.2
        /// 版 权：mall3s开发
        /// 作 者：Mall3s开发平台组
        /// 日 期：2021-06-01 
        /// </summary>
    public class ProvinceCoreService : IProvinceCoreService
    {
        private readonly ISqlSugarRepository<ProvinceEntity> _provinceRepository;

        /// <summary>
        /// 初始化一个<see cref="ProvinceService"/>类型的新实例
        /// </summary>
        /// <param name="provinceRepository"></param>
        public ProvinceCoreService(ISqlSugarRepository<ProvinceEntity> provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }

        #region PulicMethod

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ProvinceEntity>> GetList()
        {
            var data = await _provinceRepository.Entities.OrderBy(o => o.SortCode).ToListAsync();
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<ProvinceEntity>> GetList(string id)
        {
            return await _provinceRepository.Entities.Where(m => m.ParentId == id && m.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 是否存在子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<bool> GetExistsLeaf(string id)
        {
            return (await _provinceRepository.Where(m => m.ParentId == id && m.DeleteMark == null).CountAsync()) > 0 ? false : true;
        }



        #endregion
    }
}
