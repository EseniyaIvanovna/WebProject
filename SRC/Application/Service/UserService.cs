using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IReactionRepository _reactionRepository;
        private readonly IInteractionRepository _interactionRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly NpgsqlConnection _connection;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository, 
            IPostRepository postRepository, 
            ICommentRepository commentRepository, 
            IMessageRepository messageRepository,
            IReactionRepository reactionRepository, 
            IInteractionRepository interactionRepository, 
            NpgsqlConnection connection, 
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _reactionRepository = reactionRepository;
            _interactionRepository = interactionRepository;
            _messageRepository = messageRepository;
            _connection = connection;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Add(CreateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            var userId = await _userRepository.Create(user);

            _logger.LogInformation(
                "User created with id {Id} and username {Username}",
                userId,
                request.Name);

            return userId;
        }

        public async Task Delete(int id)
        {
            await using var tran = await _connection.BeginTransactionAsync();

            try
            {
                await _commentRepository.DeleteByUserId(id);
                await _commentRepository.DeleteByPostOwnerId(id);
                await _reactionRepository.DeleteByUserId(id);
                await _reactionRepository.DeleteByPostOwnerId(id);
                await _interactionRepository.DeleteByUserId(id);
                await _messageRepository.DeleteMessagesByUser(id);
                await _postRepository.DeleteByUserId(id);


                var result = await _userRepository.Delete(id);

                if(result == false)
                {
                    throw new EntityDeleteException("User", id.ToString());
                }

                await tran.CommitAsync();

                _logger.LogInformation(
                    "User successfully deleted with id {Id} along with all related data",
                    id);
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<UserResponse>> GetAll()
        {
            var users = await _userRepository.GetAll();
            var responses = _mapper.Map<IEnumerable<UserResponse>>(users);

            _logger.LogInformation(
                "Retrieved {Count} users in total",
                responses.Count());

            return responses;
        }

        public async Task<UserResponse> GetById(int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
                throw new NotFoundApplicationException($"User {id} not found");

            var response = _mapper.Map<UserResponse>(user);

            _logger.LogInformation(
                "User retrieved with id {Id}",
                id);

            return response;
        }

        public async Task Update(UpdateUserRequest request)
        {
            var existingUser = await _userRepository.GetById(request.Id);
            if (existingUser == null)
                throw new NotFoundApplicationException($"User {request.Id} not found");

            _mapper.Map(request, existingUser);
            var result = await _userRepository.Update(existingUser);
            if(result == false)
            {
                throw new EntityUpdateException("User", request.Id.ToString());
            }

            _logger.LogInformation(
                "User updated with id {Id}",
                request.Id);
        }
    }
}
