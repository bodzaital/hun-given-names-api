using HunGivenNames.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHttpClient();
builder.Services.AddTransient<ISync, Sync>();
builder.Services.AddTransient<IName, Name>();
builder.Services.AddTransient<IRepository, Repository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/{name}/gender", (string name, IName nameService) =>
{
    return nameService.Gender(name);
});

app.Run();