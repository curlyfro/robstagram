using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using robstagram.Models.Entities;

namespace robstagram.ViewModels
{
    public class ActivityData
    {
        public ActivityAction Action { get; set; }
        public int PostId { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Actors { get; set; }
        public DateTime LastChange { get; set; }
    }

    public enum ActivityTimeRange
    {
        Today,
        Yesterday,
        Week,
        Month,
        Other
    }

    public enum ActivityAction
    {
        Liked,
        Commented
    }
}