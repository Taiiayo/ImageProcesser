using ImageProcesser.Extensions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
        public IEnumerable<IActionResult> GetImages([FromBody] string receivedImageUrls)
        {
            if (!ModelState.IsValid)
                yield return BadRequest(ModelState.GetErrorMessages());

            List<byte[]> allImageBytes = new List<byte[]>();

            List<string> imageUrls = new List<string>();
            imageUrls.AddRange(receivedImageUrls.Split(','));
            foreach (var imageUrl in imageUrls)
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    yield return BadRequest(ModelState.GetErrorMessages());
                }

                string pathToImagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                using (WebClient webClient = new WebClient())
                {
                    var fullPathToImage = pathToImagesFolder + $"\\{Guid.NewGuid()}.png";
                    webClient.DownloadFile(imageUrl, fullPathToImage);
                    byte[] byteData = System.IO.File.ReadAllBytes(fullPathToImage);
                    if (byteData == null)
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    allImageBytes.Add(byteData);
                }                                               
            }
            foreach (var bytesArray in allImageBytes)
            {
                yield return File(bytesArray, "image/png");
            }
        }
    }
}
