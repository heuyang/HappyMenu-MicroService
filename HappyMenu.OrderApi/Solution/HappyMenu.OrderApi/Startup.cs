using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using HappyMenu.OrderApi.Data.Database;
using HappyMenu.OrderApi.Data.Repository;
using HappyMenu.OrderApi.Domain;
using HappyMenu.OrderApi.Messaging.Receive.Options.v1;
using HappyMenu.OrderApi.Messaging.Receive.Receiver.v1;
using HappyMenu.OrderApi.Models;
using HappyMenu.OrderApi.Service.v1.Command;
using HappyMenu.OrderApi.Service.v1.Query;
using HappyMenu.OrderApi.Service.v1.Services;
using HappyMenu.OrderApi.Validators.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HappyMenu.OrderApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<RabbitMqConfiguration>(Configuration.GetSection("RabbitMq"));

            services.AddDbContext<OrderContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddAutoMapper(typeof(Startup));

            services.AddMvc().AddFluentValidation();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Order Api",
                    Description = "A simple API to create or pay orders",
                    Contact = new OpenApiContact
                    {
                        Name = "Wolfgang Ofner",
                        Email = "WolfgangOfner@aon.at",
                        Url = new Uri("https://www.programmingwithwolfgang.com/")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext =
                        actionContext as ActionExecutingContext;

                    if (actionContext.ModelState.ErrorCount > 0
                        && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });

            services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(ICustomerNameUpdateService).Assembly);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddTransient<IValidator<OrderModel>, OrderModelValidator>();

            services.AddTransient<IRequestHandler<GetPaidOrderQuery, List<Order>>, GetPaidOrderQueryHandler>();
            services.AddTransient<IRequestHandler<GetOrderByIdQuery, Order>, GetOrderByIdQueryHandler>();
            services.AddTransient<IRequestHandler<GetOrderByCustomerGuidQuery, List<Order>>, GetOrderByCustomerGuidQueryHandler>();
            services.AddTransient<IRequestHandler<CreateOrderCommand, Order>, CreateOrderCommandHandler>();
            services.AddTransient<IRequestHandler<PayOrderCommand, Order>, PayOrderCommandHandler>();
            services.AddTransient<IRequestHandler<UpdateOrderCommand>, UpdateOrderCommandHandler>();
            services.AddTransient<ICustomerNameUpdateService, CustomerNameUpdateService>();

            services.AddHostedService<CustomerFullNameUpdateReceiver>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
