using Mall3s.Dependency;
using Mall3s.DynamicApiController;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Mall3s.SpecificationDocument
{
    /// <summary>
    /// 过滤一些特殊的标签
    /// </summary>
    [SuppressSniffer]
    public class HiddenApiDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// 配置拦截
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            
            if (!App.SpecificationApiDocsHideList.Any())//如果都不需要隐藏，则不隐藏
            {
                return;
            }
              
           foreach(var fullName in App.SpecificationApiDocsHideList)
            {
                foreach (var item in context.ApiDescriptions)
                {
                    if (item.ActionDescriptor!=null&&item.ActionDescriptor.DisplayName.Contains(fullName))
                    {
                        swaggerDoc.Paths.Remove("/" + item.RelativePath);
                     
                    }
                }
            }

            
           
        }
    }
}