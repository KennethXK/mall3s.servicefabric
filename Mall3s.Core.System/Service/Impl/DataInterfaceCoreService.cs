using Mall3s.Common.Enum;
using Mall3s.Core.System.Manager;
using Mall3s.FriendlyException;
using Mall3s.JsonSerialization;
using Mall3s.RemoteRequest.Extensions;
using Mall3s.System.Entitys.Model.System.DataInterFace;
using Mall3s.System.Entitys.System;
using Mall3s.UnifyResult;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service.Impl
{

    /// <summary>
    /// 数据接口
    /// 版 本：V3.2
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public class DataInterfaceCoreService : IDataInterfaceCoreService
    {
        private readonly ISqlSugarRepository<DataInterfaceEntity> _dataInterfaceRepository;
        private readonly IDictionaryDataCoreService _dictionaryDataService;
        private readonly IDbLinkCoreService _dbLinkService;
        private readonly IDataBaseCoreService _dataBaseService;
        private readonly IUserCoreManager _userManager;
        private readonly IFileCoreService _fileService;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataInterfaceRepository"></param>
        /// <param name="dictionaryDataService"></param>
        /// <param name="dbLinkService"></param>
        /// <param name="dataBaseService"></param>
        /// <param name="userManager"></param>
        /// <param name="fileService"></param>
        public DataInterfaceCoreService(ISqlSugarRepository<DataInterfaceEntity> dataInterfaceRepository,
            IDictionaryDataCoreService dictionaryDataService,
            IDbLinkCoreService dbLinkService,
            IDataBaseCoreService dataBaseService,
            IUserCoreManager userManager,
            IFileCoreService fileService)
        {
            _dataInterfaceRepository = dataInterfaceRepository;
            _dictionaryDataService = dictionaryDataService;
            _dbLinkService = dbLinkService;
            _dataBaseService = dataBaseService;
            _userManager = userManager;
            _fileService = fileService;
        }


        #region PublicMethod
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<DataInterfaceEntity>> GetList()
        {
            return await _dataInterfaceRepository.Entities.Where(x => x.DeleteMark == null).OrderBy(x => x.SortCode).ToListAsync();
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        [NonAction]
        public async Task<DataInterfaceEntity> GetInfo(string id)
        {
            return await _dataInterfaceRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(DataInterfaceEntity entity)
        {
            return await _dataInterfaceRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(DataInterfaceEntity entity)
        {
            return await _dataInterfaceRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(DataInterfaceEntity entity)
        {
            return await _dataInterfaceRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<DataTable> GetData(DataInterfaceEntity entity)
        {
            var result = await connection(entity.DBLinkId, entity.Query, entity.CheckType);
            return result;
        }

        /// <summary>
        /// 查询(工作流)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<DataTable> GetData(string id)
        {
            var data = await _dataInterfaceRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
            var result = await connection(data.DBLinkId, data.Query, data.CheckType);
            return result;
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 通过连接执行sql
        /// </summary>
        /// <param name="dbLinkId"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        private async Task<DataTable> connection(string dbLinkId, string sql, int? checkType)
        {
            var linkEntity = await _dbLinkService.GetInfo(dbLinkId);
            var parameter = new List<SugarParameter>();
            if (checkType != 0 && _userManager.ToKen != null)
            {
                parameter.Add(new SugarParameter("@user", _userManager.UserId));
                parameter.Add(new SugarParameter("@organize", _userManager.User.OrganizeId));
                parameter.Add(new SugarParameter("@department", _userManager.User.OrganizeId));
                parameter.Add(new SugarParameter("@postion", _userManager.User.PositionId));
            }
            var dt = _dataBaseService.GetInterFaceData(linkEntity, sql, parameter.ToArray());
            return dt;
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task ImportData(DataInterfaceEntity data)
        {
            try
            {
                _dataInterfaceRepository.Context.BeginTran();
                var stor = _dataInterfaceRepository.Context.Storageable(data).Saveable().ToStorage(); //存在更新不存在插入 根据主键
                await stor.AsInsertable.ExecuteCommandAsync(); //执行插入
                await stor.AsUpdateable.ExecuteCommandAsync(); //执行更新　
                _dataInterfaceRepository.Context.CommitTran();
            }
            catch (Exception ex)
            {
                _dataInterfaceRepository.Context.RollbackTran();
                throw Mall3sException.Oh(ErrorCode.D3006);
            }
        }

        /// <summary>
        /// 根据不同规则请求接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task<object> GetApiDataByType(DataInterfaceEntity entity)
        {
            var parameters = JSON.Deserialize<List<DataInterfaceReqParameter>>(entity.RequestParameters);
            var dic = new Dictionary<string, object>();
            foreach (var item in parameters)
            {
                dic.Add(item.field, item.value);
            }

            var result = await entity.Path.SetHeaders(new { Authorization = _userManager.ToKen }).SetQueries(dic).GetAsStringAsync();
            return JSON.Deserialize<RESTfulResult<object>>(result).data;

            //switch (entity.CheckType)
            //{
            //    case 1:
            //        var result2 = await entity.Path.SetHeaders(new { Authorization = _userManager.ToKen }).SetQueries(dic).GetAsStringAsync();
            //        return JSON.Deserialize<RESTfulResult<object>>(result2).data;
            //    case 2:
            //        var ipList = entity.RequestHeaders.Split(",").ToList();
            //        if (ipList.Contains(App.HttpContext.GetLocalIpAddressToIPv4()))
            //            throw Mall3sException.Oh(ErrorCode.D9002);
            //        var result3 = await entity.Path.SetQueries(dic).GetAsStringAsync();
            //        return JSON.Deserialize<RESTfulResult<object>>(result3).data;
            //    default:
            //        var result1 = await entity.Path.SetQueries(dic).GetAsStringAsync();
            //        return JSON.Deserialize<RESTfulResult<object>>(result1).data;
            //}
        }
        #endregion
    }
}
