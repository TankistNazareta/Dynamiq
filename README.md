dotnet ef migrations add НазваТвоєїМіграції --project ../Dynamiq.API.DAL --startup-project ./

dotnet ef database update --project ../Dynamiq.API.DAL --startup-project ./
