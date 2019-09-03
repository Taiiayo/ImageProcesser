using ImageProcesser.Extensions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using FromBody = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace ImageProcesser.Controllers
{
    [EnableCors("MyPolicy")]
    [Microsoft.AspNetCore.Mvc.Route("/api/[controller]")]
    public class ImageController : Controller
    {
        [HttpPost("GetImages")]
        public IEnumerable<IActionResult> GetImages([FromBody] string query)
        {
            if (!ModelState.IsValid)
                yield return BadRequest(ModelState.GetErrorMessages());
            List<byte[]> allBytes = new List<byte[]>();
            List<string> urls = new List<string>();
            urls.AddRange(query.Split(','));
            foreach (var imageUrl in urls)
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    yield return BadRequest(ModelState.GetErrorMessages());
                }
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");


                WebClient webClient = new WebClient();
                var path = fullPath + $"\\{Guid.NewGuid()}.png";
                webClient.DownloadFile(imageUrl, path);

                byte[] byteData = System.IO.File.ReadAllBytes(path);

                if (byteData == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                allBytes.Add(byteData);
                
            }
            foreach(var b in allBytes)
            {
                yield return File(b, "image/png");
            }
        }
    }
}
