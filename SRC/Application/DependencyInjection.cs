using Application.Mappings;
using Application.Service;
using Application.Service.AttachmentService;
using Application.Service.FileStorageService;
using Application.Service.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IReactionService, ReactionService>();
            services.AddTransient<IInteractionService, InteractionService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IAttachmentService, AttachmentService>();
            services.AddTransient<IFileStorageService, FileStorageService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IPasswordHasher, PasswordHasher>();

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
