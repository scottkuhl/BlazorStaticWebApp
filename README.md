# Azure Static Web App

This template is an example and starting point for creating a Azure Static Web App based on Blazor.
It goes further than the Blazor Starter provided by Microsoft.
The demo uses a basic movie list viewer with a separate lazy loaded area for administration.

## TO DO

Create stories for the following based on the application needs.

1. Update this README (title, description, user experience).

1. Rename the solution and all instances of the application name.

1. Add support channels (Discord, Email (SendGrid), GitHub Discussions, Reddit, Report Abuse).

1. Add revenue (Ads, Affiliate Links, Paid Accounts, Patreon, Merchandise, Revenue Cat).

1. Custom app icons and images.

1. Help (Wiki, [Tango](https://chrome.google.com/webstore/detail/tango-screenshots-trainin/lggdbpblkekjjbobadliahffoaobaknh)).

1. Add communication channels (Blog, Newsletter).

1. Update the privacy policy page.

### Tips to Reduce Complexity

1. Consider replacing the Resource file usage with plain text if you are only going to support one language.

1. Remove the Lazy Loaded assemblies if the application is not large.

## Getting Started

1. Install the **latest version** of [Visual Studio](https://visualstudio.microsoft.com/).

1. Setup your workstation to develop with **[Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)**.  You will also need the **.NET WebAssembly build tools** optional component.

1. Add the **Azure development workload** to Visual Studio.

1. Install the **[Azure Static Web Apps CLI](https://techcommunity.microsoft.com/t5/apps-on-azure-blog/introducing-the-azure-static-web-apps-cli/ba-p/2257581)**.  This will launch automatically when the project is opened using the Command Task Runner extension.

1. Install the **[Azure Cosmos DB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)**.  This will launch automatically if it is not already running when the project is opened using the Command Task Runner extension.

1. Install the **[Markdown Editor](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor64)** extension.  You can use this to preview markdown files like this one.

1. Install the **[Command Task Runner (64-bit)](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.CommandTaskRunner64)** extension.

1. Install the **[CodeMaid](https://marketplace.visualstudio.com/items?itemName=SteveCadwallader.CodeMaidVS2022)** extension.

1. Install the **[Code Cleanup on Save](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.CodeCleanupOnSave)** extension.  The clean up profile should have all options applied.  Unfortunately at this time, these options can't be stored in a solution.

*Optional: Setup Windows Terminal to close the process exits, fails or crashes instead of closing only when the process exits successfully.
This will close the Azure Functions window when you stop debugging.*

### Visual Studio 2022

Once you clone the project, open the solution in [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) and follow these steps:

1. Right-click on the solution and select **Set Startup Projects...**.

1. Select **Multiple startup projects** and set the following actions for each project:
    - *Api* - **Start**
    - *Client* - **Start**
    - *Shared* - None

1. Press **F5** to launch both the client application and the Functions API app.

## Clean Code Rules

1. [CodeMaid](http://www.codemaid.net/) - The solution folder contains a basic configuration for it that will automatically apply setting on save.

1. [Roslynator](https://github.com/JosefPihrt/Roslynator) - A extended collection of Rosyln analyzers is included as a NuGet package on all projects.

1. [AsyncFixer](http://www.asyncfixer.com/) - A NuGet package included on all projects that finds and corrects common misuses of async/await.

1. A starting .editorconfig file with minimal rule changes is included.

1. Code styles are [enforced on build](https://docs.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#enforcecodestyleinbuild).

1. [Code Cleanup on Save](https://github.com/madskristensen/CodeCleanupOnSave) - Use this extension in Visual Studio to apply code cleanup rules on every save. 

1. Changes should not be committed with unresolved compiler Errors, Warning or Messages.

1. Razor components should use code behind files.

## User Experience

EXAMPLE

- The user interface should adapt primary to **mobile and tablets**.

- The **voice and personality** of the application should feel fun.

- Since we will be using MudBlazor, the application will follow **[Material Design](https://material.io/develop/web)** for the web guidelines.

- Icons are from the Material Icon set in MudBlazor and **[Undraw](https://undraw.co/)** when required.

## Architecture

- **Common**: Common functionality that is not application specific is located in separate projects.

- **Api**: A C# Azure Functions API, which the Blazor application will call

- **Client**: The Blazor WebAssembly sample application

- **Shared**: A C# class library with a shared data model between the Blazor and Functions application

### Api

- **Cache**: Basic response caching is available using a custom filter.

- **Cancellation Tokens**: Cancellation tokens are used to end requests if the client cancels them.

- **Data**: Cosmos DB is used to store data.
A common repository is setup for shared logic.
Bogus will seed test data in the local environment on startup.

- **Exceptions**: Common custom exceptions are available.  Functions inherit from a base Function class which implements global exception handling and logging which can be monitored by Application Insights.

- **Lists**: List based queries support server side paging, filtering, searching, sorting and return meta data about the results as either a pagable or virtualized list.

- **Parameters**: Standard query parameters are extracted from the query string.

- **Security**: Authorization for functions is defined in the staticwebapp.config.json file in the Client project. Any method that requires admin access is under a /manage route.  (/admin is a reserved route that can't be used)

- **Uploads**: File uploads are stored in Azure Storage.  The local Azurite emulator is installed with Visual Studio 2022 and the API project is configured to run it.

- **Validation**: A custom model validator will deserialize and valid an model sent into the body of the request.

### Client

- **AOT**: Ahead-of-Time compilation is enabled on for published releases.

- **Cancellation Tokens**: Cancellation tokens are passed to API calls and the request is canceled if a user navigates to another component.

- **Components**: There are standard components for confirmation and notification dialogs.

- **Exceptions**: There are custom 404, unauthorized, client and server pages.
The HTTP Client enables an interceptor that handles any errors coming back from the server and sends them to the proper error page.
Any other error thrown on a page is caught in a global error boundary on the Main Layout component and sent to the client error page.
Anything that is thrown outside of all of these will trigger the error UI in the index.html page.

- **Form Submit**: Double form posting can be prevented using an IsSubmitting variable.

- **Lazy Loading**: The client application is broken into logical areas to speed up the initial render by lazy loading the area projects.

- **Loading**: HTTP requests will show a loading bar.

- **Localization**: Resource files are available for multiple language support.
The default language is based on the browser, but a component is available that allows the user to change their language.

- **Privacy**: A sample privacy page is included as well as a [GDPR cookie compliance banner](https://github.com/majorimi/blazor-components/blob/master/.github/docs/GdprConsent.md).

- **Progessive Wep App**: A starting manifest and service worker is setup that mirrors the default PWA template configuration but with Skip Waiting setup.
Skip Waiting ensures that any new versions of a service worker will take over the page and become activated immediately.

- **Security Headers**: Some headers have been added to make the application more secure by default.
These are configured in the staticwebapp.config.json file in the Client project.
  - [Content-Security-Policy](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy): Default to allowing HTTPS and localhost. 'unsafe-inline' 'unsafe-eval' are allowed for WASM compilation, Blazor compatibility and Hot Reload.  Objects are not allowed.
  - [Referrer-Policy](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy): Set to send no referrer information.
  - [Strict-Transport-Security](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Strict-Transport-Security): Expires in one year, applies to all sub-domains, allows submission to Google's HTST Preload list.
  - [X-Frame-Options](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options): Set to deny the site from loading in a frame.

- **Timeout**: HTTP client requests use a simple retry policy for failed network connections.

### Shared

- **DTOs**: Data Transfer Objects are used as needed to return lists of data.

- **Exceptions**: When exceptions happen in the API layer, a custom ErrorDetails class is returned.

- **Models**: Models inherit from a common base Model class.

- **Parameters**: Standard query parameters that can be passed to list based API requests.

- **Sanitizing**: Models should sanitize there input for HTML.

- **Services**: A date / time and guid service are available to make automated testing easier.

- **Validation**: If an invalid model is passed in a InvalidResults class is returned.

## Reference

These references are presented in a logical order for review from start to finish for gathering a deeper understanding of the code base.
There is an assumption that you are already familiar with C# and ASP.NET Core.

### Books & Series

- **[Microsoft Blazor: Building Web Applications in .NET 6 and Beyond](https://learning.oreilly.com/library/view/microsoft-blazor-building/9781484278451/)**: Reading this book helps you learn to build user interfaces and present data to a user for display and modification, capturing the user’s changes via data binding. The book shows how to access a rich library of .NET functionality such as a component model for building a composable user interface, including how to develop reusable components that can be used across many pages and websites. Also covered is data exchange with a server using REST, SignalR, and gRPC, giving you access to microservices and database services. 

- **[Ultimate ASP.NET Core Web API](https://code-maze.com/ultimate-aspnetcore-webapi-second-edition/)**: Second Edition of Code Maze's complete, hands-on, and easy to digest program that gets you from zero to building production-ready APIs in just a few weeks.

- **[Blazor WebAssembly - A Practical Guide to Full Stack Development With .NET](https://code-maze.com/blazor-webassembly-course/)**: The lessons start with the basic concepts of building a simple but practical application with Blazor WebAssembly. We cover all the things we need in order to build a real-world application and we quickly transition to building full-fledged application parts, slowly introducing the concepts as we go.

- **[Guide to NoSQL with Azure Cosmos DB](https://www.packtpub.com/product/guide-to-nosql-with-azure-cosmos-db/9781789612899)**: By the end of the book, you will be able to build an application that works with a Cosmos DB NoSQL document database with C#, the .NET Core SDK, LINQ, and JSON.

- **[Blazor Train](https://blazortrain.com/)**: Blazor Train is an extensive class on Microsoft Blazor, an extremely productive technology for developing web applications using HTML markup and the C# programming language.

- **[30 Days of SWA](https://www.azurestaticwebapps.dev/blog)**: Simply put, it's a month-long series of blog posts that provides you a curated and structured tour through the universe of Azure Static Web Apps.

### Articles & Videos

- **[Deploying Blazor WebAssembly into Azure Static Web Apps](https://code-maze.com/deploying-blazor-webassembly-into-azure-static-web-apps/)**: In this article, we are going to learn about Azure Static Web Apps, which is a great solution for publishing static web applications into the cloud.

- **[Azure Static Web Apps, Blazor, Authentication and Visual Studio 2022](https://scottkuhl.medium.com/azure-static-web-apps-blazor-authentication-and-visual-studio-2022-40364cc543b7)**: Azure Static Web Apps is a very easy way to create a secured Blazor Web Assembly application with a backend API based on Azure Functions. I will walk you through getting this all setup and running on your local workstation using Visual Studio 2022.

- **[Azure Cosmos DB with ASP.NET Core Web API](https://code-maze.com/azure-cosmos-db-with-asp-net-core-web-api/)**: We’ll start by learning what an Azure Cosmos DB is and the various APIs that it supports. Then we’ll create a Cosmos DB from the portal. After that, we’ll learn how to create an ASP.NET Core application that connects with the Cosmos DB using the Core (SQL) API. First, we’ll test it locally and then deploy it to Azure. Finally, we’ll verify that everything works well together in the cloud environment.

- **[Azure Functions (v3): Response Caching](https://tebest.net/2020/08/14/azure-functions-v3-response-caching/)**: This article describes how to add response caching to an Azure Functions v3 App. Response Caching can reduce the number of requests to your Functions significantly which helps you save costs and boosts performance.

- **[Azure functions. Limiting throughput and scalability of a serverless app](https://medium.com/microsoftazure/azure-functions-limiting-throughput-and-scalability-of-a-serverless-app-5b1c381491e3)**: You will get info on how to limit incoming incoming HTTP traffic to Serverless API and messages stream from Azure IoT Hub. And how to enforce restriction to Azure Functions scale-out and why singleton attribute might be needed.

- **[Blazor Material Table – Server-Side Paging, Searching, Sorting](https://code-maze.com/blazor-material-table-paging-searching-sorting/)**: Now, since we can navigate on our application, it is time to use the Blazor material table component, to display some data on our page.

- **[Blazor Material Form Creation with File Upload and Dialogs](https://code-maze.com/blazor-material-form-creation-with-file-upload-and-dialogs/)**: As a continuation, in this article, we are going to use the Blazor Material Form component, to create a form that handles product creation logic. We are going to add a file upload functionality to our form and show some messages using Dialog and Snackbar components.

- **[Azure BLOB Storage with ASP.NET Core and Angular](https://code-maze.com/azure-blob-storage-with-asp-net-core-and-angular/)**: In this article, we are going to take a look at the Azure Storage Platform and learn how to work with Azure BLOB Storage.

- **[Error Boundaries in .NET 6 Blazor: Carl Franklin's Blazor Train Ep 61](https://www.youtube.com/watch?v=aQ9n92TcRLE)**: Carl demonstrates how to use the new ErrorBoundary component to deal with unhandled exceptions.  

- **[Lazy Loading in Blazor WebAssembly](https://code-maze.com/lazy-loading-in-blazor-webassembly/)**: Lazy Loading enables us to improve the startup speed of our Blazor WebAssembly application by delaying the download of the resources we do not require yet.

- **[Blazor WebAssembly Component Virtualization with ASP.NET Core API](https://code-maze.com/blazor-webassembly-component-virtualization-with-web-api/)**: In this article, we are going to talk about Blazor WebAssembly component virtualization and explain how to use the Virtualize component to achieve that. Also, we are going to show you how to achieve the Blazor WebAssembly component virtualization while fetching the data from the web API project.

- **[Preventing double form submission in a Blazor application](https://www.meziantou.net/preventing-double-form-submission-in-a-blazor-application.htm)**: f the user quickly clicks on the button twice you may process the form multiple times simultaneously. If the form inserts a new record in a database, you'll get 2 records instead of a single one. You may want to prevent the user from submitting the form twice.

- **[HTTP Security Headers You Need To Have On Your Web Apps](https://www.youtube.com/watch?v=7MWXTXjtl8s):** In this session, we'll explain a handful of HTTP Security Headers (including HSTS, CSP, XFO, and more) from the bottom up. We'll explain what they are, what they do, and how you can implement them to secure your sites. On each of these, we'll demo a before and after so you can see first hand what each of these security headers do.  See also [.NET 6 Hot Reload and "Refused to connect to ws: because it violates the Content Security Policy directive" because Web Sockets](https://www.hanselman.com/blog/net-6-hot-reload-and-refused-to-connect-to-ws-because-it-violates-the-content-security-policy-directive-because-web-sockets).

- **[Blazor GDPR Consent Controls](https://github.com/majorimi/blazor-components/blob/master/.github/docs/GdprConsent.md)**: Blazor injectable service and components that renders a customizable GDPR consent Banner or Popup which stores the user's choice to Browser storage.

- **[Localization in Blazor WebAssembly Applications](https://code-maze.com/localization-in-blazor-webassembly-applications/)**: In this article, we are going to learn how to implement localization in Blazor WebAssembly applications. We are going to start with a simple introduction to Globalization and Localization. Then, we are going to show you how to implement localization in Blazor WebAssembly applications and also how to enable users to choose the culture they want.

### Documentation

- **[Blazor WebAssembly](https://docs.microsoft.com/aspnet/core/blazor/)**: Blazor is a framework for building interactive client-side web UI with .NET.

- **[MudBlazor](https://mudblazor.com/)**: MudBlazor is an ambitious Material Design component framework for Blazor with an emphasis on ease of use and clear structure.

- **[Azure Static Web Apps](https://docs.microsoft.com/en-us/azure/static-web-apps/)**: Azure Static Web Apps allows you to build modern web applications that automatically publish to the web as your code changes.

- **[Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/)**: Azure Functions is a cloud service available on-demand that provides all the continually updated infrastructure and resources needed to run your applications. You focus on the pieces of code that matter most to you, and Functions handles the rest. Functions provides serverless compute for Azure. You can use Functions to build web APIs, respond to database changes, process IoT streams, manage message queues, and more.

- **[Azure Cosmos DB](https://docs.microsoft.com/en-us/azure/cosmos-db/)**: Fast NoSQL database with SLA-backed speed and availability, automatic and instant scalability, and open-source APIs for MongoDB and Cassandra. 

- **[Azure Blob Storage](https://docs.microsoft.com/en-us/azure/storage/blobs/)**: Azure Blob Storage is Microsoft's object storage solution for the cloud. Blob storage is optimized for storing massive amounts of unstructured data.

### Other

- **[Azure Updates](https://azure.microsoft.com/en-gb/updates/?query=Static%20Web%20App)**: Keep up to date with the latest changes in Azure Static Web Apps.

- **[See Static Web Apps In Action](https://nitya.github.io/static-web-apps-gallery-code-samples/showcase)**: A community-contributed app showcase to learn from!

- **[Static Web App Dev Community](https://dev.to/t/staticwebapps)**: Static Web App blog posts from the Azure team.

- **[Devup 2022 - Blazor Azure Static Web Apps](https://github.com/scottkuhl/devup2022)**: Repo for the Devup 2022 talk on Blazor Azure Static Web Apps.