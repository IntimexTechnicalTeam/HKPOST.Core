using HandleTestJobService;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Framework;

namespace HandleJobService
{
    /// <summary>
    /// 具体的Job，业务逻辑
    /// </summary>
    [DisallowConcurrentExecution]
    public class UpdateProductQtyJob : BaseJob
    {
        public override string categoryName => this.GetType().FullName;
        public override string JobName => "定时更新ProductQty表数据";

        public UpdateProductQtyJob(IServiceProvider services) : base(services) { }

        protected override async Task<SystemResult> Handle()
        {
            var result = new SystemResult();

            /////do something here    
            //IServiceScope scope = this.services.CreateScope();
            //using (scope)
            //{
            //    ITestService testService = scope.ServiceProvider.GetService<ITestService>();

            //    result = await testService.Hello("q2134");
            //}
 
            using var scope = this.services.CreateScope();
        
            return result;
        }
    }
}
