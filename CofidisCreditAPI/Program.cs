var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.WebHost.UseUrls("http://localhost:5000");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Credit Check Cofidis");
        c.RoutePrefix = string.Empty;
    });

}

app.UseAuthorization();

app.MapControllers();

app.Run();
