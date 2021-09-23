# Library website
Simple ASP.NET Core website for fictional library.

## Table of contents
* [General info](#general-info)
* [Technologies](#technologies)
* [Setup](#setup)

## General info
The main goal of this project was to learn about technology related to creating websites, focusing mainly on the backend,
but not omitting the frontend as well. Hosted website can be viewed at:
[https://simplelibrarywebsite20210906115859.azurewebsites.net](https://simplelibrarywebsite20210906115859.azurewebsites.net "Hosted website").

To login as a librarian or an administator please view [Setup](#setup).

The following functionalities have been implemented:

- __Book management__  
![Book Managment](https://github.com/Kasprzak-Arkadiusz/SimpleLibraryWebsite/blob/master/images/Book%20management.png)

- __Book loan management__  
![Loan Management](https://github.com/Kasprzak-Arkadiusz/SimpleLibraryWebsite/blob/master/images/Loan%20management.png)

- __Book request management__  
![Request Management](https://github.com/Kasprzak-Arkadiusz/SimpleLibraryWebsite/blob/master/images/Request%20management.png)

- __Logging in with external service__
- __User roles management__
- __Restrictring access depending on user role__
- __Changing account settings__
- __Viewing own book requests and loans__

## Technologies
The following technologies and libraries were used during the development:

- __C# 9.0__
- __.NET Core 5.0__
- __ASP.NET Core 5.0.9__
- __Entity Framework 5.0.8__
- __MSSQL__
- __NLog 4.7.10__
- __X.PagedList.Mvc.Core 8.1.0__
- __Bootstrap 5__
- __Azure Key Vault__

## Setup
To set up a website, follow these steps:

1. Clone repository from github.
2. Build solution. If you get a dependency error, clean the solution and nuget cache. Then rebuild the solution.
3. Fill User Secrets file with the following content:
```javascript
{  
"AdminSeedPassword": "Passw0rd!",
"Authentication:Google:ClientId": "144664142705-1pl3flem38kulrj6rlumbca1k604i5ek.apps.googleusercontent.com",
"Authentication:Google:ClientSecret": "L-uSxZGTTzdbYGzsMRO9jrSi",
"LibrarianSeedPassword": "Pa$$word?"
}
```

5. In .NET Core CLI run  
  `dotnet ef database update`  
  or in Package Manager Console run  
  `Update-Database`
6. Start program
