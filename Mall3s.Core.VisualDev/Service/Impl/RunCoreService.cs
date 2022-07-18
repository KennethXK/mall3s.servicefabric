using Mall3s.Common.Const;
using Mall3s.Common.Enum;
using Mall3s.Common.Extension;
using Mall3s.Common.Helper;
using Mall3s.Core.System.Manager;
using Mall3s.Core.System.Service;
using Mall3s.Dependency;
using Mall3s.FriendlyException;
using Mall3s.JsonSerialization;
using Mall3s.System.Entitys.System;
using Mall3s.VisualDev.Entitys;
using Mall3s.VisualDev.Entitys.Entity;
using Mall3s.VisualDev.Entitys.Enum.VisualDevModelData;
using Mall3s.VisualDev.Entitys.Model.VisualDevModelData;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.VisualDev.Service.Impl
{

    /// <summary>
    /// 在线开发运行服务
    /// </summary>
    public class RunCoreService : IRunCoreService
    {
        private readonly ISqlSugarRepository<VisualDevModelDataEntity> _visualDevModelDataRepository;
        private readonly ISqlSugarRepository<VisualDevEntity> _visualDevRepository;
        private readonly IBillRullCoreService _billRuleService;
        private readonly IOrganizeCoreService _organizeService;
        private readonly IDepartmentCoreService _departmentService;
        private readonly IUsersCoreService _userService;
        private readonly IPositionCoreService _positionService;
        private readonly IDataBaseCoreService _databaseService;
        private readonly IDbLinkCoreService _dbLinkService;
        private readonly ISysCacheCoreService _sysCacheService;
        private readonly IDictionaryDataCoreService _dictionaryDataService;
        private readonly IDataInterfaceCoreService _dataInterfaceService;
        private readonly IDictionaryTypeCoreService _dictionaryTypeService;
        private readonly IProvinceCoreService _provinceService;
        private readonly IUserCoreManager _userManager; // 用户管理

        /// <summary>
        /// 初始化一个<see cref="RunService"/>类型的新实例
        /// </summary>
        public RunCoreService(ISqlSugarRepository<VisualDevModelDataEntity> visualDevModelDataRepository, 
            ISqlSugarRepository<VisualDevEntity> visualDevRepository,
            IUserCoreManager userManager, 
            IBillRullCoreService billRuleService,
            IOrganizeCoreService organizeService,
            IUsersCoreService userService,
            IPositionCoreService positionService,
            IDataBaseCoreService databaseService,
            IDbLinkCoreService dbLinkService,
            ISysCacheCoreService sysCacheService,
            IDictionaryDataCoreService dictionaryDataService,
            IDataInterfaceCoreService dataInterfaceService,
            IDictionaryTypeCoreService dictionaryTypeService,
            IProvinceCoreService provinceService,
            IDepartmentCoreService departmentService)
        {
            _visualDevModelDataRepository = visualDevModelDataRepository;
            _visualDevRepository = visualDevRepository;
            _userManager = userManager;
            _billRuleService = billRuleService;
            _organizeService = organizeService;
            _userService = userService;
            _positionService = positionService;
            _databaseService = databaseService;
            _dbLinkService = dbLinkService;
            _sysCacheService = sysCacheService;
            _dictionaryDataService = dictionaryDataService;
            _dataInterfaceService = dataInterfaceService;
            _dictionaryTypeService = dictionaryTypeService;
            _provinceService = provinceService;
            _departmentService = departmentService;
        }


        #region PrivateMethod

        #region 拆解模板

        /// <summary>
        /// 模板缓存数据转换
        /// 专门为模板缓存数据，会将子表内的控件全部获取出来
        /// 适用场景缓存模板数据
        /// </summary>
        /// <returns></returns>
        private List<FieldsModel> TemplateCacheDataConversion(List<FieldsModel> fieldsModelList)
        {
            var template = new List<FieldsModel>();
            //将模板内的无限children解析出来
            //包含子表children
            foreach (var item in fieldsModelList)
            {
                var config = item.__config__;
                switch (config.jnpfKey)
                {
                    //栅格布局
                    case "row":
                        {
                            template.AddRange(TemplateCacheDataConversion(config.children));
                        }
                        break;
                    //表格
                    case "table":
                        {
                            template.AddRange(TemplateCacheDataConversion(config.children));
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            template.AddRange(TemplateCacheDataConversion(config.children));
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            foreach (var collapse in config.children)
                            {
                                template.AddRange(TemplateCacheDataConversion(collapse.__config__.children));
                            }
                        }
                        break;
                    //tab标签
                    case "tab":
                        {
                            foreach (var collapse in config.children)
                            {
                                template.AddRange(TemplateCacheDataConversion(collapse.__config__.children));
                            }
                        }
                        break;
                    //文本
                    case "Mall3sText":
                        break;
                    //分割线
                    case "divider":
                        break;
                    //分组标题
                    case "groupTitle":
                        break;
                    default:
                        {
                            template.Add(item);
                        }
                        break;
                }
            }
            return template;
        }

        #endregion

        #region 解析模板数据

        /// <summary>
        /// 控制模板数据转换
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="fieldsModel">数据模板</param>
        /// <param name="actionType">操作类型(List-列表值,create-创建值,update-更新值,detail-详情值,transition-过渡值,query-查询)</param>
        /// <returns></returns>
        private object TemplateControlsDataConversion(object data, FieldsModel fieldsModel, string actionType = null)
        {
            try
            {
                object conversionData = new object();
                switch (fieldsModel.__config__.jnpfKey)
                {
                    #region 基础控件

                    //单行输入
                    case "comInput":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //多行输入
                    case "textarea":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //数字输入
                    case "numInputc":
                        {
                            conversionData = data.ToInt();
                        }
                        break;
                    //金额输入
                    case "Mall3sAmount":
                        {
                            conversionData = data.ToDecimal();
                        }
                        break;
                    //单选框组
                    case "radio":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //多选框组
                    case "checkbox":
                        {
                            if (data.ToString().Contains("["))
                                conversionData = data.ToString().ToObject<List<string>>();
                            else
                                conversionData = data.ToString();
                        }
                        break;
                    //下拉选择
                    case "select":
                        {
                            switch (actionType)
                            {
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                case "List":
                                    {
                                        if (data.ToString().Contains(","))
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple && actionType != "query")
                                        {
                                            if (data.ToString().Contains(","))
                                            {
                                                conversionData = data.ToString().ToObject<List<string>>();
                                            }
                                            else
                                            {
                                                conversionData = string.Join(",", data.ToString().Split(',').ToArray());
                                            }
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //时间选择
                    case "time":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", data);
                                    break;
                                case "create":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", data);
                                    break;
                                case "detail":
                                    conversionData = data.ToString();
                                    break;
                                default:
                                    conversionData = data.ToString();
                                    break;
                            }
                        }
                        break;
                    //时间范围
                    case "timeRange":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                                case "create":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                                case "transition":
                                    conversionData = data;
                                    break;
                                case "update":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                            }
                        }
                        break;
                    //日期选择
                    case "date":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "create":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "detail":
                                    conversionData = data.ToString();
                                    break;
                                case "update":
                                    conversionData = data;
                                    break;
                                default:
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                            }
                        }
                        break;
                    //日期范围
                    case "dateRange":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                                case "transition":
                                    conversionData = data;
                                    break;
                                case "create":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                                case "update":
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                            }
                        }
                        break;
                    //创建时间
                    case "createTime":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "create":
                                    conversionData = data.ToString();
                                    break;
                                case "update":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "detail":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", data);
                                    break;
                                default:
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", data);
                                    break;
                            }
                        }
                        break;
                    //修改时间
                    case "modifyTime":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                case "create":
                                    conversionData = data.ToString();
                                    break;
                                case "update":
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Ext.GetDateTime(data.ToString()));
                                    break;
                                default:
                                    conversionData = string.Format("{0:yyyy-MM-dd HH:mm:ss}", data);
                                    break;
                            }
                        }
                        break;
                    //文件上传
                    case "uploadFz":
                        {
                            if (data.ToString() != "[]")
                            {
                                conversionData = data.ToString().ToObject<List<FileControlsModel>>();
                            }
                            else
                            {
                                conversionData = null;
                            }
                        }
                        break;
                    //图片上传
                    case "uploadImg":
                        {
                            if (data.ToString() != "[]")
                            {
                                conversionData = data.ToString().ToObject<List<FileControlsModel>>();
                            }
                            else
                            {
                                conversionData = null;
                            }
                        }
                        break;
                    //颜色选择
                    case "colorPicker":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //评分
                    case "rate":
                        {
                            conversionData = data.ToInt();
                        }
                        break;
                    //开关
                    case "switch":
                        {
                            conversionData = data.ToInt();
                        }
                        break;
                    //滑块
                    case "slider":
                        {
                            if (fieldsModel.range)
                            {
                                conversionData = data.ToString().ToObject<List<int>>();
                            }
                            else
                            {
                                conversionData = data.ToInt();
                            };
                        }
                        break;
                    ////文本
                    //case "Mall3sText":
                    //    break;
                    //富文本
                    case "editor":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //单据组件
                    case "billRule":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //省市区联动
                    case "address":
                        {
                            switch (actionType)
                            {
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    conversionData = data.ToString().ToObject<List<string>>();
                                    break;
                            }
                        }
                        break;
                    //创建人员
                    case "createUser":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //修改人员
                    case "modifyUser":
                        {
                            conversionData = string.IsNullOrEmpty(data.ToString()) ? null : data.ToString();
                        }
                        break;
                    //所属组织
                    case "currOrganize":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //所属部门
                    case "currDept":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    //所属岗位
                    case "currPosition":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                    case "table":
                        {

                        }
                        break;
                    //级联
                    case "cascader":
                        switch (actionType)
                        {
                            case "transition":
                                {
                                    conversionData = data;
                                }
                                break;
                            default:
                                {
                                    if (fieldsModel.props.props.multiple)
                                    {
                                        conversionData = data.ToString().ToObject<List<List<string>>>();
                                    }
                                    else
                                    {
                                        conversionData = data.ToString().ToObject<List<string>>();
                                    }
                                }
                                break;
                        }
                        break;
                    default:
                        conversionData = data.ToString();
                        break;

                    #endregion

                    #region 高级控件

                    //公司组件
                    case "comSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            var list = data.ToString().ToObject<List<string>>();
                                            conversionData = string.Join(",", list.ToArray());
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //部门组件
                    case "depSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            var list = data.ToString().ToObject<List<string>>();
                                            conversionData = string.Join(",", list.ToArray());
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //岗位组件
                    case "posSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            var list = data.ToString().ToObject<List<string>>();
                                            conversionData = string.Join(",", list.ToArray());
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //用户组件
                    case "userSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            var list = data.ToString().ToObject<List<string>>();
                                            conversionData = string.Join(",", list.ToArray());
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //树形选择
                    case "treeSelect":
                        {
                            switch (actionType)
                            {
                                case "List":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "create":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "update":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            var list = data.ToString().ToObject<List<string>>();
                                            conversionData = string.Join(",", list.ToArray());
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "detail":
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.ToString().ToObject<List<string>>();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                                case "transition":
                                    {
                                        conversionData = data;
                                    }
                                    break;
                                default:
                                    {
                                        if (fieldsModel.multiple)
                                        {
                                            conversionData = data.CastTo("").Split(",", true).ToList();
                                        }
                                        else
                                        {
                                            conversionData = data.ToString();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    //弹窗选择
                    case "popupSelect":
                        {
                            conversionData = data.ToString();
                        }
                        break;
                        #endregion

                }
                return conversionData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 转换对应数据Object
        /// </summary>
        /// <param name="dataValue">数据值</param>
        /// <param name="dataKey">数据Key</param>
        /// <param name="fieldsModelList">模板</param>
        /// <param name="actionType">操作类型(List-列表值,create-创建值,detail-详情值,transition-过渡值)</param>
        /// <returns></returns>
        private string TransformDataObject(object dataValue, string dataKey, List<FieldsModel> fieldsModelList, string actionType = null)
        {
            StringBuilder sb = new StringBuilder();
            //根据KEY查找模板
            var model = fieldsModelList.Find(f => f.__vModel__ == dataKey);
            if (model != null)
            {
                switch (model.__config__.jnpfKey)
                {
                    //文件上传
                    case "uploadFz":
                        {
                            var fileList = TemplateControlsDataConversion(dataValue, model);
                            if (fileList != null)
                            {
                                sb.AppendFormat("{0}", fileList.Serialize());
                            }
                        }
                        break;
                    //图片上传
                    case "uploadImg":
                        {
                            var fileList = TemplateControlsDataConversion(dataValue, model);
                            if (fileList != null)
                            {
                                sb.AppendFormat("{0}", fileList.Serialize());
                            }
                        }
                        break;
                    //省市区联动
                    case "address":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                        }
                        break;
                    //树形选择
                    case "treeSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //公司组件
                    case "comSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //部门组件
                    case "depSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //岗位组件
                    case "posSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //用户组件
                    case "userSelect":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //滑块
                    case "slider":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //时间范围
                    case "timeRange":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                        }
                        break;
                    //日期范围
                    case "dateRange":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType).Serialize());
                        }
                        break;
                    //下拉选择
                    case "select":
                        {
                            if (model.multiple)
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                            }
                            else
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model));
                            }
                        }
                        break;
                    //复选框
                    case "checkbox":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                        }
                        break;
                    //级联
                    case "cascader":
                        {
                            sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model).Serialize());
                        }
                        break;
                    //日期选择
                    case "date":
                        {
                            try
                            {
                                DateTime.Parse(dataValue.ToString());
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(Ext.TimeToTimeStamp(Convert.ToDateTime(dataValue)), model, actionType));
                            }
                            catch
                            {
                                sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType));
                            }
                        }
                        break;
                    default:
                        sb.AppendFormat("{0}", TemplateControlsDataConversion(dataValue, model, actionType));
                        break;
                }
            }
            if (dataKey == "F_Id")
            {
                sb.AppendFormat("{0}", dataValue.ToString());
            }
            return sb.ToString().TrimStart('"').TrimEnd('"');
        }

        #endregion

        #region 缓存模板数据

        /// <summary>
        /// 获取可视化开发模板可缓存数据
        /// </summary>
        /// <param name="moldelId">模型id</param>
        /// <param name="formData">模板数据结构</param>
        /// <returns></returns>
        private async Task<Dictionary<string, object>> GetVisualDevTemplateData(string moldelId, List<FieldsModel> formData)
        {
            Dictionary<string, object> templateData = new Dictionary<string, object>();
            var cacheKey = CommonConst.VISUALDEV + _userManager.TenantId + "_" + moldelId;
            if (_sysCacheService.Exists(cacheKey))
            {
                templateData = _sysCacheService.Get(cacheKey).Deserialize<Dictionary<string, object>>();
            }
            else
            {
                foreach (var model in formData)
                {
                    if (model != null && model.__vModel__ != null)
                    {
                        ConfigModel configModel = model.__config__;
                        string fieldName1 = model.__vModel__;
                        string type = configModel.jnpfKey;
                        switch (type)
                        {
                            //单选框
                            case JnpfKeyConst.RADIO:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        var dictionaryDataEntityList = string.IsNullOrEmpty(configModel.dictionaryType) ? null : await _dictionaryDataService.GetList(configModel.dictionaryType);
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var optionList = model.__slot__.options;
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                    if (vModelType.DYNAMIC.GetDescription() == configModel.dataType)
                                    {
                                        //获取远端数据
                                        var dynamic = await _dataInterfaceService.GetInfo(model.__config__.propsUrl);
                                        if (1.Equals(dynamic.DataType))
                                        {
                                            var linkEntity = await _dbLinkService.GetInfo(dynamic.DBLinkId);
                                            var dt = _databaseService.GetInterFaceData(linkEntity, dynamic.Query);
                                            List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            foreach (var item in dynamicDataList)
                                            {
                                                Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                list.Add(dynamicDic);
                                            }
                                            templateData.Add(fieldName1, list);
                                        }
                                    }
                                }
                                break;
                            //下拉框
                            case JnpfKeyConst.SELECT:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        List<DictionaryDataEntity> dictionaryDataEntityList = string.IsNullOrEmpty(configModel.dictionaryType) ? null : await _dictionaryDataService.GetList(configModel.dictionaryType);
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    else if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var optionList = model.__slot__.options;
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                    else if (vModelType.DYNAMIC.GetDescription() == configModel.dataType)
                                    {
                                        //获取远端数据
                                        DataInterfaceEntity dynamic = await _dataInterfaceService.GetInfo(model.__config__.propsUrl);
                                        if (1.Equals(dynamic.DataType))
                                        {
                                            var linkEntity = await _dbLinkService.GetInfo(dynamic.DBLinkId);
                                            var dt = _databaseService.GetInterFaceData(linkEntity, dynamic.Query);
                                            List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            foreach (var item in dynamicDataList)
                                            {
                                                Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                list.Add(dynamicDic);
                                            }
                                            templateData.Add(fieldName1, list);
                                        }
                                    }
                                }
                                break;
                            //复选框
                            case JnpfKeyConst.CHECKBOX:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        List<DictionaryDataEntity> dictionaryDataEntityList = string.IsNullOrEmpty(configModel.dictionaryType) ? null : await _dictionaryDataService.GetList(configModel.dictionaryType);
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var optionList = model.__slot__.options;
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                    if (vModelType.DYNAMIC.GetDescription() == configModel.dataType)
                                    {
                                        //获取远端数据
                                        DataInterfaceEntity dynamic = await _dataInterfaceService.GetInfo(model.__config__.propsUrl);
                                        if (1.Equals(dynamic.DataType))
                                        {
                                            var linkEntity = await _dbLinkService.GetInfo(dynamic.DBLinkId);
                                            var dt = _databaseService.GetInterFaceData(linkEntity, dynamic.Query);
                                            List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            foreach (var item in dynamicDataList)
                                            {
                                                Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                list.Add(dynamicDic);
                                            }
                                            templateData.Add(fieldName1, list);
                                        }
                                    }
                                }
                                break;
                            //树形选择
                            case JnpfKeyConst.TREESELECT:
                                {
                                    if (vModelType.DICTIONARY.GetDescription() == configModel.dataType)
                                    {
                                        List<DictionaryDataEntity> dictionaryDataEntityList = await _dictionaryDataService.GetList();
                                        List<Dictionary<string, string>> dictionaryDataList = new List<Dictionary<string, string>>();
                                        foreach (var item in dictionaryDataEntityList)
                                        {
                                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                            dictionary.Add(item.Id, item.FullName);
                                            dictionaryDataList.Add(dictionary);
                                        }
                                        templateData.Add(fieldName1, dictionaryDataList);
                                    }
                                    else if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var props = model.props.props;
                                        var optionList = GetTreeOptions(model.options, props);
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item.value, item.label);
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                    else if (vModelType.DYNAMIC.GetDescription() == configModel.dataType)
                                    {
                                        //获取远端数据
                                        DataInterfaceEntity dynamic = await _dataInterfaceService.GetInfo(model.__config__.propsUrl);
                                        if (1.Equals(dynamic.DataType))
                                        {
                                            var linkEntity = await _dbLinkService.GetInfo(dynamic.DBLinkId);
                                            var dt = _databaseService.GetInterFaceData(linkEntity, dynamic.Query);
                                            List<Dictionary<string, object>> dynamicDataList = dt.Serialize().Deserialize<List<Dictionary<string, object>>>();
                                            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                            foreach (var item in dynamicDataList)
                                            {
                                                Dictionary<string, string> dynamicDic = new Dictionary<string, string>();
                                                dynamicDic.Add(item[model.__config__.props.value].ToString(), item[model.__config__.props.label].ToString());
                                                list.Add(dynamicDic);
                                            }
                                            templateData.Add(fieldName1, list);
                                        }
                                    }
                                }
                                break;
                            //公司
                            case JnpfKeyConst.COMSELECT:
                                {
                                    var com_organizeEntityList = await _organizeService.GetCompanyListAsync();
                                    List<Dictionary<string, string>> com_organizeList = new List<Dictionary<string, string>>();
                                    foreach (var item in com_organizeEntityList)
                                    {
                                        Dictionary<string, string> com_organize = new Dictionary<string, string>();
                                        com_organize.Add(item.Id, item.FullName);
                                        com_organizeList.Add(com_organize);
                                    }
                                    templateData.Add(fieldName1, com_organizeList);
                                }
                                break;
                            //部门
                            case JnpfKeyConst.DEPSELECT:
                                {
                                    var dep_organizeEntityList = await _departmentService.GetListAsync();
                                    List<Dictionary<string, string>> dep_organizeList = new List<Dictionary<string, string>>();
                                    foreach (var item in dep_organizeEntityList)
                                    {
                                        Dictionary<string, string> dep_organize = new Dictionary<string, string>();
                                        dep_organize.Add(item.Id, item.FullName);
                                        dep_organizeList.Add(dep_organize);
                                    }
                                    templateData.Add(fieldName1, dep_organizeList);
                                }
                                break;
                            //岗位
                            case JnpfKeyConst.POSSELECT:
                                {
                                    var positionEntityList = await _positionService.GetListAsync();
                                    List<Dictionary<string, string>> positionList = new List<Dictionary<string, string>>();
                                    foreach (var item in positionEntityList)
                                    {
                                        Dictionary<string, string> position = new Dictionary<string, string>();
                                        position.Add(item.Id, item.FullName);
                                        positionList.Add(position);
                                    }
                                    templateData.Add(fieldName1, positionList);
                                }
                                break;
                            //用户
                            case JnpfKeyConst.USERSELECT:
                                {
                                    var userEntityList = await _userService.GetList();
                                    List<Dictionary<string, string>> userList = new List<Dictionary<string, string>>();
                                    foreach (var item in userEntityList)
                                    {
                                        Dictionary<string, string> user = new Dictionary<string, string>();
                                        user.Add(item.Id, item.RealName + "/" + item.Account);
                                        userList.Add(user);
                                    }
                                    templateData.Add(fieldName1, userList);
                                }
                                break;
                            //数据字典
                            case JnpfKeyConst.DICTIONARY:
                                {
                                    var dictionaryTypeEntityLists = await _dictionaryTypeService.GetList();
                                    List<Dictionary<string, string>> dictionaryTypeList = new List<Dictionary<string, string>>();
                                    foreach (var item in dictionaryTypeEntityLists)
                                    {
                                        Dictionary<string, string> dictionaryType = new Dictionary<string, string>();
                                        dictionaryType.Add(item.Id, item.FullName);
                                        dictionaryTypeList.Add(dictionaryType);
                                    }
                                    templateData.Add(fieldName1, dictionaryTypeList);
                                }
                                break;
                            //省市区
                            case JnpfKeyConst.ADDRESS:
                                {
                                    var addressEntityList = await _provinceService.GetList();
                                    List<Dictionary<string, string>> addressList = new List<Dictionary<string, string>>();
                                    foreach (var item in addressEntityList)
                                    {
                                        Dictionary<string, string> address = new Dictionary<string, string>();
                                        address.Add(item.Id, item.FullName);
                                        addressList.Add(address);
                                    }
                                    templateData.Add(fieldName1, addressList);
                                }
                                break;
                            //级联选择
                            case JnpfKeyConst.CASCADER:
                                {
                                    if (vModelType.STATIC.GetDescription() == configModel.dataType)
                                    {
                                        var props = model.props.props;
                                        var optionList = GetTreeOptions(model.options, props);
                                        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                                        foreach (var item in optionList)
                                        {
                                            Dictionary<string, string> option = new Dictionary<string, string>();
                                            option.Add(item.value, item.label);
                                            list.Add(option);
                                        }
                                        templateData.Add(fieldName1, list);
                                    }
                                }
                                break;
                        }
                    }
                }
                //缓存2分钟
                _sysCacheService.Set(cacheKey, templateData, TimeSpan.FromMinutes(2));
            }
            return templateData;
        }

        #endregion

        #region 系统组件生成与解析

        /// <summary>
        /// 生成系统自动生成字段
        /// </summary>
        /// <param name="fieldsModelList">模板数据</param>
        /// <param name="allDataMap">真实数据</param>
        /// <param name="IsCreate">创建与修改标识 true创建 false 修改</param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> GenerateFeilds(List<FieldsModel> fieldsModelList, Dictionary<string, object> allDataMap, bool IsCreate)
        {
            int dicCount = allDataMap.Keys.Count;
            string[] strKey = new string[dicCount];
            allDataMap.Keys.CopyTo(strKey, 0);
            for (int i = 0; i < strKey.Length; i++)
            {
                //根据KEY查找模板
                var model = fieldsModelList.Find(f => f.__vModel__ == strKey[i]);
                if (model != null)
                {
                    //如果模板jnpfKey为table为子表数据
                    if ("table".Equals(model.__config__.jnpfKey) && allDataMap[strKey[i]] != null)
                    {
                        List<FieldsModel> childFieldsModelList = model.__config__.children;
                        var objectData = allDataMap[strKey[i]];
                        List<Dictionary<string, object>> childAllDataMapList = objectData.Serialize().Deserialize<List<Dictionary<string, object>>>();
                        List<Dictionary<string, object>> newChildAllDataMapList = new List<Dictionary<string, object>>();
                        foreach (var childmap in childAllDataMapList)
                        {
                            var newChildData = new Dictionary<string, object>();
                            foreach (KeyValuePair<string, object> item in childmap)
                            {
                                var childFieldsModel = childFieldsModelList.Where(c => c.__vModel__ == item.Key).FirstOrDefault();
                                if (childFieldsModel != null)
                                {
                                    var userInfo = await _userManager.GetUserInfo();
                                    if (childFieldsModel.__vModel__.Equals(item.Key))
                                    {
                                        string jnpfKeyType = childFieldsModel.__config__.jnpfKey;
                                        switch (jnpfKeyType)
                                        {
                                            case "billRule":
                                                if (IsCreate)
                                                {
                                                    string billNumber = await _billRuleService.GetBillNumber(childFieldsModel.__config__.rule);
                                                    if (!"单据规则不存在".Equals(billNumber))
                                                    {
                                                        newChildData[item.Key] = billNumber;
                                                    }
                                                    else
                                                    {
                                                        newChildData[item.Key] = "";
                                                    }
                                                }
                                                else
                                                {
                                                    newChildData[item.Key] = childmap[item.Key];
                                                }
                                                break;
                                            case "createUser":
                                                if (IsCreate)
                                                {
                                                    newChildData[item.Key] = userInfo.userId;
                                                }
                                                break;
                                            case "createTime":
                                                if (IsCreate)
                                                {
                                                    newChildData[item.Key] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                                                }
                                                break;
                                            case "modifyUser":
                                                if (!IsCreate)
                                                {
                                                    newChildData[item.Key] = userInfo.userId;
                                                }
                                                break;
                                            case "modifyTime":
                                                if (!IsCreate)
                                                {
                                                    newChildData[item.Key] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                                                }
                                                break;
                                            case "currPosition":
                                                var userEntity = _userService.GetInfoByUserId(userInfo.userId);
                                                if (!string.IsNullOrEmpty(userEntity.PositionId))
                                                {
                                                    var positionEntity = await _positionService.GetInfoById(userEntity.PositionId.Split(",").FirstOrDefault());
                                                    if (positionEntity != null)
                                                    {
                                                        newChildData[item.Key] = positionEntity.Id;
                                                    }
                                                }
                                                else
                                                {
                                                    newChildData[item.Key] = "";
                                                }
                                                break;
                                            case "currOrganize":
                                                {
                                                    if (userInfo.organizeId != null)
                                                    {
                                                        newChildData[item.Key] = userInfo.organizeId;
                                                    }
                                                    else
                                                    {
                                                        newChildData[item.Key] = "";
                                                    }
                                                }
                                                break;
                                            default:
                                                newChildData[item.Key] = childmap[item.Key];
                                                break;
                                        }
                                    }
                                }
                            }
                            newChildAllDataMapList.Add(newChildData);
                            allDataMap[strKey[i]] = newChildAllDataMapList;
                        }
                    }
                    else
                    {
                        var userInfo = await _userManager.GetUserInfo();
                        if (model.__vModel__.Equals(strKey[i]))
                        {
                            string jnpfKeyType = model.__config__.jnpfKey;
                            switch (jnpfKeyType)
                            {
                                case "billRule":
                                    if (IsCreate)
                                    {
                                        string billNumber = await _billRuleService.GetBillNumber(model.__config__.rule);
                                        if (!"单据规则不存在".Equals(billNumber))
                                        {
                                            allDataMap[strKey[i]] = billNumber;
                                        }
                                        else
                                        {
                                            allDataMap[strKey[i]] = "";
                                        }
                                    }
                                    break;
                                case "createUser":
                                    {
                                        if (IsCreate)
                                        {
                                            allDataMap[strKey[i]] = userInfo.userId;
                                        }
                                        else
                                        {
                                            allDataMap[strKey[i]] = await _userService.GetUserIdByRealName(allDataMap[strKey[i]].ToString());
                                        }
                                    }
                                    break;
                                case "createTime":
                                    if (IsCreate)
                                    {
                                        allDataMap[strKey[i]] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                                    }
                                    break;
                                case "modifyUser":
                                    if (!IsCreate)
                                    {
                                        allDataMap[strKey[i]] = userInfo.userId;
                                    }
                                    break;
                                case "modifyTime":
                                    if (!IsCreate)
                                    {
                                        allDataMap[strKey[i]] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                                    }
                                    break;
                                case "currPosition":
                                    var userEntity = _userService.GetInfoByUserId(userInfo.userId);
                                    if (!string.IsNullOrEmpty(userEntity.PositionId))
                                    {
                                        var positionEntity = await _positionService.GetInfoById(userEntity.PositionId.Split(",").FirstOrDefault());
                                        if (positionEntity != null)
                                        {
                                            allDataMap[strKey[i]] = positionEntity.Id;
                                        }
                                    }
                                    else
                                    {
                                        allDataMap[strKey[i]] = "";
                                    }
                                    break;
                                case "currOrganize":
                                    {
                                        if (userInfo.organizeId != null)
                                        {
                                            allDataMap[strKey[i]] = userInfo.organizeId;
                                        }
                                        else
                                        {
                                            allDataMap[strKey[i]] = "";
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            return allDataMap;
        }

        /// <summary>
        /// 将系统组件生成的数据转换为数据
        /// </summary>
        /// <param name="formData">数据库模板数据</param>
        /// <param name="templateData">模板真实数据</param>
        /// <param name="entity">真实数据</param>
        /// <returns></returns>
        private async Task<string> GetSystemComponentsData(List<FieldsModel> formData, Dictionary<string, object> templateData, string modelData)
        {
            //剔除无限极
            formData = TemplateDataConversion(formData);
            //数据库保存的F_Data
            Dictionary<string, object> dataMap = modelData.ToObject<Dictionary<string, object>>();
            int dicCount = dataMap.Keys.Count;
            string[] strKey = new string[dicCount];
            dataMap.Keys.CopyTo(strKey, 0);
            //自动生成的数据不在模板数据内
            var record = dataMap.Keys.Except(templateData.Keys).ToList();
            foreach (var key in record)
            {
                if (dataMap[key] != null)
                {
                    var dataValue = dataMap[key].ToString();
                    if (!string.IsNullOrEmpty(dataValue))
                    {
                        var model = formData.Where(f => f.__vModel__ == key).FirstOrDefault();
                        if (model != null)
                        {
                            ConfigModel configModel = model.__config__;
                            string type = configModel.jnpfKey;
                            switch (type)
                            {
                                //case "currDept":
                                //    {
                                //        var deptMapList = await _departmentService.GetListAsync();
                                //        dataMap[key] = deptMapList.Find(o => o.Id.Equals(dataMap[key].ToString())).FullName;
                                //    }
                                //    break;
                                case "createUser":
                                    {
                                        var createUser = await _userService.GetInfoByUserIdAsync(dataMap[key].ToString());
                                        if (createUser != null)
                                            dataMap[key] = createUser.RealName;
                                    }
                                    break;
                                case "modifyUser":
                                    {
                                        var modifyUser = await _userService.GetInfoByUserIdAsync(dataMap[key].ToString());
                                        if (modifyUser != null)
                                            dataMap[key] = modifyUser.RealName;
                                    }
                                    break;
                                case "currPosition":
                                    {
                                        var mapList = await _positionService.GetListAsync();
                                        dataMap[key] = mapList.Where(p => p.Id == dataMap[key].ToString()).FirstOrDefault().FullName;
                                    }
                                    break;
                                case "currOrganize":
                                    {
                                        var orgMapList = await _organizeService.GetListAsync();
                                        dataMap[key] = orgMapList.Find(o => o.Id.Equals(dataMap[key].ToString())).FullName;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            return dataMap.Serialize();
        }

        #endregion


        #region 数据转换与筛选

        /// <summary>
        /// 无表的数据筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="keyJsonMap"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        private List<VisualDevModelDataEntity> GetNoTableFilteringData(List<VisualDevModelDataEntity> list, Dictionary<string, object> keyJsonMap, List<FieldsModel> formData)
        {
            List<VisualDevModelDataEntity> realList = new List<VisualDevModelDataEntity>();
            foreach (var entity in list)
            {
                Dictionary<String, object> query = keyJsonMap;
                Dictionary<String, object> realEntity = entity.Data.Deserialize<Dictionary<string, object>>();
                if (query != null && query.Count != 0)
                {
                    int m = 0;
                    int dicCount = query.Keys.Count;
                    string[] strKey = new string[dicCount];
                    query.Keys.CopyTo(strKey, 0);
                    for (int i = 0; i < strKey.Length; i++)
                    {
                        var keyValue = keyJsonMap[strKey[i]];
                        var queryEntity = realEntity.Where(e => e.Key == strKey[i]).FirstOrDefault();
                        var model = formData.Where(f => f.__vModel__ == strKey[i]).FirstOrDefault();
                        if (queryEntity.Value != null && !string.IsNullOrWhiteSpace(keyValue.ToString()))
                        {
                            var realValue = queryEntity.Value;
                            var type = model.__config__.jnpfKey;
                            switch (type)
                            {
                                case JnpfKeyConst.DATE:
                                    {
                                        List<String> queryTime = keyValue.ToObeject<List<string>>();
                                        int formatType = 0;
                                        if (model.format == "yyyy-MM")
                                        {
                                            formatType = 1;
                                        }
                                        else if (model.format == "yyyy")
                                        {
                                            formatType = 2;
                                        }
                                        string value1 = string.Format("{0:yyyy-MM-dd}", Ext.GetDateTime(queryTime.First()));
                                        string value2 = string.Format("{0:yyyy-MM-dd}", Ext.GetDateTime(queryTime.Last()));
                                        if (Ext.IsInTimeRange(Ext.GetDateTime(realValue.ToString()), value1, value2, formatType))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.TIME:
                                    {
                                        List<String> queryTime = keyValue.ToObeject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), queryTime.First(), queryTime.Last(), 3))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "createTime":
                                    {
                                        List<String> dayTime1 = keyValue.ToObeject<List<string>>();
                                        string value1 = string.Format("{0:yyyy-MM-dd 00:00:00}", Ext.GetDateTime(dayTime1.First()));
                                        string value2 = string.Format("{0:yyyy-MM-dd 23:59:59}", Ext.GetDateTime(dayTime1.Last()));
                                        if (Ext.IsInTimeRange(Convert.ToDateTime(realValue), value1, value2))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "modifyTime":
                                    {
                                        List<String> dayTime1 = keyValue.ToObeject<List<string>>();
                                        string value1 = string.Format("{0:yyyy-MM-dd 00:00:00}", Ext.GetDateTime(dayTime1.First()));
                                        string value2 = string.Format("{0:yyyy-MM-dd 23:59:59}", Ext.GetDateTime(dayTime1.Last()));
                                        if (!string.IsNullOrEmpty(realValue.ToString()))
                                        {
                                            if (Ext.IsInTimeRange(Convert.ToDateTime(realValue), value1, value2))
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                    }
                                    break;
                                case "numInput":
                                    {
                                        List<string> numArray = keyValue.ToObeject<List<string>>();
                                        var numA = numArray.First().ToInt();
                                        var numB = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                        var numC = realValue.ToInt();
                                        if (numC >= numA && numC <= numB)
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "calculate":
                                    {
                                        List<string> numArray = keyValue.ToObeject<List<string>>();
                                        var numA = numArray.First().ToInt();
                                        var numB = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                        var numC = realValue.ToInt();
                                        if (numC >= numA && numC <= numB)
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        if (model.searchType == 2)
                                        {
                                            if (realValue.ToString().Contains(keyValue.ToString()))
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                        else if (model.searchType == 1)
                                        {
                                            //多选时为模糊查询
                                            if (model.multiple || type == "checkbox")
                                            {
                                                if (realValue.ToString().Contains(keyValue.ToString()))
                                                {
                                                    realEntity["id"] = entity.Id;
                                                    m++;
                                                }
                                            }
                                            else
                                            {
                                                if (realValue.ToString().Equals(keyValue.ToString()))
                                                {
                                                    realEntity["id"] = entity.Id;
                                                    m++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (realValue.ToString().Contains(keyValue.ToString()))
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        if (m == dicCount)
                        {
                            realList.Add(entity);
                        }
                    }
                }
                else
                {
                    realList.Add(entity);
                }
            }
            return realList;
        }

        /// <summary>
        /// 查询过滤数据
        /// </summary>
        /// <param name="keyJsonMap"></param>
        /// <param name="list"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetQueryFilteringData(Dictionary<string, object> keyJsonMap, List<VisualDevModelDataEntity> list, ColumnDesignModel columnDesignModel)
        {
            List<Dictionary<string, object>> realList = new List<Dictionary<string, object>>();
            foreach (var entity in list)
            {
                Dictionary<String, object> query = keyJsonMap;
                Dictionary<String, object> realEntity = entity.Data.Deserialize<Dictionary<String, object>>();
                var formData = columnDesignModel.searchList;
                if (query != null && query.Count != 0)
                {
                    //添加关键词全匹配计数，全符合条件则添加
                    int m = 0;
                    int dicCount = query.Keys.Count;
                    string[] strKey = new string[dicCount];
                    query.Keys.CopyTo(strKey, 0);
                    for (int i = 0; i < strKey.Length; i++)
                    {
                        var keyValue = keyJsonMap[strKey[i]];
                        var queryEntity = realEntity.Where(e => e.Key == strKey[i]).FirstOrDefault();
                        var model = formData.Where(f => f.__vModel__ == strKey[i]).FirstOrDefault();
                        if (queryEntity.Value != null && !string.IsNullOrWhiteSpace(keyValue.ToString()))
                        {
                            var realValue = queryEntity.Value;
                            var type = model.__config__.jnpfKey;
                            switch (type)
                            {
                                case JnpfKeyConst.DATERANGE:
                                    {
                                        List<String> dayTime1 = keyValue.Serialize().Deserialize<List<string>>();
                                        List<String> dayTime2 = realValue.Serialize().Deserialize<List<string>>();
                                        var newList1 = new List<string> { dayTime1[0].TrimEnd('至') };
                                        var newList2 = new List<string> { dayTime2[0].TrimEnd('至') };
                                        dayTime1.RemoveRange(0, 1);
                                        dayTime2.RemoveRange(0, 1);
                                        dayTime1.InsertRange(0, newList1);
                                        dayTime2.InsertRange(0, newList2);
                                        bool cont1 = Ext.timeCalendar(dayTime2[0], dayTime1[0], dayTime1[1]);
                                        bool cont2 = Ext.timeCalendar(dayTime2[1], dayTime1[0], dayTime1[1]);
                                        if (cont1 || cont2)
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.DATE:
                                    {
                                        List<String> queryTime = keyValue.ToObeject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), queryTime[0].TrimEnd('至'), queryTime[1].TrimEnd('至')))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.TIME:
                                    {
                                        List<String> queryTime = keyValue.ToObeject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), queryTime[0], queryTime[1]))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case JnpfKeyConst.TIMERANGE:
                                    {
                                        List<String> dayTime1 = keyValue.Serialize().Deserialize<List<string>>();
                                        List<String> dayTime2 = realValue.Serialize().Deserialize<List<string>>();
                                        if (dayTime2 != null)
                                        {
                                            var newList1 = new List<string> { dayTime1[0].TrimEnd('至') };
                                            var newList2 = new List<string> { dayTime2[0].TrimEnd('至') };
                                            dayTime1.RemoveRange(0, 1);
                                            dayTime2.RemoveRange(0, 1);
                                            dayTime1.InsertRange(0, newList1);
                                            dayTime2.InsertRange(0, newList2);
                                            bool cont1 = Ext.timeCalendar(dayTime2[0], dayTime1[0], dayTime1[1]);
                                            bool cont2 = Ext.timeCalendar(dayTime2[1], dayTime1[0], dayTime1[1]);
                                            if (cont1 || cont2)
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                    }
                                    break;
                                case "createTime":
                                    {
                                        List<String> dayTime1 = keyValue.ToObeject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), dayTime1[0].TrimEnd('至'), dayTime1.Last().TrimEnd('至')))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "modifyTime":
                                    {
                                        List<String> dayTime1 = keyValue.ToObeject<List<string>>();
                                        if (Ext.IsInTimeRange(realValue.ToDate(), dayTime1[0].TrimEnd('至'), dayTime1.Last().TrimEnd('至')))
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                case "numInput":
                                    {
                                        List<string> numArray = keyValue.ToObeject<List<string>>();

                                        var numA = numArray.First().ToInt();
                                        var numB = numArray.Last() == null ? Int64.MaxValue : numArray.Last().ToInt();
                                        var numC = realValue.ToInt();
                                        if (numC >= numA && numC <= numB)
                                        {
                                            realEntity["id"] = entity.Id;
                                            m++;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        if (model.searchType == 2)
                                        {
                                            if (realValue.ToString().Contains(keyValue.ToString()))
                                            {
                                                realEntity["id"] = entity.Id;
                                                m++;
                                            }
                                        }
                                        else if (model.searchType == 1)
                                        {
                                            //多选时为模糊查询
                                            if (model.multiple || type == "checkbox")
                                            {
                                                if (realValue.ToString().Contains(keyValue.ToString()))
                                                {
                                                    realEntity["id"] = entity.Id;
                                                    m++;
                                                }
                                            }
                                            else
                                            {
                                                if (realValue.ToString().Equals(keyValue.ToString()))
                                                {
                                                    realEntity["id"] = entity.Id;
                                                    m++;
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        if (m == dicCount)
                        {
                            realList.Add(realEntity);
                        }
                    }
                }
                else
                {
                    realEntity["id"] = entity.Id;
                    realList.Add(realEntity);
                }
            }
            return realList;
        }

        /// <summary>
        /// 查询转换格式
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetQueryDataConversion(List<VisualDevModelDataEntity> list)
        {
            List<Dictionary<string, object>> realList = new List<Dictionary<string, object>>();
            foreach (var entity in list)
            {
                Dictionary<String, object> realEntity = entity.Data.Deserialize<Dictionary<String, object>>();
                realEntity["id"] = entity.Id;
                realList.Add(realEntity);
            }
            return realList;
        }

        #endregion

        /// <summary>
        /// 获取无表列表数据
        /// </summary>
        /// <param name="modelId">模板ID</param>
        /// <returns></returns>
        private async Task<List<VisualDevModelDataEntity>> GetList(string modelId)
        {
            return await _visualDevModelDataRepository.Where(m => m.VisualDevId == modelId && m.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// options无限级
        /// </summary>
        /// <returns></returns>
        private List<OptionsModel> GetTreeOptions(List<object> model, PropsBeanModel props)
        {
            List<OptionsModel> options = new List<OptionsModel>();
            foreach (var item in model)
            {
                OptionsModel option = new OptionsModel();
                var dicObject = item.Serialize().Deserialize<Dictionary<string, object>>();
                option.label = dicObject[props.label].ToString();
                option.value = dicObject[props.value].ToString();
                if (dicObject.ContainsKey(props.children))
                {
                    var children = dicObject[props.children].Serialize().Deserialize<List<object>>();
                    options.AddRange(GetTreeOptions(children, props));
                }
                options.Add(option);
            }
            return options;
        }

        /// <summary>
        /// 获取动态无限级数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        private List<Dictionary<string, string>> GetDynamicInfiniteData(string data, PropsBeanModel props)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            var value = props.value;
            var label = props.label;
            var children = props.children;
            JToken dynamicDataList = JValue.Parse(data);
            foreach (var info in dynamicDataList)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic[value] = info.Value<string>(value);
                dic[label] = info.Value<string>(label);
                list.Add(dic);
                if (info.Value<object>(children) != null && info.Value<object>(children).ToString() != "")
                {
                    list.AddRange(GetDynamicInfiniteData(info.Value<object>(children).ToString(), props));
                }
            }
            return list;
        }

        /// <summary>
        /// 将有表转无表
        /// </summary>
        /// <param name="dicList"></param>
        /// <param name="mainPrimary"></param>
        /// <returns></returns>
        private List<VisualDevModelDataEntity> GetTableDataList(List<Dictionary<string, object>> dicList, string mainPrimary)
        {
            List<VisualDevModelDataEntity> list = new List<VisualDevModelDataEntity>();
            foreach (var dataMap in dicList)
            {
                VisualDevModelDataEntity entity = new VisualDevModelDataEntity();
                //需要将小写的转为大写
                entity.Data = dataMap.ToJson();
                entity.Id = dataMap[mainPrimary].ToString();
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// 获取有表单条数据
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetTableDataInfo(List<Dictionary<string, object>> dataList, List<FieldsModel> fieldsModels, string actionType)
        {
            //转换表字符串成数组
            foreach (var dataMap in dataList)
            {
                int dicCount = dataMap.Keys.Count;
                string[] strKey = new string[dicCount];
                dataMap.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    var dataValue = dataMap[strKey[i]];
                    if (dataValue != null && !string.IsNullOrEmpty(dataValue.ToString()) && !dataValue.ToString().Contains("[]"))
                    {
                        var model = fieldsModels.Find(f => f.__vModel__ == strKey[i]);
                        if (model != null)
                        {
                            switch (model.__config__.jnpfKey)
                            {
                                //日期选择
                                case "date":
                                    {
                                        dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, "List");
                                    }
                                    break;
                                //创建时间
                                case "createTime":
                                    {
                                        dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, "List");
                                    }
                                    break;
                                //修改选择
                                case "modifyTime":
                                    {
                                        dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, "List");
                                    }
                                    break;
                                default:
                                    dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, actionType);
                                    break;
                            }

                        }
                    }
                }
            }
            return dataList;
        }

        /// <summary>
        /// 转换字典列表内是否有时间戳
        /// </summary>
        /// <param name="dicList"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetTableDataListByDic(List<Dictionary<string, object>> dicList, List<FieldsModel> fieldsModels)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (var dataMap in dicList)
            {
                int dicCount = dataMap.Keys.Count;
                string[] strKey = new string[dicCount];
                dataMap.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    var dataValue = dataMap[strKey[i]];
                    if (dataValue != null && !string.IsNullOrEmpty(dataValue.ToString()))
                    {
                        var model = fieldsModels.Find(f => f.__vModel__ == strKey[i]);
                        if (model != null)
                        {
                            dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, "transition");
                        }
                    }
                }
                list.Add(dataMap);
            }
            return list;
        }

        /// <summary>
        /// 获取有表单条数据 转时间戳
        /// </summary>
        /// <param name="main"></param>
        /// <param name="fieldsModels"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetTableDataInfoByTimeStamp(List<Dictionary<string, object>> dataList, List<FieldsModel> fieldsModels, string actionType)
        {
            //转换表字符串成数组
            foreach (var dataMap in dataList)
            {
                int dicCount = dataMap.Keys.Count;
                string[] strKey = new string[dicCount];
                dataMap.Keys.CopyTo(strKey, 0);
                for (int i = 0; i < strKey.Length; i++)
                {
                    var dataValue = dataMap[strKey[i]];
                    if (dataValue != null && !string.IsNullOrEmpty(dataValue.ToString()) && !dataValue.ToString().Contains("[]"))
                    {
                        var model = fieldsModels.Find(f => f.__vModel__ == strKey[i]);
                        if (model != null)
                        {
                            dataMap[strKey[i]] = TemplateControlsDataConversion(dataMap[strKey[i]], model, actionType);
                        }
                    }
                }
            }
            return dataList;
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 模板数据转换
        /// </summary>
        /// <param name="fieldsModelList"></param>
        /// <returns></returns>
        public List<FieldsModel> TemplateDataConversion(List<FieldsModel> fieldsModelList)
        {
            var template = new List<FieldsModel>();
            //将模板内的无限children解析出来
            //不包含子表children
            foreach (var item in fieldsModelList)
            {
                var config = item.__config__;
                switch (config.jnpfKey)
                {
                    //栅格布局
                    case "row":
                        {
                            template.AddRange(TemplateDataConversion(config.children));
                        }
                        break;
                    //表格
                    case "table":
                        {
                            template.Add(item);
                        }
                        break;
                    //卡片
                    case "card":
                        {
                            template.AddRange(TemplateDataConversion(config.children));
                        }
                        break;
                    //折叠面板
                    case "collapse":
                        {
                            foreach (var collapse in config.children)
                            {
                                template.AddRange(TemplateDataConversion(collapse.__config__.children));
                            }
                        }
                        break;
                    case "tab":
                        foreach (var collapse in config.children)
                        {
                            template.AddRange(TemplateDataConversion(collapse.__config__.children));
                        }
                        break;
                    //文本
                    case "Mall3sText":
                        break;
                    //分割线
                    case "divider":
                        break;
                    //分组标题
                    case "groupTitle":
                        break;
                    default:
                        {
                            template.Add(item);
                        }
                        break;
                }
            }
            return template;
        }

        /// <summary>
        /// 获取有表详情系统组件数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelList"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> GetTableInfoBySystemComponentsData(string id, List<FieldsModel> modelList, string data)
        {
            //获取Redis缓存模板数据
            var templateData = await GetVisualDevTemplateData(id, modelList);
            data = await GetSystemComponentsData(modelList, templateData, data);
            return data;
        }

        /// <summary>
        /// 动态表单时间格式转换
        /// </summary>
        /// <param name="diclist"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> DateConver(List<Dictionary<string, object>> diclist)
        {
            foreach (var item in diclist)
            {
                foreach (var dic in item.Keys)
                {
                    if (item[dic] is DateTime)
                    {
                        item[dic] = item[dic].ToString() + " ";
                    }
                }
            }
            return diclist;
        }
        #endregion
    }
}
