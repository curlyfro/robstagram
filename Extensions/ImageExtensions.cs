using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace robstagram.Extensions
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Resize an image to new dimensions.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static Image Resize(this Image current, int maxWidth, int maxHeight)
        {
            int width, height;

            if (current.Width > current.Height)
            {
                width = maxWidth;
                height = Convert.ToInt32(current.Height * maxHeight / (double) current.Width);
            }
            else
            {
                width = Convert.ToInt32(current.Width * maxWidth / (double) current.Height);
                height = maxHeight;
            }

            var canvas = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(canvas))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(current, 0, 0, width, height);
            }

            return canvas;
        }

        /// <summary>
        /// Convert Image object to byte array.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Image current)
        {
            using (var stream = new MemoryStream())
            {
                current.Save(stream, current.RawFormat);
                return stream.ToArray();
            }
        }
    }
}
