using Mall3s.Dependency;
using Mall3s.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall3s.ServiceFabric.TestApi
{ /// <summary>
  /// 测试
  /// </summary>
    [ApiDescriptionSettings(Tag = "Test", Name = "WebApiClient", Order = 800)]
    [ApiController]
    [Route("[controller]")]
    public class WebApiClientDemoController : IDynamicApiController, ITransient
    {
        private ITestHttpApi _api;
        public WebApiClientDemoController(ITestHttpApi api)
        {
            _api = api;
        }
        /// <summary>
        /// 获取测试Nacos
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("GetWebApiClientAddr")]
        public async Task<dynamic> GetNacosServerAddrAsync()
        {
            var result = await _api.GetAsync();
            return result;
        }

    }
}
