using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MessageService> _logger;

        public MessageService(IMessageRepository messageRepository, IMapper mapper, ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Create(CreateMessageRequest request)
        {
            var message = _mapper.Map<Message>(request);
            var messageId = await _messageRepository.Create(message);

            _logger.LogInformation(
                "Message created with id {Id} from user {FromUserId} to user {ToUserId}",
                messageId,
                request.FromUserId,
                request.ToUserId);

            return messageId;
        }

        public async Task Delete(int id)
        {
            var message = await _messageRepository.GetById(id);
            if (message == null)
                throw new NotFoundApplicationException($"Message {id} not found");

            var result = await _messageRepository.Delete(id);
            if(result == false)
            {
                throw new EntityDeleteException("Message", id.ToString());
            }

            _logger.LogInformation(
                "Message successfully deleted with id {Id}",
                id);
        }

        public async Task<MessageResponse> GetById(int id)
        {
            var message = await _messageRepository.GetById(id);
            if (message == null)
                throw new NotFoundApplicationException($"Message {id} not found");

            var response = _mapper.Map<MessageResponse>(message);

            _logger.LogInformation(
                "Message retrieved with id {Id}",
                id);

            return response;
        }
        
        public async Task<IEnumerable<MessageResponse>> GetAll()
        {
            var messages = await _messageRepository.GetAll();
            var responses = _mapper.Map<IEnumerable<MessageResponse>>(messages);

            _logger.LogInformation(
                "Retrieved {Count} messages in total",
                responses.Count());

            return responses;
        }

        public async Task<IEnumerable<MessageResponse>> GetByUserId(int userId)
        {
            var messages = await _messageRepository.GetByUserId(userId);
            var responses = _mapper.Map<IEnumerable<MessageResponse>>(messages);

            _logger.LogInformation(
                "Retrieved {Count} messages for user {UserId}",
                responses.Count(),
                userId);

            return responses;
        }

        public async Task Update(UpdateMessageRequest request)
        {
            var existingMessage = await _messageRepository.GetById(request.Id);
            if (existingMessage == null)
                throw new NotFoundApplicationException($"Message {request.Id} not found");

            existingMessage.Text = request.Text;
            var result = await _messageRepository.Update(existingMessage);

            if(result == false)
            {
                throw new EntityUpdateException("Message", request.Id.ToString());
            }

            _logger.LogInformation(
                "Message updated with id {Id}",
                request.Id);
        }
    }
}
