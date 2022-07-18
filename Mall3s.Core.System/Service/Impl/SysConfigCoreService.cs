using Mall3s.JsonSerialization;
using Mall3s.System.Entitys.Dto.System.SysConfig;
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
    public class SysConfigCoreService : ISysConfigCoreService
    {
        private readonly ISqlSugarRepository<SysConfigEntity> _sysConfigRepository;

        /// <summary>
        /// 初始化一个<see cref="SysConfigService"/>类型的新实例
        /// </summary>
        /// <param name="sysConfigRepository"></param>
        public SysConfigCoreService(ISqlSugarRepository<SysConfigEntity> sysConfigRepository)
        {
            _sysConfigRepository = sysConfigRepository;
        }

        #region PublicMethod

        /// <summary>
        /// 系统配置信息
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<SysConfigEntity> GetInfo(string category, string key)
        {
            return await _sysConfigRepository.FirstOrDefaultAsync(s => s.Category == category && s.Key == key);
        }

        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<SysConfigOutput> GetInfo()
        {
            var array = new Dictionary<string, string>();
            var data = await _sysConfigRepository.Entities.Where(x => x.Category.Equals("SysConfig")).ToListAsync();
            foreach (var item in data)
            {
                array.Add(item.Key, item.Value);
            }
            var output = array.Serialize().Deserialize<SysConfigOutput>();
            return output;
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private async Task Save(List<SysConfigEntity> entitys, string category)
        {
            try
            {
                _sysConfigRepository.Context.Ado.BeginTran();
                await _sysConfigRepository.DeleteAsync(x => x.Category.Equals(category));
                await _sysConfigRepository.InsertAsync(entitys);
                _sysConfigRepository.Context.Ado.CommitTran();
            }
            catch (Exception)
            {
                _sysConfigRepository.Context.Ado.RollbackTran();
            }
        }

        #endregion
    }
}
