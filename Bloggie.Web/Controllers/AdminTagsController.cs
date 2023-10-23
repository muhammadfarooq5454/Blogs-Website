using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    public class AdminTagsController : Controller
    {
        private readonly ITagRepository _tagRepository;

        //Constructor Injection
        public AdminTagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        //Block the access to other users give only to SuperAdmin
        [Authorize(Roles = "User")]       
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ActionName("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            ValidateAddTagRequest(addTagRequest);
            if(ModelState.IsValid == false)
            {
                return View();
            }
            //Model Binding
            // var name = addTagRequest.Name;
            // var displayName = addTagRequest.DisplayName;

            //Mapping AddTagRequest to Tag domain Model
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            }; 

            //Job of a repository to save changes to database not controller
            await _tagRepository.AddAsync(tag);

            return RedirectToAction("List");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        //Name of the method is the action name itself
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            // Use Dbcontext to read the tags
            var tags = await _tagRepository.GetAllAsync();

            return View(tags);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);

            if(tag != null)
            {
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName =  tag.DisplayName
                };

                return View(editTagRequest);
            }
            return View(null);
        }

        [Authorize(Roles = "Admin")]
        //Method overloading. It is an HTTP POST method.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            //Again using Model Binding to save changes in the database using dbcontext class. Simply converting view models into domain models

            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };

            var updatedTag = await _tagRepository.UpdateAsync(tag);

            if(updatedTag != null)
            {
                //Show Success Notification
                //This is the way to pass parameter to the function when called in view like passing the object
                return RedirectToAction("Edit", new { id = editTagRequest.Id });
            }
            else
            {
                //Show Failure Notification
                return RedirectToAction("Edit", new { id = editTagRequest.Id });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {

            var deletedTag = await _tagRepository.DeleteAsync(editTagRequest.Id);

            if(deletedTag != null)
            {                
                //Show success notification
                return RedirectToAction("List");
            }
            else
            {
                //Show failure notification
                return RedirectToAction("Edit", new { id = editTagRequest.Id });
            }
        }
        private void ValidateAddTagRequest(AddTagRequest addTagRequest)
        {
            if(addTagRequest.Name is not null && addTagRequest.DisplayName != null)
            {
                if(addTagRequest.Name == addTagRequest.DisplayName)
                {
                    ModelState.AddModelError("DisplayName", "Name cannot be the same as DisplayName");
                }
            }
        }
    }
}
