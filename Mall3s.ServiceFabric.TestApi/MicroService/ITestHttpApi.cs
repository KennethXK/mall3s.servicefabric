using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;

namespace Mall3s.ServiceFabric.TestApi
{
    [HttpHost("http://mall3s.test")]
   public interface ITestHttpApi : IHttpApi
    {
        [HttpGet("/Home/home/GetNacosServerAddr")]
        Task<string> GetAsync();
    }
}
