using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Framework;
using Web.MQ;
using WS.BLL;

namespace HandleMqService
{
    /// <summary>
    /// 
    /// </summary>
    public class DealProductQtyService : ConsumerHostServiceBase,IBackDoor
    {        
        public DealProductQtyService(IServiceProvider services) : base(services)
        { 
        
        
        }

        protected override string Queue => MQSetting.UpdateQtyQueue;
        protected override string Exchange => MQSetting.UpdateQtyExchange;

        protected override string categoryName => this.GetType().FullName;

        protected override async Task Handle(string msg)
        {          
            using var scope = base.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDealProductQtyBll>();
            var result = await service.DealProductQty(Guid.Parse(msg));
            SaveLog(result.Message, result.Succeeded);
        }

      
    }
}
