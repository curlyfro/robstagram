using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace robstagram.ViewModels
{
    public class ProfileData
    {
      public string Message { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public string PictureUrl { get; set; }
      public long FacebookId { get; set; }
      public string Location { get; set; }
      public string Locale { get; set; }
      public string Gender { get; set; }
    }
}
