using InertiaNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddInertia(options =>
{
    // options.JsonResultResolver = (page) => EtResults.Ok(page).AsInertiaResult()!.ExecuteResultAsync(context);
}).AddViteHelper();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseInertia();
app.Run();