using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers
{
    public class AdminBlogPostsController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        //Constructor Injection
        public AdminBlogPostsController(ITagRepository tagRepository ,IBlogPostRepository blogPostRepository)
        {
            _tagRepository = tagRepository;
            _blogPostRepository = blogPostRepository;
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        
        public async Task<IActionResult> Add() 
        {
            //Saray tags agaye yahan se repository le ayi database se hamare liye ye
            var tags = await _tagRepository.GetAllAsync();

            var model = new AddBlogPostRequest()
            {
                //Always take string value in linq

                Tags = tags.Select(x => new SelectListItem { Text = x.DisplayName , Value = x.Id.ToString()})

            };

            return View(model);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            //Map the view model to domain model a good practice

            var blogPost = new BlogPost()
            {
                Heading = addBlogPostRequest.Heading,
                PageTitle = addBlogPostRequest.PageTitle,
                Content = addBlogPostRequest.Content,
                ShortDescription = addBlogPostRequest.ShortDescription,
                FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible,

            };

            //Map Tags from selected tags crucial step iterate through all the selected tags we want to save in database.
            var selectedTags = new List<Tag>();

            foreach (var selectedTagId in addBlogPostRequest.SelectedTags)
            {
                //Basically jo tags humne select krke rkhe the na woh save tu id ki form mai hi horhe the tu
                //hamai ziada kuch krna nhi pra bas un arrat of strings ko guid banane ki koshish jri hai aur unki wohi id
                //se hum database se unhe uthane kis koshish krrhe

                var selectedTagAsGuid = Guid.Parse(selectedTagId);
                var existing = await _tagRepository.GetAsync(selectedTagAsGuid);
                
                if (existing != null)
                {
                    selectedTags.Add(existing);   
                }  
            }

            //Dalgye yahan pe saray selected tags asli walay domain model mai allah ka shukr
            blogPost.Tags = selectedTags; 

            await _blogPostRepository.AddAsync(blogPost);

            return RedirectToAction("Add");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> List()
        {

            //Call the repository to get all the blogpost

            var blogPosts = await _blogPostRepository.GetAllAsync();
            
            return View(blogPosts);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            // Retrieve the result from the repository
            var blogPost = await _blogPostRepository.GetAsync(Id);

            //Saaray tags dobara mangwao kyun ke edit krte waqt na janay kis tag ki zaroorat par jaye
            var tags = await _tagRepository.GetAllAsync();

            if(blogPost != null)
            {
                //Mapping domain mdel to view model and pass that model to view
                var editBlogPostRequest = new EditBlogPostRequest()
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    ShortDescription = blogPost.ShortDescription,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    PublishedDate = blogPost.PublishedDate,
                    Author = blogPost.Author,
                    Visible = blogPost.Visible,
                    //Saaray tags dobara mangwao kyun ke edit krte waqt na janay kis tag ki zaroorat par jaye
                    Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }),

                    //Konse tags selected the ye sirf user ko batana hai it is not a necessary step
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };
                return View(editBlogPostRequest);
            }
            else
            {
                return View(null);
            }
        }

        [Authorize(Roles = "Admin")]
        //Method overloading. It is an HTTP POST method.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            //Map view model to domain model again after done editing
            var blogPostDomainModel = new BlogPost()
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                Content = editBlogPostRequest.Content,
                PageTitle = editBlogPostRequest.PageTitle,
                ShortDescription = editBlogPostRequest.ShortDescription,
                FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
                UrlHandle = editBlogPostRequest.UrlHandle,
                PublishedDate = editBlogPostRequest.PublishedDate,
                Author = editBlogPostRequest.Author,
                Visible = editBlogPostRequest.Visible
            };

            //Map Tags from selected tags crucial step iterate through all the selected tags we want to save in database.
            var selectedTags = new List<Tag>();

            foreach (var selectedTagsinBlogPost in editBlogPostRequest.SelectedTags)
            {
                //Parse into Guid because it comes in the value which is in string

                var selectedTagAsGuid = Guid.Parse(selectedTagsinBlogPost);

                var existing = await _tagRepository.GetAsync(selectedTagAsGuid);

                if (existing != null)
                {
                    selectedTags.Add(existing);
                }
            }

            blogPostDomainModel.Tags = selectedTags;

            var addedinBlogPost = await _blogPostRepository.UpdateAsync(blogPostDomainModel);

            if(addedinBlogPost != null)
            {
                return RedirectToAction("Edit", new {id = editBlogPostRequest.Id});
            }
            else
            {
                return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            var founded = await _blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

            if(founded != null)
            {
                //Show success notification
                return RedirectToAction("List");
            }
            else
            {
                //Show failure notification
                return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
            }
        } 
    }
}
