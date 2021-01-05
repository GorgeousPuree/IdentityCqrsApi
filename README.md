# Identity CQRS app
#### Add this variables to your `appsettings.json` or `secrets.json` to start server locally:

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost; Port=5432; Database=identity_app; Username=postgres; Password=12345;"
  },
  "AuthOptions": {
    "Issuer": "IdentityApp",
    "Audience": "None",
    "Key": "secret_secret_secret_secret_secret_secret_secret_secret_secret_secret_secret_secret_secret_secret",
    "Lifetime": 43200
  } 
}
```
https://identity-cqrs-app.herokuapp.com/swagger/index.html
