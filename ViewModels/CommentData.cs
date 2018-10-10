using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace robstagram.ViewModels
{
    public class CommentData
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
