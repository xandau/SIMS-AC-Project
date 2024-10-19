# SIMS-AC-Project
This project is include a Frontend created with Angular and a Backend created with ASP.Net and Entity Framework. With the Frontend the user can use the features to create and lookup assigned security tickets.

## Badges
![Version](https://img.shields.io/badge/version-v1.0-blue)
![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Angular](https://img.shields.io/badge/angular-v18-red)
![.NET](https://img.shields.io/badge/.NET-8.0-red)
![Docker](https://img.shields.io/badge/docker-supported-blue)
![Redis](https://img.shields.io/badge/Redis-v7.0-red)
![MSSQL](https://img.shields.io/badge/MSSQL-database-blue)
![Semgrep Test](https://img.shields.io/badge/Security-Semgrep-green)

![Coverage](https://img.shields.io/codecov/c/github/DEIN_USERNAME/DEIN_REPOSITORY)

## Version

Current Version: **v1.0** 

## License

This project is licensed under the **MIT license**. Details about the license can be found in the file [LICENSE](https://mit-license.org/).   


## Contributors

- **Benjamin Borenich** - Frontend-Developer - is231024@fhstp.ac.at
- **Fabian Treber** - Backend-Developer - is231007@fhstp.ac.at

## Roadmap

### Version 0.1 (14.10.2024)
- Creation of database containers
- Creation of entity framework models and first migrations
### Version 0.2 (15.10.2024)
- Implemented Repository-Pattern and Controllers
- Implemented JWT-Token generation
### Version 0.3 (16.10.2024)
- Tried Blazor and sufferd from it - switched to Angular
- Created AuthRepository for better separation
### Version 0.4 (17.10.2024)
-  Extended funtionality of repositories and controllers
### Version 0.5 (18.10.2024)
- Middleware for checking connection user role
- Semgrep-Test of Backend
### Version 0.6 (19.10.2024)
- Extended middleware for check of database connection 
- Merged Git-Branches and destroyed repository (thanks beni ;) )
- Fixed repository (thanks beni ;) )
### Version 0.7 (20.10.2024)
- Docker integration

## Program functionality and Features 
A user can register himself on the website. In the background the user is stored in a MSSQL-Database. After the registration the user is able to login on the webapp which gives him access to create tickets concerning security or even view and assign open tickets. When logging in the user receives a access and refresh token, technically speaking JWT-Tokens. The access token is to authorize the client on the backend and with the refresh token a new access token can be created without the input of credentials. Refresh tokens are stored in a Redis-NoSQL-Database.

The system differs between users and admins. The main difference between the two roles is that admins can make CRUD operations with the user entries whereas the user can view, create, edit or delete tickets. User can be blocked and receive an UUID and a autoincrement id from the database because in .NET 8 a UUIDv7 can not be created. Additionally, the email, username, first and lastname are stored with the hashed and salted password of the user.  

A user can create multiple tickets and can have more than one assigned - in the tickets the creator must be stored and a assigned person can be stored. Each tickets gets a id from the database in addition to its title. Furthermore a description, its severity and a CVE-ID is provided with the ticket. The state of the ticket and its creationtime can also be received. 

Logs about the CRUD operations of users and tickets can also be viewed, created, edited or removed for the system. 

The backend provides an API and the main logic of the system. It connects to the two Databases, Redis and MSSQL. It communicated with the Angular-Webapp to provide the user with its features. The MSSQL database is crucial for the services otherwise the system can not meet the needs of the consumers. The NoSQL-Database is only crucial for login or refresh operation in other words for giving the user its access tokens. Both database are checked for connectivity with the use of a middleware. If one of them is not able to be conntacted a error message is printed. In this way the backend is prevented from crashing.

## System requirements

- **CPU**: 2 Kerne (depending on load)
- **RAM**: 8 GB RAM (depending on load)
- **Storage**: 10 GB (depending on load)
- **Operating System**: Windows 10 or higher, not tested on linux devices
- **Software**:
  - Docker Engine 
  - .NET 8 Runtime
  - Node.js for Angular
  - MSSQL and Redis Container in docker compose
  - SSL-configuration for web und API security (in this project self-signed certificates are used)

You can also use the docker compose provided in this repository and the dockerfiles for hsoting the frontend and backend.

### Create a .env File with the neccesarry path for the docker database containers
The .env-file is used in the dockercompose.yml and locates where the database files are persistently stored: ``` .env  DB_DATA_PATH=c:\yourpath_here ```

## UML-Diagram

![Alt text](https://github.com/xandau/SIMS-AC-Project/blob/main/doku/UML-C%23.svg)
<img src="https://github.com/xandau/SIMS-AC-Project/blob/main/doku/UML-C%23.svg">

### Explaination

As mentioned the projects uses the entity framework and stores three items: Users, Tickets and LogEntries. 
All of them inherit from AItem which gives the a ID-Property. Tickets and User have both an enum for the state of the ticket and the role of the admin.
In SIMSContext which is the DbContext in this project all three are registered in order to be used.

The backend makes use of the repository pattern. Each item has a repository and they inherit from the generic abstract ARepository class which gives each repository the implemented methods defined in IRepository. The implemented methods of the ARepository uses the SIMSContext and its DbSets of the items. The implemented In some of the repositories methods where overwritten or other methods added. 

The repositories are used in the controllers named after the item they provide interfaces for. These controllers inherit from a generic abstract class called AController similar to the repositories. The AuthController is a exception an does not implement a interface or inherit from a base class. The AuthController uses to classes: RedisTokenStore for connection and CRUD operation to the Redis-Database and the JwtService-Class to create and access the claims of a access token where the ID and username can be selected. Additionally it uses two DTOs for login and the refresh token in each in one method to make it validation easier.

In Program.cs everything is put together and registered to be used. An important part is the Middleware where the connection of the databases is checked and the access to the "/user" interfaces if the user behind the request is an admin or not. If one of the credentials is not met, the response is an error message. 

## ER-Diagram

![Alt text](https://github.com/xandau/SIMS-AC-Project/blob/main/doku/UML-SQL.svg)
<img src="https://github.com/xandau/SIMS-AC-Project/blob/main/doku/UML-SQL.svg">

### Explaination

In this diagram the three items are shown which are created by the Entity Framework. A user can create multiple tickets and can have more than one assigned - in the tickets the creator must be stored and a assigned person can be stored. LogEntry do not have any relations. In this table are log stored regarding CRUD operations in the other two table. Not shown in the diagram is the migrations table created by entity framework because it is not necessary for the system. 