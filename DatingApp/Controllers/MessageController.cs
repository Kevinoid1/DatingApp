using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [ServiceFilter(typeof(LogUserLastActive))]
    public class MessageController : ControllerBase
    {
        private readonly IDatingRepository datingRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;

        public MessageController(IDatingRepository _datingRepository, IMessageRepository _messageRepository, IMapper _mapper)
        {
            datingRepository = _datingRepository;
            messageRepository = _messageRepository;
            mapper = _mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto entity)
        {
            var username = User.GetUsername();

            if (username.ToLower() == entity.RecipientUsername.ToLower()) return BadRequest("You cannot send messages to yourself");

            var sender = await datingRepository.GetUserByUsernameAsync(username);
            var recipient = await datingRepository.GetUserByUsernameAsync(entity.RecipientUsername);

            if(recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.Username,
                RecipientUsername = recipient.Username,
                Content = entity.Content
            };

            messageRepository.AddMessage(message);

            if(await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetUserMessages([FromQuery]MessageParams messageParams)
        {
            messageParams.Username= User.GetUsername();

            var messages = await messageRepository.GetUserMessagesAsync(messageParams);

            Response.AddPagination(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientUsername}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string recipientUsername)
        {
            var currentUsername = User.GetUsername();

            var threadMessages = await messageRepository.GetMessageThreadAsync(currentUsername, recipientUsername);

            return Ok(threadMessages);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await messageRepository.GetMessageAsync(id);

            if (message == null) return BadRequest();

            if(message.SenderUsername != username && message.RecipientUsername != username) return Unauthorized();

            if(message.SenderUsername == username) message.SenderDeleted = true;

            if(message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted) messageRepository.RemoveMessage(message);

            if (await messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete message");
        }
    }
}
