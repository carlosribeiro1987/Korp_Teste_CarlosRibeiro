using Estoque.API.Data;
using Estoque.API.Services;
using Estoque.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Estoque.API {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();

            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<EstoqueDbContext>(options => {
                options.UseSqlite(builder.Configuration.GetConnectionString("Estoque"));
            });

            builder.Services.AddScoped<IProdutoService, ProdutoService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if(app.Environment.IsDevelopment()) {
                //app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
