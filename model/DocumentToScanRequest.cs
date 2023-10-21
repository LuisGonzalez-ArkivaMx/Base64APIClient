using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base64ApiClient.model
{
    public class DocumentToScanRequest
    {
        public string originalFileName { get; set; }
        public string document { get; set; }

    }
}
