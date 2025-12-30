# Backend Developer Code Challenge

## Introduction
Welcome to the Backend Developer Technical Assessment! This test is designed to evaluate your proficiency in building REST APIs using .NET 8, focusing on clean architecture, business logic, and testing practices. We have prepared a set of tasks and questions that cover a spectrum of skills, ranging from fundamental concepts to more advanced topics.

**Note:** This assessment focuses on API development, architecture, and testing. During the interview, we'll discuss your experience with databases, event-driven design, Docker/Kubernetes, and cloud platforms.

## Tasks
Complete the provided tasks to demonstrate your ability to work with .NET 8, ASP.NET Core Web API, and unit testing. Adjust the complexity based on your experience level.

## Questions
Answer the questions to showcase your understanding of the underlying concepts and best practices associated with the technologies in use.

## Time Limit
This assessment is designed to take approximately 1-2 hours to complete. Please manage your time effectively.

## Setup the Repository
Make sure you have .NET 8 SDK installed
- Install dependencies with `dotnet restore`
- Build the project with `dotnet build`
- Run the project with `dotnet run --project CodeChallenge.Api`
- Navigate to `https://localhost:5095/swagger` to see the API documentation

## Prerequisite
Start the test by forking this repository, and complete the following tasks:

---

## Task 1
**Assignment:** Implement a REST API with CRUD operations for messages. Use the provided `IMessageRepository` and models to create a `MessagesController` with these endpoints:
- `GET /api/v1/organizations/{organizationId}/messages` - Get all messages for an organization
- `GET /api/v1/organizations/{organizationId}/messages/{id}` - Get a specific message
- `POST /api/v1/organizations/{organizationId}/messages` - Create a new message
- `PUT /api/v1/organizations/{organizationId}/messages/{id}` - Update a message
- `DELETE /api/v1/organizations/{organizationId}/messages/{id}` - Delete a message

**Question 1:** Describe your implementation approach and the key decisions you made.

**Question 2:** What would you improve or change if you had more time?

commit the code as task-1

---

## Task 2
**Assignment:** Separate business logic from the controller and add proper validation.
1. Implement `MessageLogic` class (implement `IMessageLogic`)
2. Implement Business Rules:
   - Title must be unique per organization
   - Content must be between 10 and 1000 characters
   - Title is required and must be between 3 and 200 characters
   - Can only update or delete messages that are active (`IsActive = true`)
   - UpdatedAt should be set automatically on updates
3. Return appropriate result types (see `Logic/Results.cs`)
4. Update Controller to use `IMessageLogic` instead of directly using the repository

**Question 3:** How did you approach the validation requirements and why?

**Question 4:** What changes would you make to this implementation for a production environment?

commit the code as task-2

---

## Task 3
**Assignment:** Write comprehensive unit tests for your business logic.
1. Create `CodeChallenge.Tests` project (xUnit)
2. Add required packages: xUnit, Moq, FluentAssertions
3. Write Tests for MessageLogic covering these scenarios:
   - Test successful creation of a message
   - Test duplicate title returns Conflict
   - Test invalid content length returns ValidationError
   - Test update of non-existent message returns NotFound
   - Test update of inactive message returns ValidationError
   - Test delete of non-existent message returns NotFound

**Question 5:** Explain your testing strategy and the tools you chose.

**Question 6:** What other scenarios would you test in a real-world application?

commit the code as task-3


Answers to Questions above :-

Question 1: Describe your implementation approach and the key decisions you made.
Answer 1: In implementing the MessagesController, I followed RESTful principles to ensure that 
each endpoint corresponds to a specific action on the message resource. IMessageRepository was already injected into the controller, promoting loose coupling and easier testing. 
Each CRUD operation was implemented with appropriate HTTP methods (GET, POST, PUT, DELETE) and status codes 
to reflect the outcome of each operation. Business Validations was not added explicitly but whatever the asp.net core provides by default.

Question 2: What would you improve or change if you had more time?
Answer 2:With more time, I would move business rules and validation logic out of the controller into a dedicated logic 
or service layer to improve separation of concerns and testability. 
I would also introduce structured validation, consistent error handling, and unit tests for the business logic.
For a production environment, I would additionally consider persistence beyond in-memory storage, 
improved logging and observability, and better handling of edge cases.

Question 3: How did you approach the validation requirements and why?
Answer 3: I centralized validation inside the business logic layer to ensure that all rules are enforced consistently, 
regardless of how the logic is consumed. This keeps controllers thin and focused on HTTP concerns while 
making the validation logic easier to test and maintain. Grouping validation errors also allows multiple issues to be 
reported in a single response which was provided and asked as part of the Results record class.

Question 4: What changes would you make to this implementation for a production environment?
Answer 4: For a production environment, I would implement persistent storage (e.g., a database) instead of in-memory storage 
to ensure data durability. I would also enhance security measures, such as authentication and authorization, to protect
sensitive data and operations. Additionally, I would incorporate logging and monitoring to track 
application behavior and performance in real-time.

Question 5: Explain your testing strategy and the tools you chose.
Answer 5: I focused my tests on the business logic layer because it contains all validation rules and decision-making logic. 
By testing the logic layer in isolation, I can verify behavior deterministically 
without involving controllers, HTTP concerns, or infrastructure.
I used xUnit as the test framework due to its simplicity and strong integration with the .NET ecosystem. 
Moq was used to mock the repository dependency so that each test controls the data conditions explicitly and remains fast and reliable.
FluentAssertions was chosen to make assertions more expressive and readable, especially when validating result types and 
expected outcomes.
Each test targets a single business rule or scenario, keeping the tests focused and easy to understand.

Question 6: What other scenarios would you test in a real-world application?
Answer 6: In a real-world application, I would extend testing to cover the following areas:
Boundary and edge cases: Testing minimum and maximum title and content lengths, empty or malformed inputs, and invalid identifiers.
Concurrency scenarios: Verifying correct behavior when multiple requests attempt to create, update, or delete messages simultaneously.
Integration tests: Ensuring business logic, repositories, and the persistence layer work correctly together, including database constraints and transactions.
API-level tests: Validating request and response contracts, HTTP status codes, and error responses.
Security and authorization: Testing access control rules, authorization boundaries, and protection against unauthorized operations.
Performance and load testing: Evaluating system behavior under high request volumes and large payloads.
Resilience and failure handling: Testing partial failures, retries, and graceful error handling under unexpected conditions.
