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
            var sender = await _userRepository.GetById(request.SenderId);
            if (sender == null)
            {
                throw new InvalidOperationException("Sender not found.");
            }

            var receiver = await _userRepository.GetById(request.ReceiverId);
            if (receiver == null)
            {
                throw new InvalidOperationException("Receiver not found.");
            }

            var message = new Message()
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Text = request.Text
            };
            return await _messageRepository.Create(message);
        }

        public async Task<bool> Delete(int id)
        {
            var message = await _messageRepository.GetById(id);
            if (message == null)
            {
                throw new InvalidOperationException("Message not found.");
            }

            return await _messageRepository.Delete(id);
        }

        public async Task<MessageResponse> GetById(int id)
        {
            var message = await _messageRepository.GetById(id);
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
            {
                throw new InvalidOperationException("Message not found.");
            }

            var message = new Message()
            {
                Id = request.Id,
                Text = request.Text
            };
            return await _messageRepository.Update(message);
        }
    }
}
