using Infrastructure.Idempotency;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.Database.IsInMemory() && dbContext.Database.GetPendingMigrations().Any())
        dbContext.Database.Migrate();

    if (builder.Environment.IsDevelopment())
    {
        dbContext.SeedTestData();
    }
}

app.UseHttpLogging();
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseRequestLocalization();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

if (app.Environment.IsDevelopment())
{
    app.UseResponseCompression();
    app.UseResponseCaching();
}

app.UseIdempotency();

app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();