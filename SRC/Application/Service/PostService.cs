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
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IReactionRepository _reactionRepository;
        private readonly IUserService _userService;
        private readonly NpgsqlConnection _connection;
        private readonly IMapper _mapper;
        private readonly ILogger<PostService> _logger;

        public PostService(
            IPostRepository postRepository, 
            ICommentRepository commentRepository, 
            IReactionRepository reactionRepository,
            IUserService userService,
            NpgsqlConnection connection, 
            IMapper mapper,
            ILogger<PostService> logger)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _reactionRepository = reactionRepository;
            _userService = userService;
            _connection = connection;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Create(CreatePostRequest request)
        {
            var user = await _userService.GetById(request.UserId);
            if (user == null)
                throw new NotFoundApplicationException($"User {request.UserId} not found");

            var post = _mapper.Map<Post>(request);
            var postId = await _postRepository.Create(post);

            _logger.LogInformation(
                "Post created with id {Id} by user {UserId}",
                postId,
                request.UserId);

            return postId;
        }

        public async Task Delete(int id)
        {
            await using var tran = await _connection.BeginTransactionAsync();

            try
            {
                await _commentRepository.DeleteByPostId(id);
                await _reactionRepository.DeleteByPostId(id);

                var result = await _postRepository.Delete(id);
                if(result == false)
                {
                    throw new EntityDeleteException("Post", id.ToString());
                }

                await tran.CommitAsync();

                _logger.LogInformation(
                    "Post successfully deleted with id {Id} along with its comments and reactions",
                    id);
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<PostResponse>> GetAll()
        {
            var posts = await _postRepository.GetAll();
            var responses = _mapper.Map<IEnumerable<PostResponse>>(posts);

            _logger.LogInformation(
                "Retrieved {Count} posts in total",
                responses.Count());

            return responses;
        }

        public async Task<PostResponse?> GetById(int id)
        {
            var post = await _postRepository.GetById(id);
            if (post == null)
                throw new NotFoundApplicationException($"Post {id} not found");

            var response = _mapper.Map<PostResponse>(post);

            _logger.LogInformation(
                "Post retrieved with id {Id}",
                id);

            return response;
        }

        public async Task Update(UpdatePostRequest request)
        {
            var existingPost = await _postRepository.GetById(request.Id);
            if (existingPost == null)
                throw new NotFoundApplicationException($"Post {request.Id} not found");

            existingPost.Text = request.Text;
            var result = await _postRepository.Update(existingPost);

            if(result == false)
            {
                throw new EntityUpdateException("Post", request.Id.ToString());
            }

            _logger.LogInformation(
                "Post updated with id {Id}",
                request.Id);
        }
    }
}
