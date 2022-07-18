using Mall3s.Common.Const;
using Mall3s.Common.Extension;
using Mall3s.Common.Filter;
using Mall3s.DataEncryption;
using Mall3s.Demo.BaseData.Entitys;
using Mall3s.Dependency;
using Mall3s.DynamicApiController;
using Mall3s.Nacos.ServiceDiscovery;
using Mall3s.ServiceFabric.Core;
using Mall3s.ServiceFabric.Core.Extensions;
using Mall3s.ServiceFabric.Entity;
using Mall3s.ServiceFabric.Manager;
using Mall3s.ServiceFabric.Model.Swagger;
using Mall3s.ServiceFabric.TestApi.Etities;
using Mall3s.ServiceFabric.TestApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nacos.V2;
using SqlSugar;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall3s.ServiceFabric.TestApi
{  /// <summary>
   /// 测试
   /// </summary>
    [ApiDescriptionSettings(Tag = "Test", Name = "TestServiceFabric", Order = 800)]
    [Route("[controller]")]
    public class HomeController : IDynamicApiController, ITransient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserManager _userManager;
        private INacosAppConfigService _scv;
        private INacosDiscoveryService _sdv;
        private IConfiguration _configuration;
        private readonly TranslateHelper _translate;
        private readonly ISwaggerProvider _swaggerProvider;
        private readonly ISqlSugarRepository<DbLinkEntity> _sqlSugarRepository;
        private readonly ISqlSugarRepository<ExchangeRate> _exchangeRate;
        private readonly ISqlSugarRepository<MapEntity> _mapRepository;

        private readonly ILogger<HomeController> _logger;
        //   private INacosAppConfigService _nacosConfig;
        public HomeController(INacosAppConfigService scv,
            INacosDiscoveryService sdv,
            IUserManager userManager,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            TranslateHelper translate,
            ISwaggerProvider swaggerProvider,
            ISqlSugarRepository<DbLinkEntity> sqlSugarRepository,
            ISqlSugarRepository<ExchangeRate> exchangeRate,
            ISqlSugarRepository<MapEntity> mapRepository,
            ILogger<HomeController> logger)
        {
            _scv = scv;
            _sdv = sdv;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _configuration = configuration;
            _translate = translate;
            _swaggerProvider = swaggerProvider;
            _sqlSugarRepository = sqlSugarRepository;
            _exchangeRate = exchangeRate;
            _logger = logger;
            _mapRepository = mapRepository;
        }

        public void Login()
        {
           var accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
                {
                    { ClaimConst.CLAINM_USERID, "47ff0279-aa26a4fbeb59f630c8776f759"},
                    { ClaimConst.CLAINM_ACCOUNT, "123" },
                    { ClaimConst.CLAINM_REALNAME, "123" },
                    { ClaimConst.CLAINM_ADMINISTRATOR, true },
                    { ClaimConst.TENANT_ID, "1" },
                    { ClaimConst.TENANT_DB_NAME, "erp_tenant" }
                }, long.Parse("360000"));
            var httpContext = _httpContextAccessor.HttpContext;
            // 设置Swagger自动登录
            httpContext.SigninToSwagger(accessToken);
        }


        /// <summary>
        /// 获取测试Nacos
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("CreateTable")]
        public async Task<dynamic> CreateTable()
        {
            // context.CreateTable(false, 255,  typeof(SlicesEntity));
            // context.CreateTable(false, 255, typeof(DiagreportsEntity));
            // context.CreateTable(false, 255, typeof(ZyBaseCodeEntity));
            // context.CreateTable(false, 255, typeof(SpecimensEntity));
            //context.CreateTable(false, 255, typeof(ZyOaArticleEntity));
            //context.CreateTable(false, 255, typeof(ZyOaBillEntity));
            //context.CreateTable(false, 255, typeof(ZyOaCapitalEntity));


            ///  context.CreateTable(false, 255, typeof(ZyOaCustomerEntity)); error










            //  context.CreateTable(false, 255, typeof(ZyOaCustomerTrackEntity));
            //  context.CreateTable(false, 255, typeof(ZyOaFinwaterEntity));
            //  context.CreateTable(false, 255, typeof(ZyOaGoodsEntity));
            ////  context.CreateTable(false, 255, typeof(ZyOaPriceEntity));
            // context.CreateTable(false, 255, typeof(ZyOaPricelistEntity));




            // typeof(ZyOaCustomerEntity), typeof(ZyOaProContractEntity), typeof(ZyOaProCostEntity),  typeof(ZyZbContractEntity),

           // context.CreateTable(false, 255, typeof(ZyZbExpertEntity), typeof(ZyZbPartnerbillEntity), typeof(ZyZbPartnerEntity), typeof(ZyZbwfBidenrollEntity), typeof(ZyZbwfConsultEntity), typeof(ZyZbwfInvitebidsEntity), typeof(ZyZbwfProjectfsrEntity), typeof(ZyZbwfProjectitemEntity), typeof(ZyZbwfProposalEntity), typeof(ZyZbwfTenderinfoEntity), typeof(ZyZbwfTenderingEntity)); 


            return true;
        }

        /// <summary>
        /// 获取测试Nacos
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("GetNacosConfig")]
        public async Task<dynamic> GetNacosConfigAsync()
        {
            Login();
           var model= _userManager.User;

            var dataId = "Mall3s.Cache";
            var group = "Mall3s.ServiceFabric";
            var result = await _scv.GetNacosMicroConfigAsync<string>(dataId, group);
            return result;
        }

        /// <summary>
        /// 获取测试Nacos
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("GetNacosServerAddr")]
        public async Task<dynamic> GetNacosServerAddrAsync()
        {
            var dataId = "mall3s.test";
            var group = "Mall3s.ServiceFabric";
            var result = await _sdv.GetNacosMicroBaseUrlAsync(dataId, group);
            return result;
        }


        /// <summary>
        /// 获取测试Nacos
        /// </summary>
        /// <param name="type"></param>
        /// <param name="frm"></param>
        /// <param name="to"></param>
        /// <param name="translateType">0代表google，1代表百度</param>
        /// <returns></returns>
        [HttpGet("Translate")]
        public  dynamic Translate(string q,string frm,string to,int translateType=0) 
        {
            if (translateType == 0)
                return _translate.TranslateByGoogle(q, frm, to);
            else
                return _translate.TranslateByBaidu(q, frm, to);
        }

        static async Task<string> GetConfig(INacosConfigService svc)
        {
            var dataId = "Mall3s.Cache";
            var group = "Mall3s.ServiceFabric";

            await Task.Delay(500);
            var config = await svc.GetConfig(dataId, group, 5000L);
            return config;
        }

        [HttpGet("config")]
        public object GetConfig()
        {
            var swaggerDefault = _swaggerProvider.GetSwagger("Default");
            SwaggerRoot swagger = new SwaggerRoot();
            swagger.Info = new SwaggerInfo
            {
                Title = swaggerDefault.Info.Title,
                Description = swaggerDefault.Info.Description,
                Version = swaggerDefault.Info.Version,
            };
            swagger.Tags = swaggerDefault.Paths
                .Select(a => a.Value)
                .SelectMany(a => a.Operations.Values)
                .SelectMany(a => a.Tags)
                .Select(a => new SwaggerTag { Name = a.Name, Description = a.Description })
                .Distinct(a => a.Name)
                .ToList();
            #region Path
            var apiPath = new Dictionary<string, Dictionary<string, ApiInfo>>();
            foreach (var path in swaggerDefault.Paths)
            {
                var operations = path.Value.Operations;
                var apiOperations = new Dictionary<string, ApiInfo>();
                foreach (var operation in operations)
                {
                    ApiInfo apiInfo = new ApiInfo();
                    apiInfo.Tags = operation.Value.Tags.Select(a => a.Name).ToList();
                    apiInfo.Summary = operation.Value.Summary;
                    var method = operation.Key.GetDisplayName();
                    var operationId = $"{path.Key}Using{method}";
                    var currentApiOperations = apiOperations.Values.Select(a => a.OperationId).Where(a => a.StartsWith(operationId));
                    if (currentApiOperations != null && currentApiOperations.Any())
                    {
                        apiInfo.OperationId = operationId + "_" + (currentApiOperations.Count() - 1);
                    }
                    else 
                    {
                        apiInfo.OperationId = operationId;
                    }
                    #region reponse & produces
                    var response = new Dictionary<string, ApiRes>();
                    var produces = new List<string>();
                    foreach (var res in operation.Value.Responses)
                    {
                        produces.AddRange(res.Value.Content.Keys);
                        var apiContent = new Dictionary<string, ApiContent>();
                        foreach (var resContent in res.Value.Content)
                        {
                            var apiContentRef = resContent.Value.Schema.Reference?.ReferenceV3;
                            apiContent.Add(resContent.Key, new ApiContent { 
                                Schema = new ApiSchema
                                {
                                    Ref = resContent.Value.Schema.Reference?.ReferenceV3,
                                    OriginalRef = apiContentRef?[(apiContentRef.LastIndexOf("/") + 1)..]
                                }
                            });
                        }
                         response[res.Key] = new ApiRes {
                            Description = res.Value.Description,
                            Content = apiContent
                         };
                    }
                    if (!response.ContainsKey("201"))
                    {
                        response["201"] = new ApiRes
                        {
                            Description = "Created"
                        };
                    }
                    if (!response.ContainsKey("401"))
                    {
                        response["401"] = new ApiRes
                        {
                            Description = "Unauthorized"
                        };
                    }
                    if (!response.ContainsKey("403"))
                    {
                        response["403"] = new ApiRes
                        {
                            Description = "Forbidden"
                        };
                    }
                    if (!response.ContainsKey("404"))
                    {
                        response["404"] = new ApiRes
                        {
                            Description = "Not Found"
                        };
                    }
                    apiInfo.Responses = response;
                    if (produces.Any())
                    {
                        apiInfo.Produces = produces.Distinct().ToList();
                    }
                    #endregion

                    #region params

                    var apiParams = new List<ApiParam>();
                    if (operation.Value.Parameters != null && operation.Value.Parameters.Any())
                    {
                        foreach (var param in operation.Value.Parameters)
                        {
                            apiParams.Add(new ApiParam
                            {
                                Name = param.Name,
                                In = param.In?.GetDisplayName(),
                                Description = param.Description,
                                Type = param.Schema.Type
                            });
                        }
                    }
                    if (operation.Value.RequestBody != null)
                    {
                        var apiRef = operation.Value.RequestBody.Content.Values.FirstOrDefault()?.Schema.Reference?.ReferenceV3;
                        apiParams.Add(new ApiParam { 
                            Name = "",
                            In = "body",
                            Required = operation.Value.RequestBody.Required,
                            Description = operation.Value.RequestBody.Description,
                            Schema = new ApiSchema { 
                                Ref = apiRef,
                                OriginalRef = apiRef?[(apiRef.LastIndexOf("/") + 1)..]
                            }
                        });
                    }
                    apiInfo.Parameters = apiParams;

                    #endregion

                    apiOperations.Add(method, apiInfo);
                }
                apiPath.Add(path.Key, apiOperations);
            }
            swagger.Paths = apiPath;
            #endregion

            #region model
            swagger.Components = new ModelComponent { Schemas = new Dictionary<string, ModelDefinition>() };
            foreach (var schema in swaggerDefault.Components.Schemas)
            {
                var properties = new Dictionary<string, ModelProperty>();
                foreach (var property in schema.Value.Properties)
                {
                    properties.Add(property.Key, new ModelProperty
                    {
                        Type = property.Value.Type,
                        Format = property.Value.Format,
                        Ref = property.Value.Reference?.ReferenceV3
                    });
                }
                swagger.Components.Schemas.Add(schema.Key, new ModelDefinition {
                    Properties = properties,
                    Title = schema.Value.Title,
                    Type = schema.Value.Type,
                });
            }
            #endregion
            return swagger;
        }

        [HttpGet("dblink")]
        public async Task<List<DbLinkEntity>> GetDbLink()
        {
            var aaa = await _exchangeRate.ToListAsync();
            await _exchangeRate.InsertAsync(new ExchangeRate
            { 
                Id = Yitter.IdGenerator.YitIdHelper.NextId().ToString(),
                CreatorTime = DateTime.Now,
                CreatorUserId = "0",
                ExchangeDate = DateTime.Now,
                From = "GBP",
                To = "CNY",
                Rate = 8.65M
            });
            /*var test = await _exchangeRate.Context.Queryable<ExchangeRateEntity>().AS("mall3s_demo.exchange_rate")
                .LeftJoin<DbLinkEntity>((aa, bb) => aa.Id == bb.Id).AS<DbLinkEntity>("mall3s_system_test.BASE_DBLINK")
                .Select((aa, bb) => new { aa = aa.From, bb = bb.FullName })
                .ToListAsync();*/
            _logger.LogInformation("插入汇率");
            return await _sqlSugarRepository.ToListAsync();
        }

        [HttpPost("testpostfrombody")]
        public SwaggerTag TestPostFromBody([FromBody] SwaggerTag swaggerTag)
        {
            return swaggerTag;
        }

        /// <summary>
        /// 地图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("map")]
        public async Task<dynamic> GetList([FromQuery] PageInputBase input)
        {
            var list = await _mapRepository
                .Context
                .Queryable<MapEntity, UserEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == a.CreatorUserId))
                .Select((a, b) => new { CreatorTime = a.CreatorTime, CreatorUser = SqlFunc.MergeString(b.RealName, "/", b.Account), EnCode = a.EnCode, EnabledMark = a.EnabledMark, FullName = a.FullName, Id = a.Id, SortCode = a.SortCode, DeleteMark = a.DeleteMark })
                .MergeTable()
                .Where(x => x.DeleteMark == null)
                .WhereIF(!string.IsNullOrEmpty(input.keyword), m => m.FullName.Contains(input.keyword) || m.EnCode.Contains(input.keyword)).OrderBy(t => t.SortCode)
                .Select<MapListOutput>()
                .ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<MapListOutput>()
            {
                list = list.list,
                pagination = list.pagination
            };
            return PageResult<MapListOutput>.SqlSugarPageResult(pageList);
        }
    }
}
