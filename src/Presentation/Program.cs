using Infrastructure.Idempotency;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.Database.IsInMemory() && dbContext.Database.GetPendingMigrations().Any())
        dbContext.Database.Migrate();
}

app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseRequestLocalization();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.UseBlazorFrameworkFiles();

// compress, then cache, then serve the static files
app.UseResponseCompression();
app.UseResponseCaching();
app.UseStaticFiles();

app.UseIdempotency();

#pragma warning disable ASP0014
app.UseEndpoints(endpoints =>
{
    endpoints.MapSwagger();
    endpoints.MapHealthChecks("/health");
    endpoints.MapControllers();
    endpoints.MapDefaultControllerRoute();
    endpoints.MapFallbackToFile("index.html");
});

app.Run();