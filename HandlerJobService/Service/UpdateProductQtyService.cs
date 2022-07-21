using Microsoft.Extensions.Configuration;
using System;
using Web.Framework;
using Web.Quartz;

namespace HandleJobService
{
    public class UpdateProductQtyService : CronJobHostServiceBase<UpdateProductQtyJob>, IBackDoor
    {
        IConfiguration configuration => Globals.Configuration;

        //设置时间
        public override string JobCron => string.IsNullOrEmpty(configuration["UpdateProductQtyTime"]) ? "0 0/10 * * * ?" : configuration["UpdateProductQtyTime"];

        public UpdateProductQtyService(IServiceProvider services) : base(services)
        {

        }
    }
}
