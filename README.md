dotnet ef migrations add ChangeArchToDDD --project ../Dynamiq.Infrastructure --startup-project ./ --output-dir ../Dynamiq.Infrastructure/Persistence/Migrations

dotnet ef database update --project ../Dynamiq.Infrastructure --startup-project ./

dotnet ef migrations remove --project ../Dynamiq.Infrastructure --startup-project ./
