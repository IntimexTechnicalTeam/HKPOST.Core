using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Framework;

namespace WS.BLL
{
    public interface IDealProductQtyBll : IDependency
    {
        Task<SystemResult> DealProductQty(Guid Id);
    }
}
