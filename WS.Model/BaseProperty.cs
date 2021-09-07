using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Model
{
    public abstract class BaseProperty
    {
        [Column(Order = 98)]
        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [Column(Order = 99)]
        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [Required]
        [Column(Order = 100)]
        public DateTime CreateDate { get; set; }

        [Column(Order = 101)]
        public DateTime? UpdateDate { get; set; }

        [Required]
        //[MaxLength(36)]
        [Column(Order = 102)]
        public Guid CreateBy { get; set; }

        //[MaxLength(36)]
        [Column(Order = 103)]
        public Guid? UpdateBy { get; set; }

      

    }
}
