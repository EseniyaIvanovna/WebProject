using Application.Exceptions.Application.Exceptions;
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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<int> Create(CreateMessageRequest request)
        {
            var message = _mapper.Map<Message>(request);
            return await _messageRepository.Create(message);
        }

        public async Task<bool> Delete(int id)
        {
            var message = await _messageRepository.GetById(id);
            if (message == null)
                throw new NotFoundApplicationException($"Message {id} not found");

            return await _messageRepository.Delete(id);
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

        public async Task<bool> Update(UpdateMessageRequest request)
        {
            var existingMessage = await _messageRepository.GetById(request.Id);
            if (existingMessage == null)
                throw new NotFoundApplicationException($"Message {request.Id} not found");

            existingMessage.Text = request.Text;
            return await _messageRepository.Update(existingMessage);
        }
    }
}
