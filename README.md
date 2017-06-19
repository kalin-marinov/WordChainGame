# WordChainGame
A Restful Web API, written in C# .NET for the "REST" student's course

# Framework
The used framework is .NET Core 1.1, hosted on Kestrel with IIS integration.

# Authentication
Authentication and Authorization is achieved via Json Web Token (JWT), holding user claims with HS256 signature

## Storage
User and identity related information is stored in Redis store that implements an ASP.NET Identity 3 UserStore

# Game objects
All game related objects are stored in a MongoDb database, that implements a similar pattern, such as the authentication

# Attributes, Filters, Aspects

## Error handling
The error handling is achieved via a global exception filter attribute that produces the proper error messages and status code based on the exception type

## Validation
The validation is achieved through the models / data contracts that are used in the API requests.
Ensure.That library is used as a helper for validating business objects and DTOs

# Testing
The testing is achieved through the .NET Core version of the xUnit testing frameworks. Integration tests are performed via the standart HttpClient in C#
