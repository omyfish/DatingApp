using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repository = repo;

            var acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id) {
            var photoFromRepo = await _repository.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoForCreationDto) {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            var userFromRepo = await _repository.GetUser(userId);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0) {
                using (var stream = file.OpenReadStream()) {
                    var uploadParams = new ImageUploadParams{
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!userFromRepo.Photos.Any(u => u.IsMain)) {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if (await _repository.SaveAll()) 
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id}, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhotoForUser(int userId, int id) {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            var userFromRepo = await _repository.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.Id == id)) {
                return Unauthorized();
            }

            var photo = await _repository.GetPhoto(id);

            if (photo.IsMain){
                return BadRequest("This is already the main photo");
            }

            var currentMainPhoto = await _repository.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photo.IsMain = true;
            
            if (await _repository.SaveAll()) 
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return NoContent();
            }

            return BadRequest("Could not add the photo");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id) {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            var userFromRepo = await _repository.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.Id == id)) {
                return Unauthorized();
            }

            var photo = await _repository.GetPhoto(id);

             if (photo.IsMain){
                return BadRequest("Can't delete Main photo");
            }

            if (photo.PublicId != null) {
                var deleteParams = new DeletionParams(photo.PublicId);

                var result =_cloudinary.Destroy(deleteParams);

                if (result.Result == "ok") {
                    _repository.Delete(photo);
                
                    if (await _repository.SaveAll()) 
                    {
                        return Ok();
                    }
                }   
            } 
            else 
            {
                    _repository.Delete(photo);
                
                    if (await _repository.SaveAll()) 
                    {
                        return Ok();
                    }
            }

            return BadRequest("Could not delete the photo");
        }
    }
}