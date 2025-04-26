using Application.Requests;
using Application.Responses;
using AutoMapper;
using Domain;

namespace Application.Mappings
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCommentRequest, Comment>();
            CreateMap<UpdateCommentRequest, Comment>();
            CreateMap<Comment, CommentResponse>();

            CreateMap<CreatePostRequest, Post>();
            CreateMap<UpdatePostRequest, Post>();
            CreateMap<Post, PostResponse>();

            CreateMap<CreateUserRequest, User>();
            CreateMap<UpdateUserRequest, User>();
            CreateMap<User, UserResponse>();

            CreateMap<RegistrationRequest, User>();
            CreateMap<User, RegistrationRequest>();

            CreateMap<CreateReactionRequest, Reaction>();
            CreateMap<UpdateReactionRequest, Reaction>();
            CreateMap<Reaction, ReactionResponse>();

            CreateMap<CreateMessageRequest, Message>();
            CreateMap<UpdateMessageRequest, Message>();
            CreateMap<Message, MessageResponse>();

            CreateMap<CreateInteractionRequest, Interaction>();
            CreateMap<UpdateInteractionRequest, Interaction>();
            CreateMap<Interaction, InteractionResponse>();
        }
    }
}
