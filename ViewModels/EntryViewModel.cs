using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using robstagram.Models.Entities;

namespace robstagram.ViewModels
{
    public class EntryViewModel
    {
        public string Description { get; set; }
        public IFormFile Image { get; set; }        
    }
}
