using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
   
namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<ICommentRepository, CommentRepository>();
            services.AddSingleton<IMessageRepository, MessageRepository>();
            services.AddSingleton<IPostRepository, PostRepository>();
            services.AddSingleton<IReactionRepository, ReactionRepository>();
            services.AddSingleton<IInteractionRepository, InteractionRepository>();

            return services;
        }
    }
}
