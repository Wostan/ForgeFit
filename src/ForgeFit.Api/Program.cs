using ForgeFit.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApi()
    .AddSwaggerDocumentation()
    .AddValidation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApi();

app.Run();