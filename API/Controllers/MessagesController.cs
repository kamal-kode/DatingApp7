using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MessagesController : BaseAPIController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(MessageDto createMessageDto)
    {
        var userName = User.GetUserName();

        if (userName == createMessageDto.RecipientUserName.ToLower())
            return BadRequest("You can not send message to yourself");

        var sender = await _unitOfWork.UserRepository.GetUserByUserNameAsync(userName);
        var recipient = await _unitOfWork.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUserName);

        if (recipient == null)
            return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUserName = sender.UserName,
            RecipientUserName = recipient.UserName,
            Content = createMessageDto.Content
        };

        _unitOfWork.MessageRepository.AddMessage(message);

        if (await _unitOfWork.Complete())
            return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message");
    }
    
    [HttpGet]
    public async Task<ActionResult<PageList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
    {
        messageParams.UserName = User.GetUserName();

        var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetMessageThread(string username){
        var currentUserName = User.GetUserName();
        return Ok(await _unitOfWork.MessageRepository.GetMessageThread(currentUserName, username));
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id){
        var username= User.GetUserName();
        var message = await _unitOfWork.MessageRepository.GetMessage(id);

        if(message.SenderUserName != username && message.RecipientUserName != username)
        return Unauthorized();

        if(message.SenderUserName == username) 
        message.SenderDeleted = true;
        
        if(message.RecipientUserName == username) 
        message.RecipientDeleted = true;
        
        if(message.SenderDeleted && message.RecipientDeleted)
        {
            _unitOfWork.MessageRepository.DeleteMessage(message);
        }

        if(await _unitOfWork.Complete())
        return Ok();

        return BadRequest("Problem deleting message");
    }
}
