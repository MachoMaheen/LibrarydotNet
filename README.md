# Library Management System

A comprehensive library management system built with ASP.NET Core 8.0 (backend) and HTML/CSS/JavaScript (frontend).

## Features

### User Roles
- **Admin**: Full system access including book inventory management
- **Librarian**: Issue and return books, manage fines
- **Member**: Browse books, view borrowed books, pay fines

### Core Functionality
- **User Authentication & Authorization**: JWT-based authentication with role-based access control
- **Book Management**: Add, update, delete, and search books
- **Issue/Return System**: Track book loans with due dates
- **Fine Management**: Automatic fine calculation for overdue books ($5 per day)
- **Search & Filter**: Search books by title, author, ISBN, or category
- **Responsive UI**: Mobile-friendly design with modern CSS

## Technology Stack

### Backend
- ASP.NET Core 8.0 Web API
- Entity Framework Core 8.0
- SQLite Database
- JWT Authentication
- BCrypt for password hashing

### Frontend
- HTML5
- CSS3 (Responsive Design)
- Vanilla JavaScript
- Fetch API for HTTP requests

## Database Schema

### Users Table
- UserId, Name, Email, Password (hashed), Role, Phone, Address, CreatedDate, IsActive

### Books Table
- BookId, ISBN, Title, Author, Publisher, PublishedYear, Category, TotalCopies, AvailableCopies, Description, AddedDate, IsActive

### BookIssues Table
- IssueId, BookId, UserId, IssueDate, DueDate, ReturnDate, Status, IssuedByLibrarianId

### Fines Table
- FineId, IssueId, UserId, Amount, Status, CreatedDate, PaidDate, Reason

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- A modern web browser

### Installation & Running

1. Clone the repository:
```bash
git clone https://github.com/MachoMaheen/LibrarydotNet.git
cd LibrarydotNet/LibraryManagement
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the application:
```bash
dotnet run
```

4. Access the application:
```
http://localhost:5000
```

### Default Admin Credentials
- Email: `admin@library.com`
- Password: `admin123`

## Project Structure

```
LibraryManagement/
├── Controllers/         # API Controllers (Auth, Books, Issues, Fines)
├── Models/             # Database Models (User, Book, BookIssue, Fine)
├── DTOs/               # Data Transfer Objects
├── Services/           # Business Logic Services
├── Data/               # DbContext and Migrations
├── wwwroot/            # Static Files
│   ├── css/           # Stylesheets
│   ├── js/            # JavaScript files
│   ├── index.html     # Homepage
│   ├── login.html     # Login page
│   ├── register.html  # Registration page
│   ├── member-dashboard.html
│   ├── librarian-dashboard.html
│   └── admin-dashboard.html
└── Program.cs          # Application entry point
```

## API Endpoints

### Authentication
- `POST /api/Auth/login` - User login
- `POST /api/Auth/register` - User registration

### Books
- `GET /api/Books` - Get all books
- `GET /api/Books/search` - Search books
- `GET /api/Books/{id}` - Get book by ID
- `POST /api/Books` - Add new book (Admin only)
- `PUT /api/Books/{id}` - Update book (Admin only)
- `DELETE /api/Books/{id}` - Delete book (Admin only)

### Issues
- `POST /api/Issues/issue` - Issue a book (Librarian/Admin)
- `POST /api/Issues/return` - Return a book (Librarian/Admin)
- `GET /api/Issues/user/{userId}` - Get user's issues
- `GET /api/Issues/active` - Get all active issues (Librarian/Admin)

### Fines
- `GET /api/Fines/user/{userId}` - Get user's fines
- `GET /api/Fines/all` - Get all fines (Librarian/Admin)
- `POST /api/Fines/pay` - Pay a fine

## Validation Rules

### User Registration
- Name: Required, max 100 characters
- Email: Required, valid email format
- Password: Required, minimum 6 characters
- Phone: Optional, max 15 characters
- Address: Optional, max 200 characters

### Book Creation
- ISBN: Required, max 13 characters
- Title: Required, max 200 characters
- Author: Required, max 100 characters
- Total Copies: Required, 1-1000

### Issue Book
- Book must be available
- User must not have pending fines
- Default loan period: 14 days

## Security Features
- Password hashing using BCrypt
- JWT token-based authentication
- Role-based authorization
- CORS enabled for frontend-backend communication
- Input validation on both client and server side

## Future Enhancements
- Email notifications for due dates
- Book reservations
- User profile management
- Reports and analytics
- Barcode scanning support
- Multi-library support

## License
This project is open source and available under the MIT License.

## Contributors
- MachoMaheen

## Support
For issues and questions, please open an issue on GitHub.