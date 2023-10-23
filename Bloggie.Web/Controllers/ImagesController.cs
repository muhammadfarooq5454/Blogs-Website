using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Bloggie.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        
        //Inject the images repository
        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        /* Method to upload images */
        [HttpPost]
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {
            // call a repository

            var imageurl = await _imageRepository.UploadAsync(file);

            if(imageurl == null) {
                //If no url comes back from cloudinary
                return Problem("Something went wrong!!!", null , (int)HttpStatusCode.InternalServerError);
            }

            //Converts the result to json format
            var jsonResult = new JsonResult(new { link = imageurl } );

            return jsonResult; 
        } 
    }
}
