using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace robstagram.Models.Entities
{
    public class Entry : EntityBase
    {
        public Customer Owner { get; set; }

        public Image Picture { get; set; }
        public string Description { get; set; }

        public List<Like> Likes { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
