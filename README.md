# Angular Auth API

This is the backend API for a full-stack application with an Angular frontend. It provides user authentication, authorization, and a simple blog management system. This project is intended to be a portfolio piece.

## Technologies Used

*   .NET 9
*   ASP.NET Core
*   Entity Framework Core
*   PostgreSQL
*   JWT for Authentication
*   Swagger for API documentation
*   Docker

## Features

*   User registration and login (including with GitHub and Google)
*   Role-based authorization (Admin role)
*   Blog post management (Create, Read, Update, Delete)
*   User and role management for admins
*   Profile management (update email, username, password, profile picture)
*   Email verification and password reset

## Getting Started

To get a local copy up and running follow these simple example steps.

### Prerequisites

*   .NET 9 SDK
*   PostgreSQL
*   Docker (optional)

### Installation (without Docker)

1.  Clone the repo
    ```sh
    git clone https://github.com/Yannick-Vk/Angular-Auth.git
    ```
2.  Navigate to the project directory
    ```sh
    cd Angular-Auth/Angular-Auth
    ```
3.  Update the connection string in `appsettings.json` with your PostgreSQL details. The connection string is named `auth`.
4.  Apply the database migrations
    ```sh
    dotnet ef database update
    ```
5.  Run the application
    ```sh
    dotnet run
    ```

### Installation (with Docker)

1.  Clone the repo
    ```sh
    git clone https://github.com/Yannick-Vk/Angular-Auth.git
    ```
2.  Navigate to the project directory
    ```sh
    cd Angular-Auth/Angular-Auth
    ```
3.  Run the application with docker-compose
    ```sh
    docker-compose up -d
    ```

The API will be running at `https://localhost:5001` or `http://localhost:5000`. You can access the Swagger UI at `https://localhost:5001/swagger`.

## API Endpoints

The following are the main endpoints of the API:

### Auth

*   `POST /api/v1/Auth/register`: Register a new user.
*   `POST /api/v1/Auth/login`: Login a user and get a JWT token.
*   `POST /api/v1/Auth/logout`: Logout the current user.
*   `POST /api/v1/Auth/refresh`: Refresh the access token.
*   `GET /api/v1/Auth/verify-email`: Verify user's email.
*   `GET /api/v1/Auth/challenge?provider={provider}`: Challenge a provider for authentication (GitHub, Google).
*   `GET /api/v1/Auth/whoami`: Get the current user's information.

### Blogs

*   `GET /api/v1/Blogs`: Get all blog posts.
*   `GET /api/v1/Blogs/{id}`: Get a blog post by id.
*   `POST /api/v1/Blogs`: Create a new blog post.
*   `PATCH /api/v1/Blogs`: Update a blog post.
*   `DELETE /api/v1/Blogs/{id}`: Delete a blog post.
*   `GET /api/v1/Blogs/{id}/banner`: Get the banner of a blog post.
*   `GET /api/v1/Blogs/author/{userId}`: Get all blogs from a specific author.
*   `GET /api/v1/Blogs/author/me`: Get all blogs from the current user.
*   `GET /api/v1/Blogs/search/{searchText}`: Search for blogs.
*   `POST /api/v1/Blogs/{blogId}/authors/add/{userId}`: Add an author to a blog.
*   `POST /api/v1/Blogs/{blogId}/authors/remove/{userId}`: Remove an author from a blog.

### Profile

*   `PUT /api/v1/me/change/email`: Update the current user's email.
*   `PUT /api/v1/me/change/username`: Update the current user's username.
*   `PUT /api/v1/me/change/password`: Update the current user's password.
*   `PUT /api/v1/me/change/profile-picture`: Upload a profile picture for the current user.
*   `GET /api/v1/me/profile-picture`: Get the profile picture of the current user.
*   `DELETE /api/v1/me/profile-picture`: Delete the profile picture of the current user.
*   `POST /api/v1/me/password/reset/{email}`: Send a password reset email.
*   `POST /api/v1/me/password/confirm`: Confirm the password reset.

### Roles (Admin only)

*   `GET /api/v1/roles`: Get all roles.
*   `POST /api/v1/roles`: Create a new role.
*   `DELETE /api/v1/roles`: Delete a role.
*   `POST /api/v1/roles/add-to-user`: Add a role to a user.
*   `POST /api/v1/roles/remove-from-user`: Remove a role from a user.
*   `GET /api/v1/roles/{roleName}`: Get all users with a specific role.
*   `GET /api/v1/roles/me/{roleName}`: Check if the current user has a specific role.

### Users (Admin only)

*   `GET /api/v1/users`: Get all users.
*   `GET /api/v1/users/find/{userName}`: Get a user by username.
*   `GET /api/v1/users/{userId}`: Get a user by id.
*   `GET /api/v1/users/me`: Get the current user's information.
*   `GET /api/v1/users/{userName}/Roles`: Get roles of a user.
*   `GET /api/v1/users/{userId}/profile-picture`: Get the profile picture of a user.

## Frontend

You can find the front-end projects here:
- [Angular (currently outdated)](https://github.com/Yannick-Vk/Angular-Admin)
- [Vue](https://github.com/Yannick-Vk/vue-blogs)

## License

Distributed under the MIT License. See `LICENSE` for more information.