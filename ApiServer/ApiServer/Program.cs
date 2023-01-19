using ApiServer.Tools;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

DBConnectionTestResult connResult = DatabaseHandler.TestConnection();
if (connResult == DBConnectionTestResult.Success)
{
    app.Logger.LogInformation($"Database connection good.");
}
else if(connResult == DBConnectionTestResult.DefaultConnectionSuccess)
{
    app.Logger.LogCritical($"Database could only be reached with default connection string. You have to set it up according to instructions.");
}
else if(connResult == DBConnectionTestResult.Failure)
{
    app.Logger.LogCritical($"Database couldn't be reached. Make sure it is online and configured correctly.");
}
else
{
    app.Logger.LogCritical($"Unhandled enum in database connection test. Value: {connResult}");
}

app.Run();