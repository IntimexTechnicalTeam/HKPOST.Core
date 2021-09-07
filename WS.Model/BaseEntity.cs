using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Model
{
    public class BaseEntity<TKey> : BaseProperty
    {
        public BaseEntity()
        {
            CreateDate = DateTime.Now;
            CreateBy = Guid.Empty;
            UpdateDate = DateTime.Now;
            UpdateBy = Guid.Empty;
            IsActive = true;
            IsDeleted = false;
        }

         
        [Key]
        [Column(Order = 1)] 
        public TKey Id { get; set; }
        [Column(Order = 2)]
        public Guid ClientId { get; set; }
    }
}
