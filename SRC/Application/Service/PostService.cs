using Application.Exceptions.Application.Exceptions;
using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;

namespace Application.Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IReactionRepository _reactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public PostService(IPostRepository postRepository, ICommentRepository commentRepository, IReactionRepository reactionRepository, IUserRepository userRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _reactionRepository = reactionRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<int> Create(CreatePostRequest request)
        {
            var post = _mapper.Map<Post>(request);
            return await _postRepository.Create(post);
        }

        public async Task<bool> Delete(int id)
        {
            var post = await _postRepository.GetById(id);
            if (post == null)
                throw new NotFoundApplicationException($"Post {id} not found");


            await _commentRepository.DeleteByPostId(id);

            await _reactionRepository.DeleteByPostId(id);

            return await _postRepository.Delete(id);
        }

        public async Task<IEnumerable<PostResponse>> GetAll()
        {
            var posts = await _postRepository.GetAll();
            return _mapper.Map<IEnumerable<PostResponse>>(posts);
        }

        public async Task<PostResponse> GetById(int id)
        {
            var post = await _postRepository.GetById(id);
            if (post == null)
                throw new NotFoundApplicationException($"Post {id} not found");

            return _mapper.Map<PostResponse>(post);
        }

        public async Task<bool> Update(UpdatePostRequest request)
        {
            var existingPost = await _postRepository.GetById(request.Id);
            if (existingPost == null)
                throw new NotFoundApplicationException($"Post {request.Id} not found");

            existingPost.Text = request.Text;
            return await _postRepository.Update(existingPost);
        }
    }
}
