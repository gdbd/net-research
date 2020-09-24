using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebUploader
{
    public class UploadHandler : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                HttpPostedFile file = context.Request.Files[0];
                file.SaveAs(@"c:\temp\a.file");
            }
            catch (Exception ex)
            {
                // include your custom logging code
                // Log.Error(ex.Message, ex);
                throw;
            }
        }
    }
}
