using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using robstagram.Extensions;
using robstagram.Helpers;

namespace robstagram.Controllers
{
    [SwaggerIgnore]
    [Authorize(Policy = "ApiUser")]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : Controller
    {
        #region Variables
        private readonly IHostingEnvironment _hostingEnvironment;
        #endregion

        #region Constructors
        public UploadController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        } 
        #endregion

        /// <summary>
        /// POST endpoint for uploading a single file using multipart/form-data and model binding
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>       
        [HttpPost("single")]
        public async Task<ActionResult<JsonResult>> PostSingle(IFormFile file)
        {
            if (file == null || !file.ContentType.ToLower().StartsWith("image/"))
            {
                return BadRequest(ModelState);
            }

            var uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, Configuration.UploadFolder);
            var filePath = Path.Combine(uploadFolder, Path.GetRandomFileName());
            filePath = Path.ChangeExtension(filePath, "jpg");
            var relativePath = Path.GetRelativePath(_hostingEnvironment.WebRootPath, filePath);
            var fileSize = file.Length;

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            MemoryStream ms = new MemoryStream();
            file.OpenReadStream().CopyTo(ms);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            image.Resize(640, 640).Save(filePath);

            //if (file.Length > 0)
            //{
            //    using (var stream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await file.CopyToAsync(stream);
            //    }
            //}                

            return Ok(new
            {
                count = 1 ,
                path = relativePath,
                size = fileSize
            });
        }

        /// <summary>
        /// POST endpoint for uploading multiple files using multipart/form-data and model binding
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("multiple")]
        public async Task<ActionResult<JsonResult>> PostMultiple(IFormFileCollection files)
        {
            // full size of all files
            long size = files.Sum(f => f.Length);
            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            var response = HandleMultipleFilesUpload(files);

            return Ok(response);
        }

        /// <summary>
        /// POST endpoint for uploading a file in base64  format
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        [HttpPost("base64")]
        public async Task<ActionResult<JsonResult>> PostBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return BadRequest(ModelState);
            }

            var bytes = Convert.FromBase64String(base64);

            var uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, Configuration.UploadFolder);
            var filePath = Path.Combine(uploadFolder, Path.GetRandomFileName());
            filePath = Path.ChangeExtension(filePath, "jpg");
            var relativePath = Path.GetRelativePath(_hostingEnvironment.WebRootPath, filePath);
            var fileSize = bytes.Length;

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var ms = new MemoryStream(bytes);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            image.Resize(640, 640).Save(filePath);

            return Ok(new
            {
                count = 1,
                path = relativePath,
                size = fileSize
            });
        }

        /// <summary>
        /// Handles the file upload by saving data to a temporary location
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private object HandleMultipleFilesUpload(IEnumerable<IFormFile> files)
        {
            var count = 0;
            var paths = new List<string>();
            var sizes = new List<long>();

            var uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, Configuration.UploadFolder);

            // iterate over files and save them to disk
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var filePath = Path.Combine(uploadFolder, Path.GetRandomFileName());
                    filePath = Path.ChangeExtension(filePath, "jpg");
                    var relativePath = Path.GetRelativePath(_hostingEnvironment.WebRootPath, filePath);
                    var fileSize = file.Length;

                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    MemoryStream ms = new MemoryStream();
                    file.OpenReadStream().CopyTo(ms);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                    image.Resize(640, 640).Save(filePath);

                    count++;
                    paths.Add(relativePath);
                    sizes.Add(fileSize);
                }
            }

            return new
            {
                count = count,
                paths = paths,
                sizes = sizes
            };
        }     
    }
}