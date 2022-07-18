using Mall3s.Common.Extension;
using Mall3s.System.Entitys.Model.Permission.User;
using Mall3s.System.Entitys.Permission;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service.Impl
{
    public class OrganizeAdministratorCoreService
    {

        private readonly ISqlSugarRepository<OrganizeAdministratorEntity> _organizeAdministratorRepository;
        private readonly IOrganizeCoreService _organizeService;

        /// <summary>
        /// 初始化一个<see cref="OrganizeAdministratorService"/>类型的新实例
        /// </summary>
        public OrganizeAdministratorCoreService(ISqlSugarRepository<OrganizeAdministratorEntity> organizeAdministratorRepository, IOrganizeCoreService organizeService)
        {
            _organizeAdministratorRepository = organizeAdministratorRepository;
            _organizeService = organizeService;
        }

        #region PublicMethod

        /// <summary>
        /// 获取用户数据范围
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<UserDataScope>> GetUserDataScope(string userId)
        {
            List<UserDataScope> data = new List<UserDataScope>();
            List<UserDataScope> subData = new List<UserDataScope>();
            List<UserDataScope> inteList = new List<UserDataScope>();
            var list = await _organizeAdministratorRepository.Where(it => SqlFunc.ToString(it.UserId) == userId && it.DeleteMark == null).ToListAsync();
            //填充数据
            foreach (var item in list)
            {
                if (item.SubLayerAdd.ToBool() || item.SubLayerEdit.ToBool() || item.SubLayerDelete.ToBool())
                {
                    var subsidiary = await _organizeService.GetSubsidiary(item.OrganizeId);
                    subsidiary.Remove(item.OrganizeId);
                    subsidiary.ForEach(it =>
                    {
                        subData.Add(new UserDataScope()
                        {
                            organizeId = it,
                            Add = item.SubLayerAdd.ToBool(),
                            Edit = item.SubLayerEdit.ToBool(),
                            Delete = item.SubLayerDelete.ToBool()
                        });
                    });
                }
                if (item.ThisLayerAdd.ToBool() || item.ThisLayerEdit.ToBool() || item.ThisLayerDelete.ToBool())
                {
                    data.Add(new UserDataScope()
                    {
                        organizeId = item.OrganizeId,
                        Add = item.ThisLayerAdd.ToBool(),
                        Edit = item.ThisLayerEdit.ToBool(),
                        Delete = item.ThisLayerDelete.ToBool()
                    });
                }
            }
            //比较数据
            //所有分级数据权限以本级权限为主 子级为辅
            //将本级数据与子级数据对比 对比出子级数据内组织ID存在本级数据的组织ID
            var intersection = data.Select(it => it.organizeId).Intersect(subData.Select(it => it.organizeId)).ToList();
            intersection.ForEach(it =>
            {
                var parent = data.Find(item => item.organizeId == it);
                var child = subData.Find(item => item.organizeId == it);
                var add = false;
                var edit = false;
                var delete = false;
                if (parent.Add || child.Add)
                {
                    add = true;
                }
                if (parent.Edit || child.Edit)
                {
                    edit = true;
                }
                if (parent.Delete || child.Delete)
                {
                    delete = true;
                }
                inteList.Add(new UserDataScope()
                {
                    organizeId = it,
                    Add = add,
                    Edit = edit,
                    Delete = delete
                });
                data.Remove(parent);
                subData.Remove(child);
            });
            return data.Union(subData).Union(inteList).ToList();
        }


        #endregion
    }
}
