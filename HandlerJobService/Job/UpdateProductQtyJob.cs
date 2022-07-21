using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Threading.Tasks;
using Web.Framework;
using WS.BLL;

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

            using var scope = this.services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDealProductQtyBll>();
            result = await service.HandleQtyAsync();

            return result;
        }
    }
}
