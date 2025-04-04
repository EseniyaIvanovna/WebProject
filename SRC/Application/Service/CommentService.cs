using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain;
using Infrastructure.Repositories.Interfaces;

namespace Application.Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository commentRepository, IUserRepository userRepository, IPostRepository postRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _postRepository = postRepository;
            _mapper = mapper;
        }
       
        public async Task<int> Create(CreateCommentRequest request)
        {
            var comment = _mapper.Map<Comment>(request);
            return await _commentRepository.Create(comment);
        }
        
        public async Task Update(UpdateCommentRequest request)
        {
            var existingComment = await _commentRepository.GetById(request.Id);
            if (existingComment == null)
                throw new NotFoundApplicationException($"Comment {request.Id} not found");

            existingComment.Content = request.Content;
            await _commentRepository.Update(existingComment);
        }

        public async Task Delete(int id)
        {
            var comment = await _commentRepository.GetById(id);
            if (comment == null)
                throw new NotFoundApplicationException($"Comment {id} not found");

            await _commentRepository.Delete(id);
        }

        public async Task<CommentResponse> GetById(int id)
        {
            var comment = await _commentRepository.GetById(id);
            if (comment == null)
                throw new NotFoundApplicationException($"Comment {id} not found");

            return _mapper.Map<CommentResponse>(comment);
        }

        public async Task<IEnumerable<CommentResponse>> GetByUserId(int userId)
        {
            var comments = await _commentRepository.GetByUserId(userId);
            return _mapper.Map<IEnumerable<CommentResponse>>(comments);
        }
   
        public async Task<IEnumerable<CommentResponse>> GetAll()
        {
            var comments = await _commentRepository.GetAll();
            return _mapper.Map<IEnumerable<CommentResponse>>(comments);
        }
    }
}
