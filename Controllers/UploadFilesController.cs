using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using robstagram.Data;
using robstagram.Models.Entities;

namespace robstagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFilesController : Controller
    {
        private readonly ApplicationDbContext _appDbContext;

        public UploadFilesController(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        /// <summary>
        /// POST endpoint for uploading a single file using multipart/form-data and model binding
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("UploadSingleFile")]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (file == null || file.ContentType.ToLower().StartsWith("image/"))
            {
                MemoryStream ms = new MemoryStream();
                file.OpenReadStream().CopyTo(ms);

                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                Models.Entities.Image imageEntity = new Image()
                {
                    Name = file.FileName,
                    Data = ms.ToArray(),
                    Length = (int)ms.Length/1024,   // KB
                    Width = image.Width,
                    Height = image.Height,
                    ContentType = file.ContentType
                };

                await _appDbContext.Images.AddAsync(imageEntity);
                await _appDbContext.SaveChangesAsync();
            }            

            //return Ok(new { count = 1, path = filePath });
            return Ok(new { count = 1 });
        }

        /// <summary>
        /// POST endpoint for uploading multiple files using multipart/form-data and model binding
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("UploadMultipleFilesCollection")]
        public async Task<IActionResult> Post(IFormFileCollection files)
        {
            // full size of all files
            long size = files.Sum(f => f.Length);
            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            await HandleMultipleFilesUpload(files, filePath);

            return Ok(new { count = files.Count, size, filePath });
        }

        /// <summary>
        /// POST endpoint for uploading multiple files using multipart/form-data and model binding
        /// BUG using postman or http.post results in a 400 Bad Request (The input was not valid)
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("UploadMultipleFilesList")]
        public async Task<IActionResult> Post(IEnumerable<IFormFile> files)
        {
            // full size of all files
            long size = files.Sum(f => f.Length);
            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            await HandleMultipleFilesUpload(files, filePath);

            return Ok(new { count = files.Count(), size, filePath });
        }

        /// <summary>
        /// Handles the file upload by saving data to a temporary location and providing a hook for
        /// processing the files
        /// </summary>
        /// <param name="files"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private async Task HandleMultipleFilesUpload(IEnumerable<IFormFile> files, string filePath)
        {
            // iterate over files and save them to disk
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process files
            await HandleProcessFilesUpload(files);
        }

        /// <summary>
        /// Handles the processing of uploaded files
        /// </summary>
        /// <param name="filePath"></param>
        private async Task HandleProcessFilesUpload(IEnumerable<IFormFile> files)
        {
            
        }        
    }
}