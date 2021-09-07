using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.DAL
{
    public class MallContextFactory
    {

        private static readonly Lazy<MallDbContext> _instance = new Lazy<MallDbContext>(() => new MallDbContext());
        public static MallDbContext GetCurrentDbContext()
        {
            return _instance.Value;
        }
    }
}
