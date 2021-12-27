using GraphQL.MicrosoftDI;
using GraphQL.Server;
using GraphQL.Types;
using GraphQLNetExample.EntityFramework;
using GraphQLNetExample.Notes;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add services to the container.
builder.Services.AddDbContext<NotesContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

// add notes schema
builder.Services.AddSingleton<ISchema, NotesSchema>(services => new NotesSchema(new SelfActivatingServiceProvider(services)));
// register graphQL
builder.Services.AddGraphQL(options =>
                {
                    options.EnableMetrics = true;
                })
                .AddSystemTextJson();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "GraphQLNetExample", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GraphQLNetExample v1"));
    // add altair UI to development only
    app.UseGraphQLAltair();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

// make sure all our schemas registered to route
app.UseGraphQL<ISchema>();

app.Run();
