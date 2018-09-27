using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using robstagram.Models.Entities;

namespace robstagram.ViewModels
{
    /// <summary>
    /// PostViewModel class for creating new posts in db using Web API
    /// </summary>
    public class PostViewModel
    {
        /// <summary>
        /// Description text of the post
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Relative image url on server received by upload service
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// Image size in bytes received by upload service
        /// </summary>
        public long Size { get; set; }
    }
}
