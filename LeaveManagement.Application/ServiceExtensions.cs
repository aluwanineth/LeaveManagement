using FluentValidation;
using LeaveManagement.Application.Behaviors;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Application.Mappings;
using LeaveManagement.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LeaveManagement.Application;

public static class ServiceExtensions
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<GeneralProfile>();
        });
        services.AddMediatR
             (cfg =>
                   cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            );

        services.AddScoped<IBusinessDayService, BusinessDayService>();

    }
}
