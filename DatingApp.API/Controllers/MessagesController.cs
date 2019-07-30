using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/users/{userId}/[controller]")]
    [Authorize]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        IDatingRepository _repository;
        private readonly IMapper _mapper;

        public MessagesController(IDatingRepository repository, IMapper mapper) {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id) 
        { 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            var messsageFromRepo = await _repository.GetMessage(id);

            if (messsageFromRepo == null) 
            {
                return NotFound();
            }

            return Ok(messsageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageForUser(int userId, [FromQuery] MessageParams messageParams) 
        { 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            messageParams.UserId = userId;

            var messsageFromRepo = await _repository.GetMessageForUser(messageParams);

            if (messsageFromRepo == null) 
            {
                return NotFound();
            }

            var messageToReturn = _mapper.Map<IEnumerable<MessageToReturnDto>>(messsageFromRepo);

            Response.AddPagination(messsageFromRepo.CurrentPage, messsageFromRepo.PageSize, messsageFromRepo.TotalCount, messsageFromRepo.TotalPages);

            return Ok(messageToReturn);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId) 
        { 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            var messsageFromRepo = await _repository.GetMessageThread(userId, recipientId);

            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messsageFromRepo);

            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto) 
        { 
            var sender = await _repository.GetUser(userId);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            messageForCreationDto.SenderId = userId;

            var recipient = await _repository.GetUser(messageForCreationDto.RecipientId);

            if (recipient == null) 
            {
                return BadRequest("User not found");
            }

            var message = _mapper.Map<Message>(messageForCreationDto);

            _repository.Add(message);

            if (await _repository.SaveAll()) 
            {
                var messageForReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new { id = message.Id}, messageForReturn);
            }

            throw new Exception("Creating the message failed on sent");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId) {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }
            
            var messageFromRepo = await _repository.GetMessage(id);

            if (messageFromRepo.SenderId == userId)
                messageFromRepo.SenderDeleted = true;

            if (messageFromRepo.RecipientId == userId)
                messageFromRepo.RecipientDeleted = true;

            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                _repository.Delete(messageFromRepo);

            if (await _repository.SaveAll())
                return NoContent();
            
            return BadRequest("Failed to delete message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id) 
        {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                    return Unauthorized();
                }
                
                var messageFromRepo = await _repository.GetMessage(id);

                if(messageFromRepo.RecipientId != userId)
                return Unauthorized();

                messageFromRepo.IsRead = true;
                messageFromRepo.DateRead = DateTime.Now;

                await _repository.SaveAll();

                return NoContent();
        } 
    }
}