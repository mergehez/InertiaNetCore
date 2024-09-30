using InertiaNetCore.Extensions;
using InertiaNetCore.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddInertia();
builder.Services.AddViteHelper();


var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseInertia();
app.AddInertiaSharedData(_ => new InertiaProps
{
    ["Auth"] = new InertiaProps
    {
        ["Token"] = "123456789",
        ["Username"] = "Mergehez",
    }
});

app.Run();