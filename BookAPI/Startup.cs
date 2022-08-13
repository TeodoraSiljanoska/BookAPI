using BookAPI.Models;
using BookAPI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
//using Microsoft.TodoApi.Nodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;









namespace BookAPI
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
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddDbContext<BookContext>(o => o.UseSqlite("Data source=books.db"));
            services.AddControllers();
            services.AddSwaggerGen(c =>
           {
               c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookAPI", Version = "v1" });
           }
            );

        }
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        

     
    }
}
