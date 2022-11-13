using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user/{userId}/photos")]
    [ServiceFilter(typeof(LogUserLastActive))]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
           _repo = repo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
                );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}",Name ="GetUserPhoto")]
        public async Task<IActionResult> GetUserPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photoMapped = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photoMapped);

        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoCreation)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo =await _repo.GetUser(userId);

            var file = photoCreation.File;
            var uploadResult = new ImageUploadResult();
            if (file !=null)
            {
                if(file.Length > 0)
                {
                    using(var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.Name, stream),
                            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                    }
                   

                }
            }

            photoCreation.Url = uploadResult.Url.ToString();
            photoCreation.PublicId = uploadResult.PublicId;

            var photoMapped = _mapper.Map<Photo>(photoCreation);
            if (!userFromRepo.Photos.Any(u => u.IsMain))
                photoMapped.IsMain = true;

            userFromRepo.Photos.Add(photoMapped);

            if (await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photoMapped);
                return CreatedAtRoute("GetUserPhoto", new { userId , id = photoToReturn.Id }, photoToReturn);
            }

            return BadRequest("Could not add the Photo");
        }

        [HttpPost("{id}/isMain")]
        public async Task<IActionResult> SetMainPhoto(int userId,int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("The selected photo is the current profile picture");
            var currentMainUserPhoto = await _repo.GetCurrentMainUserPhoto(userId);
            currentMainUserPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

           


            if (await _repo.SaveAll())
                return NoContent();

            return BadRequest("Your profile picture update was unsuccessful");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("You cannot delete your profile picture");

            if(photoFromRepo.PublicId != null)
            {
                var deletionParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deletionParams);

                if(result.Result == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }

            if(photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Photo could not be deleted");
        }
    }
} 
