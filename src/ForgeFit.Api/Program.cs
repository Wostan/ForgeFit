using ForgeFit.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddValidation()
    .AddApi()
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerDocumentation()
    .AddApplication()
    .AddLowercaseUrls()
    .AddEnumConverter()
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