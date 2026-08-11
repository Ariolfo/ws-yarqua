dotnet tool install --global dotnet-ef

dotnet ef migrations add IdentityMigration \
    --project src/Yarqua.Infrastructure \
    --startup-project src/Yarqua.Api \
    --output-dir Persistence/Migrations

bash database/install.sh    

    dotnet ef database update \
    --project src/Yarqua.Infrastructure \
    --startup-project src/Yarqua.Api