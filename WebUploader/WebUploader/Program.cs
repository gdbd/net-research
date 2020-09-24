namespace WebUploader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {

          //  UploadFilesToServer("http://localhost:8040/upload.ashx", null, "text.txt", "application/octet-stream", new byte[] { 14, 24 , 36 });

            //await Upload("http://localhost:8040/upload.ashx", "a",)

            using (var client = new WebClient())
            {
                // client.UploadFile("http://localhost:8040/upload.ashx", @"c:\temp\SW_DVD5_Project_Pro_2016_64Bit_Russian_MLF_X20-42702.zip");
                var w = client.OpenWrite("http://localhost:8040/upload.ashx");

                var f = File.OpenRead(@"c:\temp\SW_DVD5_Project_Pro_2016_64Bit_Russian_MLF_X20-42702.zip");

                using (StreamReader streamReader = new StreamReader(f))
                {
                   
                }
            }

            Console.Read();
        }

        private async Task<System.IO.Stream> Upload(string actionUrl, string paramString, Stream paramFileStream, byte[] paramFileBytes)
        {
            HttpContent stringContent = new StringContent(paramString);
            HttpContent fileStreamContent = new StreamContent(paramFileStream);
            HttpContent bytesContent = new ByteArrayContent(paramFileBytes);
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(stringContent, "param1", "param1");
                formData.Add(fileStreamContent, "file1", "file1");
                formData.Add(bytesContent, "file2", "file2");
                var response = await client.PostAsync(actionUrl, formData);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                return await response.Content.ReadAsStreamAsync();
            }
        }


        private static void UploadFilesToServer(string uri, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData)
        {
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";

            var req = httpWebRequest.GetRequestStream();

            WriteToStream(req, fileData);

            var resp = httpWebRequest.GetResponse();


            return;


            /*
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";            

            var ar = httpWebRequest.BeginGetRequestStream((result) =>
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                    using (Stream requestStream = request.EndGetRequestStream(result))
                    {
                        WriteMultipartForm(requestStream, boundary, data, fileName, fileContentType, fileData);
                    }
                    request.BeginGetResponse(a =>
                    {
                        try
                        {
                            var response = request.EndGetResponse(a);
                            var responseStream = response.GetResponseStream();
                            using (var sr = new StreamReader(responseStream))
                            {
                                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                                {
                                    string responseString = streamReader.ReadToEnd();
                                    //responseString is depend upon your web service.
                                    if (responseString == "Success")
                                    {
                                        Console.WriteLine("Backup stored successfully on server.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error occurred while uploading backup on server.");
                                    }
                                }
                            }
                        }
                        catch (Exception x)
                        {
                            throw;
                        }
                    }, null);
                }
                catch (Exception)
                {
                    throw;
                }
            }, httpWebRequest);*/
        }

        private static void WriteMultipartForm(Stream s, string boundary, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData)
        {
            /// The first boundary
            byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            /// the last boundary.
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "–-\r\n");
            /// the form data, properly formatted
            string formdataTemplate = "Content-Dis-data; name=\"{0}\"\r\n\r\n{1}";
            /// the form-data file upload, properly formatted
            string fileheaderTemplate = "Content-Dis-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";

            /// Added to track if we need a CRLF or not.
            bool bNeedsCRLF = false;

            if (data != null)
            {
                foreach (string key in data.Keys)
                {
                    /// if we need to drop a CRLF, do that.
                    if (bNeedsCRLF)
                        WriteToStream(s, "\r\n");

                    /// Write the boundary.
                    WriteToStream(s, boundarybytes);

                    /// Write the key.
                    WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                    bNeedsCRLF = true;
                }
            }

            /// If we don't have keys, we don't need a crlf.
            if (bNeedsCRLF)
                WriteToStream(s, "\r\n");

            WriteToStream(s, boundarybytes);
            WriteToStream(s, string.Format(fileheaderTemplate, "file", fileName, fileContentType));
            /// Write the file data to the stream.
            WriteToStream(s, fileData);
            WriteToStream(s, trailer);
        }

        private static void WriteToStream(Stream s, string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(bytes, 0, bytes.Length);
        }

        private static void WriteToStream(Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }
    }
}