using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace robstagram.Models.Entities
{
    public class Comment : EntityBase
    {
        public Customer Owner { get; set; }
        public string Text { get; set; }
    }
}
