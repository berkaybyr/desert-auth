using desert_auth.Class;


var builder = WebApplication.CreateBuilder(args);

Service _service = new Service(true);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseMiddleware<IPControlMiddleware>();

//sapp.UseHttpsRedirection();


app.MapControllers();


    app.Run();
