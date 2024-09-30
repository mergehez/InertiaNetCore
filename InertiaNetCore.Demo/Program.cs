using InertiaNetCore.Extensions;
using InertiaNetCore.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddInertia(options =>
{
    // options.JsonResultResolver = (page) => CustomResults.Ok(page);
});
builder.Services.AddViteHelper();




var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseInertia()
    .AddSharedData(_ => new InertiaProps
    {
        ["Auth"] = new InertiaProps
        {
            ["Token"] = "123456789",
            ["Username"] = "Mergehez",
        }
    });

app.Run();