using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly ILogger<CommentService> _logger;

        public CommentService(ICommentRepository commentRepository, IPostService postService, IMapper mapper, ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _postService = postService;
            _mapper = mapper;
            _logger = logger;
        }
       
        public async Task<int> Create(CreateCommentRequest request)
        {
            var post = await _postService.GetById(request.PostId);
            if (post == null)
                throw new NotFoundApplicationException($"Post {request.PostId} not found");

            var comment = _mapper.Map<Comment>(request);
            var commentId = await _commentRepository.Create(comment);

            _logger.LogInformation(
                "Comment created with id {Id} with text {Content} by user {UserId} to post {PostId}",
                commentId,
                request.Content,
                comment.UserId,
                comment.PostId);

            return commentId;
        }
        
        public async Task Update(UpdateCommentRequest request)
        {
            var existingComment = await _commentRepository.GetById(request.Id);
            if (existingComment == null)
                throw new NotFoundApplicationException($"Comment {request.Id} not found");

            existingComment.Content = request.Content;
            var result = await _commentRepository.Update(existingComment);

            _logger.LogInformation(
                "Comment updated with id {Id} with text {Content} by user {UserId} to post {PostId}",
                request.Id,
                request.Content,
                existingComment.UserId,
                existingComment.PostId);

            if (result == false)
            {
                throw new EntityUpdateException("Comment", request.Id.ToString());
            }

            _logger.LogInformation(
                "Comment successfully updated with id {Id}",
                request.Id);
        }

        public async Task Delete(int id)
        {
            var comment = await _commentRepository.GetById(id);
            if (comment == null)
                throw new NotFoundApplicationException($"Comment {id} not found");

            var result = await _commentRepository.Delete(id);
            if(result == false)
            {
                throw new EntityDeleteException("Comment", id.ToString());
            }

            _logger.LogInformation(
                "Comment successfully deleted with id {Id}",
                id);
        }

        public async Task<CommentResponse> GetById(int id)
        {
            var comment = await _commentRepository.GetById(id);
            if (comment == null)
                throw new NotFoundApplicationException($"Comment {id} not found");

            var response = _mapper.Map<CommentResponse>(comment);

            _logger.LogInformation(
                "Comment retrieved with id {Id}",
                id);

            return response;
        }

        public async Task<IEnumerable<CommentResponse>> GetByUserId(int userId)
        {
            var comments = await _commentRepository.GetByUserId(userId);
            var responses = _mapper.Map<IEnumerable<CommentResponse>>(comments);

            _logger.LogInformation(
                "Retrieved {Count} comments for user {UserId}",
                responses.Count(),
                userId);

            return responses;
        }
   
        public async Task<IEnumerable<CommentResponse>> GetAll()
        {
            var comments = await _commentRepository.GetAll();
            var responses = _mapper.Map<IEnumerable<CommentResponse>>(comments);

            _logger.LogInformation(
                "Retrieved {Count} comments in total",
                responses.Count());

            return responses;
        }
    }
}
