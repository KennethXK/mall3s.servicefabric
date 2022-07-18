using Mall3s.System.Entitys.Dto.System.DbLink;
using Mall3s.System.Entitys.Entity.System;
using Mall3s.System.Entitys.Permission;
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
    /// 数据连接
    /// 版 本：V3.2
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public class DbLinkCoreService : IDbLinkCoreService
    {
        private readonly ISqlSugarRepository<DbLinkEntity> _dbLinkRepository;
        private readonly IDictionaryDataCoreService _dictionaryDataService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbLinkRepository"></param>
        /// <param name="dictionaryDataService"></param>
        public DbLinkCoreService(ISqlSugarRepository<DbLinkEntity> dbLinkRepository, IDictionaryDataCoreService dictionaryDataService)
        {
            _dbLinkRepository = dbLinkRepository;
            _dictionaryDataService = dictionaryDataService;
        }


        #region PublicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<DbLinkListOutput>> GetList()
        {
            var list = await _dbLinkRepository.Context.Queryable<DbLinkEntity, UserEntity, UserEntity, DictionaryDataEntity, DictionaryTypeEntity>(
                (a, b, c, d, e) => new JoinQueryInfos(
                    JoinType.Left, a.CreatorUserId == b.Id,
                    JoinType.Left, a.LastModifyUserId == c.Id,
                    JoinType.Left, a.DbType == d.EnCode && d.DeleteMark == null,
                    JoinType.Left, d.DictionaryTypeId == e.Id && e.EnCode == "dbType")).
                    Select((a, b, c, d) => new
                    {
                        id = a.Id,
                        parentId = d.Id == null ? "-1" : d.Id,
                        creatorTime = a.CreatorTime,
                        creatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account),
                        dbType = a.DbType,
                        enabledMark = a.EnabledMark,
                        fullName = a.FullName,
                        host = a.Host,
                        lastModifyTime = a.LastModifyTime,
                        lastModifyUser = SqlFunc.MergeString(c.RealName, "/", c.Account),
                        port = a.Port.ToString(),
                        sortCode = a.SortCode,
                        deleteMark = a.DeleteMark
                    }).MergeTable().Where(x => x.deleteMark == null).Distinct().OrderBy(o => o.sortCode).OrderBy(o => o.creatorTime, OrderByType.Desc).Select<DbLinkListOutput>().ToListAsync();
            return list;
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<DbLinkEntity> GetInfo(string id)
        {
            return await _dbLinkRepository.FirstOrDefaultAsync(m => m.Id == id && m.DeleteMark == null);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(DbLinkEntity entity)
        {
            return await _dbLinkRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(DbLinkEntity entity)
        {
            return await _dbLinkRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(DbLinkEntity entity)
        {
            return await _dbLinkRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }
        #endregion

        #region PrivateMethod

        /// <summary>
        /// 测试连接串
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        private bool TestDbConnection(DbLinkEntity entity)
        {
            try
            {
                return App.GetService<IDataBaseCoreService>().IsConnection(entity);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
