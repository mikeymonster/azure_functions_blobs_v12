using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobStorageV12ConsoleApp.Model
{
    public class BlobInfoMessage
    {
        public string ContainerName { get; set; }
        public string FileName { get; set; }
        public string ETag { get; set; }
        public string VersionId { get; set; }
    }
}
