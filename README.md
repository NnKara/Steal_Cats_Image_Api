**Steal All Cats Images Project**

Current Project is about downloading cat images from The Cat API and storing them in a SQL Server database using an ASP.NET Core Web API.
The application uses Hangfire background jobs to fetch images asynchronously and provides REST API endpoints for searching and viewing the stored data. 


------------------------------------------------------------------------

**Requirements**

We need:

-.NET 8 SDK
 https://dotnet.microsoft.com/download/dotnet/8.0

-Microsoft SQL Server

 -SQL Server Express
 -SQL Server LocalDB
 -Full SQL Server

-The Cat API Key
 https://thecatapi.com/signup

------------------------------------------------------------------------

**Project Setup**

1. Clone repository

-git clone https://github.com/NnKara/Steal_Cats_Image_Api.git
-cd  Steal_Cats_Image_Api

------------------------------------------------------------------------

2. Create Cat API Key

1. Create an account in : https://thecatapi.com/signup
2. Go to Dashboard
3. Select "Get your api key"
4. Select get free access
5. Enter your e-mail , add a description and press submit
6. Check your e-mail for the API key

------------------------------------------------------------------------

3. Set API Key (User Secrets -- Recommended)

For safety reasons and to avoid committing sensitive data, it's recommended to use User Secrets for storing the API key during development.
Follow these steps:

1. cd Steal_Cats_Image_Api
2. Run command : dotnet user-secrets set "TheCatApi:ApiKey" "YOUR_API_KEY"
3. Replace "YOUR_API_KEY" with the API key you obtained in your e-mail.


------------------------------------------------------------------------

Alternative: Using ->  appsettings.json

Instead of using User Secrets, you can store the API key directly in appsettings.json

json
"TheCatApi": {
  "BaseUrl": "https://api.thecatapi.com/v1/",
  "ApiKey": "YOUR_API_KEY"
}

!Not recommended approach for development, but it can be used for quick testing.

------------------------------------------------------------------------

4. Database Configuration

The application uses SQL Server as its primary database.

Add the connection strings to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=StealCatsImageDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True",
    "HangfireConnection": "Server=localhost\\SQLEXPRESS;Database=StealCatsImageDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
  }
}
```

-Be sure to replace the connection string with your actual database server details. The above example assumes a local SQL Server Express instance.
-Migrations run automatically at startup. Ensure the database server is running and the connection string is correct.

------------------------------------------------------------------------

5. Running the Application

Follow these steps to build and run the API locally.

Navigate to the API project directory and restore the dependencies:

-cd Steal_Cats_Image_Api
-dotnet restore
-dotnet build


**Start the application using:**

-dotnet run

By default, the API runs at:

HTTP: http://localhost:5004

-If specific port is taken you can change it in launchSettings.json

**API Documentation**

Once the application is running, you can access the interactive API documentation (Swagger UI) at: /swagger

Example:

https://localhost:7175/swagger

------------------------------------------------------------------------

**Using Docker**

You can run the application also by using Docker.


**Build the Docker Image**

From the root directory of the repository, run:

```bash
docker build -t steal-cats-api -f Steal_Cats_Image_Api/Dockerfile .
docker compose up --build
```

Be sure you are inside the folder: 
```bash
cd ..
```

Swagger documentation: http://localhost:5004/swagger

------------------------------------------------------------------------

**API Endpoints**

  POST || /api/cats/fetch?limit=25 || **Fetches cat images - background job**

  GET || /api/cats/{id} || **Get a cat by its id**

  GET || /api/cats?page=1&pageSize=10 || **Fetch cats with paging**

  GET || /api/cats?tag=playful&page=1&pageSize=10 || **Fetch cats by tag**

  GET || /api/jobs/{jobId} || **Fetches job status**



**Hangfire Dashboard**

The application includes **Hangfire Dashboard**, which provides a web interface for monitoring and managing background jobs.

Through the dashboard you can:

- View queued, processing, and completed jobs
- Monitor failed jobs and retry them
- See detailed job execution information
- Manually trigger or delete jobs if needed

You can access the dashboard at: /hangfire

eg : https://localhost:7175/hangfire



**Running Tests**

The project includes unit tests (xUnit) and integration tests.

**Unit Tests** – Test the CatService with mocked repositories and API client.
- GetByIdAsync
- GetPagedAsync
- GetPagedByTagAsync
- FetchCatsAsync


**Integration Tests** – Test the HTTP endpoints with In-Memory database.
- GET /api/cats
- GET /api/cats/{id}
- POST /api/cats/fetch


**Run All Tests:**

-dotnet test



**Enjoy stealing cats responsibly**

Fetch them, store them, paginate them… and most importantly, enjoy browsing through all the cats!

