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
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository, IReactionRepository reactionRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _reactionRepository = reactionRepository;
            _mapper = mapper;
        }

        public async Task<int> Add(CreateUserRequest request)
        {
            var user = new User()
            {
                Name = request.Name,
                LastName = request.LastName,
                Age = request.Age,
                Info = request.Info,
                Email = request.Email
            };
            return await _userRepository.Create(user);
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            await _postRepository.DeleteByUserId(id);

            await _commentRepository.DeleteByUserId(id);

            await _reactionRepository.DeleteByUserId(id);

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
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<bool> Update(UpdateUserRequest request)
        {
            var user = new User()
            {
                Id=request.Id,
                Name = request.Name,
                LastName = request.LastName,
                Age = request.Age,
                Info = request.Info,
                Email = request.Email
            };

            var existingUser = await _userRepository.GetById(user.Id);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            //_mapper.Map(user, existingUser);
            return await _userRepository.Update(user);
        }
    }
}
