using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Bloggie.Web.Repositories
{
    public class CloudinaryImageRepository : IImageRepository
    {
        //Injecting configuration object in cloudinary object
        private readonly IConfiguration _configuration;
        private readonly Account account;
        public CloudinaryImageRepository(IConfiguration configuration)
        {
            _configuration = configuration;

            //Initialize cloudinary account what it needs
            account = new Account(
                _configuration.GetSection("Cloudinary")["CloudName"],
                _configuration.GetSection("Cloudinary")["ApiKey"],
                _configuration.GetSection("Cloudinary")["ApiSecret"]
                );
        }
        public async Task<string?> UploadAsync(IFormFile file)
        {
            //Cloudinary class ke constructor mai account as a object paas krdiya mere account pe upload krdega cloudinary pe.
            var client = new Cloudinary(account);

            //We require some upload parameter to upload our image.
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                DisplayName = file.FileName
            };

            //Now upload method helps to upload the parameters of the image we set
            var uploadResult = await client.UploadAsync(uploadParams);

            if(uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }
            return null;
        }
    }
}
