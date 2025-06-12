dotnet ef migrations add НазваТвоєїМіграції --project ../Dynamiq.Auth.DAL --startup-project ./

dotnet ef database update --project ../Dynamiq.Auth.DAL --startup-project ./
