
using System.Security.AccessControl;

namespace robstagram.Models.Entities
{
    public class Image
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Url { get; set; }

        public byte[] Data { get; set; }
        public long Size { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public string ContentType { get; set; }
    }
}