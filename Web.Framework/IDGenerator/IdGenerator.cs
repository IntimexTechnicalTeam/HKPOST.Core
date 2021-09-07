using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Web.Framework.IDGenerator
{
    public class IdGenerator
    {
        public IdGenerator()
        {
            var machineId = Convert.ToUInt16(Globals.Configuration["WorkerID"] ?? "1");
            var options = new IdGeneratorOptions(machineId);
            YitIdHelper.SetIdGenerator(options);
        }

        public static string NewId => YitIdHelper.NextId().ToString();
    }
}
