using System;
using AutoMapper;
using Core;
using Core.Services;
using DataAccess;
using DataAccess.Models;
using DataAccess.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using UsersAccounts.Extensions;
using UsersAccounts.Middlewares;

namespace UsersAccounts
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAutoMapper((Action<IMapperConfigurationExpression>) null, AppDomain.CurrentDomain.GetAssemblies());
			
			string usersDbConnectionString = Configuration.GetConnectionString("UsersDbConnectionString");
			
			services.AddDbContext<UsersDbContext>(opt =>
			{
				opt.UseNpgsql(usersDbConnectionString);
			});

			var dbContext = services.BuildServiceProvider().GetService<UsersDbContext>();
			
			services.AddTransient<IUsersAccountsRepository, UsersAccountsRepository>(e => new UsersAccountsRepository(dbContext));
			services.AddTransient<IRepository<AccountHistory>, AccountHistoryRepository>(e => new AccountHistoryRepository(dbContext));
			services.AddSingleton<IClockProvider, ClockProvider>();
			services.AddTransient<IUsersAcccountsManager, UsersAcccountsManager>();
			
			services.AddMvc(opt =>
				{
					opt.Filters.Add(new GlobalExceptionFilter());
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			
			services.AddSwaggerGen(opt =>
			{
				opt.SwaggerDoc("v1", new Info { Title = "Web API", Version = "v1" });
				opt.DescribeAllEnumsAsStrings();
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseResponseWrapper();
			app.UseSwagger();

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API v1");
			});
			
			app.UseHttpsRedirection();
			app.UseMvc();
		}
	}
}