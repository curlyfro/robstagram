using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace robstagram.ViewModels
{
    public class PostData
    {
      public int Id { get; set; }
      public string Owner { get; set; }
      public string ImageUrl { get; set; }
      public string Description { get; set; }
      public List<string> Likes { get; set; }
      public List<CommentData> Comments { get; set; }
      public DateTime DateCreated { get; set; }
    }
}
