using Bloggie.Web.Data;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add DbContext class into our application so that we can easily use it.
//(Whenever we want to use dbcontext class we can use it from Services)
builder.Services.AddDbContext<BloggieDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("BloggieDbConnectionString")));

//Inject AuthDbContext in Program.cs like we did it for BloggieDbContext
builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("BloggieAuthDbConnectionString")));


//But we also have to inform that as part of the identity we are using this AuthDbContext. Kis user ka kia role hoga I think shayad ye
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();


//Injecting the Repositories in the file. Give the implementation instead of interface while calling interface.
builder.Services.AddScoped<ITagRepository, TagRepository>();

//Injecting the Repositories in the file. Give the implementation instead of interface while calling interface.
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();

//Injecting the Repositories in the file. Give the implementation instead of interface while calling interface.
builder.Services.AddScoped<IImageRepository, CloudinaryImageRepository>();

//Injecting the Repositories in the file. Give the implementation instead of interface while calling interface.
builder.Services.AddScoped<IBlogPostLikeRepository, BlogPostLikeRepository>();

//Injecting the Repositories in the file. Give the implementation instead of interface while calling interface.
builder.Services.AddScoped<IBlogPostCommentRepository, BlogPostCommentRepository>();

//Injecting the Repositories in the file. Give the implementation instead of interface while calling interface.
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.Configure<IdentityOptions>(options => 
    {
        //Default settings 
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
    }
);

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
