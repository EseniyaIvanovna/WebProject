using Application.Exceptions.Application.Exceptions;
using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;

namespace Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IReactionRepository _reactionRepository;
        private readonly IInteractionRepository _interactionRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository,
            IReactionRepository reactionRepository, IInteractionRepository interactionRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _reactionRepository = reactionRepository;
            _interactionRepository = interactionRepository;
            _mapper = mapper;
        }
        public async Task<int> Add(CreateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            return await _userRepository.Create(user);
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
                throw new NotFoundApplicationException($"User {id} not found");


            await _postRepository.DeleteByUserId(id);

            await _commentRepository.DeleteByUserId(id);

            await _reactionRepository.DeleteByUserId(id);
            await _interactionRepository.DeleteByUserId(id);

            return await _userRepository.Delete(id);
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

        public async Task<bool> Update(UpdateUserRequest request)
        {
            var existingUser = await _userRepository.GetById(request.Id);
            if (existingUser == null)
                throw new NotFoundApplicationException($"User {request.Id} not found");

            _mapper.Map(request, existingUser);
            return await _userRepository.Update(existingUser);
        }
    }
}
