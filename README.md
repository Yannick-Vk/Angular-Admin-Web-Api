# Angular Auth API

This is the backend API for a full-stack application with an Angular frontend. It provides user authentication, authorization, and a simple blog management system. This project is intended to be a portfolio piece.

## Technologies Used

*   .NET 9
*   ASP.NET Core
*   Entity Framework Core
*   SQL Server
*   JWT for Authentication
*   Swagger for API documentation

## Features

*   User registration and login
*   Role-based authorization (Admin role)
*   Blog post management (Create, Read, Update, Delete)
*   User and role management for admins

## Getting Started

To get a local copy up and running follow these simple example steps.

### Prerequisites

*   .NET 9 SDK
*   SQL Server

### Installation

1.  Clone the repo
    ```sh
    git clone https://github.com/Yannick-Vk/Angular-Auth.git
    ```
2.  Navigate to the project directory
    ```sh
    cd Angular-Auth/Angular-Auth
    ```
3.  Update the connection string in `appsettings.json` with your SQL Server details. The connection string is named `auth`.
4.  Apply the database migrations
    ```sh
    dotnet ef database update
    ```
5.  Run the application
    ```sh
    dotnet run
    ```

The API will be running at `https://localhost:5001` or `http://localhost:5000`. You can access the Swagger UI at `https://localhost:5001/swagger`.

## API Endpoints

The following are the main endpoints of the API:

### Auth

*   `POST /api/v1/Auth/register`: Register a new user.
*   `POST /api/v1/Auth/login`: Login a user and get a JWT token.
*   `POST /api/v1/Auth/logout`: Logout the current user.
*   `POST /api/v1/Auth/refresh`: Refresh the access token.

### Roles

*   `GET /api/v1/roles`: Get all roles (Admin only).
*   `POST /api/v1/roles`: Create a new role (Admin only).
*   `DELETE /api/v1/roles`: Delete a role (Admin only).
*   `POST /api/v1/roles/add-to-user`: Add a role to a user (Admin only).
*   `POST /api/v1/roles/remove-from-user`: Remove a role from a user (Admin only).
*   `GET /api/v1/roles/{roleName}`: Get all users with a specific role (Admin only).
*   `GET /api/v1/roles/me/{roleName}`: Check if the current user has a specific role.

### Users

*   `GET /api/v1/User`: Get all users (Admin only).
*   `GET /api/v1/User/{id}`: Get a user by id (Admin only).
*   `GET /api/v1/User/me`: Get the current user's information.

### Blogs

*   `GET /api/v1/Blog`: Get all blog posts.
*   `GET /api/v1/Blog/{id}`: Get a blog post by id.
*   `POST /api/v1/Blog`: Create a new blog post.
*   `PUT /api/v1/Blog/{id}`: Update a blog post.
*   `DELETE /api/v1/Blog/{id}`: Delete a blog post.

## Frontend

The frontend for this project is an Angular application and is located in a separate repository. You can find it here: [https://github.com/Yannick-Vk/Angular-Admin](https://github.com/Yannick-Vk/Angular-Admin)

## License

Distributed under the MIT License. See `LICENSE` for more information.
