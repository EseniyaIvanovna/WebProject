using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
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
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository, IMessageRepository messageRepository,
                             IReactionRepository reactionRepository, IInteractionRepository interactionRepository, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _reactionRepository = reactionRepository;
            _interactionRepository = interactionRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<int> Add(CreateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            return await _userRepository.Create(user);
        }

        public async Task Delete(int id)
        {
            var connectionString = _configuration.GetConnectionString("PostgresDb");

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var tran = await connection.BeginTransactionAsync();

            try
            {
                await _commentRepository.DeleteByUserId(id);
                await _reactionRepository.DeleteByUserId(id);
                await _reactionRepository.DeleteByPostOwnerId(id);
                await _postRepository.DeleteByUserId(id);
                await _interactionRepository.DeleteByUserId(id);
                await _messageRepository.DeleteMessagesByUser(id);

                var result = await _userRepository.Delete(id);

                if(result == false)
                {
                    throw new EntityDeleteException("User", id.ToString());
                }

                await tran.CommitAsync();
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
            return _mapper.Map<IEnumerable<UserResponse>>(users);
        }

        public async Task<UserResponse> GetById(int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
                throw new NotFoundApplicationException($"User {id} not found");

            return _mapper.Map<UserResponse>(user);
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
        }
    }
}
