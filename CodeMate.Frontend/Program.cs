var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();   // باعث می‌شه با باز کردن ریشه سایت، index.html خودکار لود بشه
app.UseStaticFiles();    // اجازه می‌ده فایل‌های داخل wwwroot سرو بشن

app.Run();