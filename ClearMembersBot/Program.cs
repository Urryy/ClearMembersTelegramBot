using ClearMembersBot;
using ClearMembersBot.Service;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(p => p.ListenLocalhost(7003));



builder.Services.AddSingleton<TelegramBot>();
builder.Services.AddTransient<ITelegramBotService, TelegramBotService>();
builder.Services.AddControllers();
var app = builder.Build();

app.UseHttpsRedirection();
app.Services.GetRequiredService<TelegramBot>().StartRecieveBot().Wait();

app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();