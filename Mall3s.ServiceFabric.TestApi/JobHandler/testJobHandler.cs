using DotXxlJob.Core;
using DotXxlJob.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall3s.ServiceFabric.TestApi.JobHandler
{
    [JobHandler("testJobHandler")]
    public class testJobHandler : IJobHandler
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<ReturnT> Execute(JobExecuteContext context)
        {
            Console.WriteLine(context.JobParameter);
            return ReturnT.Success("测试成功");
        }
    }
}
