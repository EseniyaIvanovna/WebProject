using Application.Mappings;
using Application.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<IReactionService, ReactionService>();
            services.AddTransient<IInteractionService, InteractionService>();
            services.AddTransient<IMessageService, MessageService>();

            return services;
        }
    }
}
