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
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IReactionRepository _reactionRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PostService(IPostRepository postRepository, ICommentRepository commentRepository, IReactionRepository reactionRepository, IMapper mapper, IConfiguration configuration)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _reactionRepository = reactionRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<int> Create(CreatePostRequest request)
        {
            var post = _mapper.Map<Post>(request);
            return await _postRepository.Create(post);
        }

        public async Task Delete(int id)
        {
            var connectionString = _configuration.GetConnectionString("PostgresDb");

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var tran = await connection.BeginTransactionAsync();

            try
            {
                await _commentRepository.DeleteByPostId(id);
                await _reactionRepository.DeleteByPostId(id);

                await _postRepository.Delete(id);

                await tran.CommitAsync();
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
            return _mapper.Map<IEnumerable<PostResponse>>(posts);
        }

        public async Task<PostResponse> GetById(int id)
        {
            var post = await _postRepository.GetById(id);
            if (post == null)
                throw new NotFoundApplicationException($"Post {id} not found");

            return _mapper.Map<PostResponse>(post);
        }

        public async Task Update(UpdatePostRequest request)
        {
            var existingPost = await _postRepository.GetById(request.Id);
            if (existingPost == null)
                throw new NotFoundApplicationException($"Post {request.Id} not found");

            existingPost.Text = request.Text;
            await _postRepository.Update(existingPost);
        }
    }
}
