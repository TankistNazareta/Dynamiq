dotnet ef migrations add НазваТвоєїМіграції --project ../Dynamiq.NAME.DAL --startup-project ./

dotnet ef database update --project ../Dynamiq.Auth.DAL --startup-project ./
