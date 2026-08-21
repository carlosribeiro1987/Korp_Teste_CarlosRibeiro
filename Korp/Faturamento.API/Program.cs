
using Faturamento.API.Clients;
using Faturamento.API.Clients.Interfaces;
using Faturamento.API.Data;
using Faturamento.API.Services;
using Faturamento.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.API {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
            });
            builder.Services.AddDbContext<FaturamentoDbContext>(options => {
                options.UseSqlite(builder.Configuration.GetConnectionString("Faturamento"));
            });
            builder.Services.AddScoped<INotaFiscalService, NotaFiscalService>();
            builder.Services.AddHttpClient<IEstoqueClient, EstoqueClient>(client => {
                var estoqueApi = builder.Configuration["Services:EstoqueApi"] ?? 
                                    throw new InvalidOperationException("ConfigureServices:EstoqueApi.");

                client.BaseAddress = new Uri(estoqueApi);

            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if(app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using(var scope = app.Services.CreateScope()) {
                var db = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();
                db.Database.Migrate();
            }

            app.MapControllers();

            app.Run();
        }
    }
}
