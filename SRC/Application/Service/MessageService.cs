using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;

namespace Application.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<int> Create(CreateMessageRequest request)
        {
            var message = _mapper.Map<Message>(request);
            return await _messageRepository.Create(message);
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
        }

        public async Task<MessageResponse> GetById(int id)
        {
            var message = await _messageRepository.GetById(id);
            if (message == null)
                throw new NotFoundApplicationException($"Message {id} not found");

            return _mapper.Map<MessageResponse>(message);
        }
        
        public async Task<IEnumerable<MessageResponse>> GetAll()
        {
            var messages = await _messageRepository.GetAll();
            return _mapper.Map<IEnumerable<MessageResponse>>(messages);
        }

        public async Task<IEnumerable<MessageResponse>> GetByUserId(int userId)
        {
            var messages = await _messageRepository.GetByUserId(userId);
            return _mapper.Map<IEnumerable<MessageResponse>>(messages);
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
        }
    }
}
