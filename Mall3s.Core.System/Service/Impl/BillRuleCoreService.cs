using Mall3s.Common.Const;
using Mall3s.Common.Enum;
using Mall3s.Core.System.Manager;
using Mall3s.FriendlyException;
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
    /// 单据规则
    /// 版 本：V3.2
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public class BillRuleCoreService : IBillRullCoreService
    {
        private readonly ISqlSugarRepository<BillRuleEntity> _billRuleRepository;
        private readonly ISysCacheCoreService _sysCacheService;
        private readonly IUserCoreManager _userManager; // 用户管理

        /// <summary>
        /// 
        /// </summary>
        /// <param name="billRuleRepository"></param>
        /// <param name="sysCacheService"></param>
        /// <param name="userManager"></param>
        /// <param name="fileService"></param>
        public BillRuleCoreService(ISqlSugarRepository<BillRuleEntity> billRuleRepository,
            ISysCacheCoreService sysCacheService,
            IUserCoreManager userManager)
        {
            _billRuleRepository = billRuleRepository;
            _sysCacheService = sysCacheService;
            _userManager = userManager;
        }

        #region PublicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<BillRuleEntity>> GetList()
        {
            return await _billRuleRepository.Entities.Where(x => x.DeleteMark == null && x.EnabledMark == 1).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<BillRuleEntity> GetInfo(string id)
        {
            return await _billRuleRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(BillRuleEntity entity)
        {
            return await _billRuleRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(BillRuleEntity entity)
        {
            return await _billRuleRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(BillRuleEntity entity)
        {
            return await _billRuleRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取单据流水号
        /// </summary>
        /// <param name="enCode"></param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetBillNumber(string enCode, bool isCache = false)
        {
            try
            {
                var strNumber = "";
                if (isCache == true)
                {
                    var cacheKey = CommonConst.CACHE_KEY_BILLRULE + _userManager.TenantId + "_" + _userManager.UserId + enCode;
                    if (!_sysCacheService.Exists(cacheKey))
                    {
                        strNumber = await GetNumber(enCode);
                        _sysCacheService.Set(cacheKey, strNumber, new TimeSpan(0, 3, 0));
                    }
                    else
                    {
                        strNumber = _sysCacheService.Get(cacheKey);
                    }
                }
                else
                {
                    strNumber = await GetNumber(enCode);
                }
                return strNumber;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 使用单据流水号（注意：必须是缓存的单据才可以调用这个方法，否则无效）
        /// </summary>
        /// <param name="enCode">流水编码</param>
        [NonAction]
        public void UseBillNumber(string enCode)
        {
            var cacheKey = CommonConst.CACHE_KEY_BILLRULE + _userManager.TenantId + "_" + _userManager.UserId + enCode;
            _sysCacheService.Del(cacheKey);
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 获取流水号
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        private async Task<string> GetNumber(string enCode)
        {
            StringBuilder strNumber = new StringBuilder();
            var entity = await _billRuleRepository.FirstOrDefaultAsync(m => m.EnCode == enCode && m.DeleteMark == null);
            if (entity != null)
            {
                //处理隔天流水号归0
                if (entity.OutputNumber != null)
                {
                    var serialDate = entity.OutputNumber.Remove(entity.OutputNumber.Length - (int)entity.Digit).Replace(entity.Prefix, "");
                    var thisDate = entity.DateFormat == "no" ? "" : DateTime.Now.ToString(entity.DateFormat);
                    if (serialDate != thisDate)
                    {
                        entity.ThisNumber = 0;
                    }
                    entity.ThisNumber = entity.ThisNumber + 1;
                }
                else
                {
                    entity.ThisNumber = 1;
                }
                //拼接单据编码
                strNumber.Append(entity.Prefix);                                                                  //前缀
                if (entity.DateFormat != "no")
                {
                    strNumber.Append(DateTime.Now.ToString(entity.DateFormat));                                  //日期格式
                }
                var number = int.Parse(entity.StartNumber) + entity.ThisNumber;
                strNumber.Append(number.ToString().PadLeft((int)entity.Digit, '0'));              //流水号

                //更新流水号
                entity.OutputNumber = strNumber.ToString();
                await Update(entity);
            }
            else
            {
                strNumber.Append("单据规则不存在");
            }
            return strNumber.ToString();
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task ImportData(BillRuleEntity data)
        {
            try
            {
                _billRuleRepository.Context.BeginTran();
                var stor = _billRuleRepository.Context.Storageable(data).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stor.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stor.AsUpdateable.ExecuteCommandAsync(); //执行更新　
                _billRuleRepository.Context.CommitTran();
            }
            catch (Exception ex)
            {
                _billRuleRepository.Context.RollbackTran();
                throw Mall3sException.Oh(ErrorCode.D3006);
            }
        }
        #endregion


    }
}
