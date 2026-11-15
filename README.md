Postgres:
brew services start postgresql@18

PATH
echo 'export PATH="/opt/homebrew/opt/postgresql@18/bin:$PATH"' >> ~/.zshrc && source ~/.zshrc

postgresql://makowiec@localhost:5432/dynasty-blake


dotnet ef migrations add InitialIdentity --project src/Blake.Infrastructure --startup-project src/Blake.Web
otnet ef database update InitialIdentity --project src/Blake.Infrastructure --startup-project src/Blake.Web