using APIGateway;
using APIGateway.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Configure Dapper
builder.Services.AddTransient<DapperContext>();
builder.Services.AddTransient<ConfigurationService>();
//builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls("http://+:5000"); // Change to a new port

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Get the configuration service instance
var configService = app.Services.GetRequiredService<ConfigurationService>();

// Middleware for logging
app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
    Console.WriteLine($"Response status: {context.Response.StatusCode}");
});

// Custom routing middleware using dynamic configurations
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    if (!string.IsNullOrEmpty(path))
    {
        var serviceName = path.Split('/')[1]; // Assuming URL is like /service1 or /service2
        var serviceUrl = configService.GetServiceUrl(serviceName);

        if (!string.IsNullOrEmpty(serviceUrl))
        {
            //context.Request.Path = new PathString(originalPath.Substring("/service1".Length));
            var client = new HttpClient();
            var response = await client.GetAsync(serviceUrl + path.Substring(serviceName.Length + 1));
            context.Response.StatusCode = (int)response.StatusCode;
            await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());

            await next(context);
        }
    }

    context.Response.StatusCode = 404;
    await context.Response.WriteAsync("Service Not Found");
});

app.Run();
