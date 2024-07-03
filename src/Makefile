test-make:
	echo hello $(name)

add-migration:
	dotnet ef migrations add --project src\Persistence\Persistence.csproj --startup-project src\Presentation\Presentation.csproj --context Persistence.AppDbContext --configuration Debug --verbose $(name) --output-dir Migrations