using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace robstagram.Models.Entities
{
    public class Like : EntityBase
    {
        public int EntryId { get; set; }
        public Entry Entry { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
