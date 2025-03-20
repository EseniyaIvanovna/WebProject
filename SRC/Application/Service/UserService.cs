using Application.Dto;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<int> Add(UserDto user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            var existingUser = await _userRepository.GetById(user.Id);
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with the same Id already exists.");
            }
            var mappedUser = _mapper.Map<User>(user);
            int userId = await _userRepository.Create(mappedUser);
            return userId;
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

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            var users = await _userRepository.GetAll();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetById(int id)
        {
            var user = await _userRepository.GetById(id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> Update(UserDto user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            var existingUser = await _userRepository.GetById(user.Id);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            _mapper.Map(user, existingUser);
            return await _userRepository.Update(existingUser);
        }
    }
}
