# Device Management Software

A comprehensive .NET Core MVC application for managing devices, users, and device assignments in an organization. Built following Clean Code principles and modern software architecture patterns.

## 🚀 Features

### Device Category Management
- ✅ Add, edit, delete device categories (Computer, Printer, Phone, etc.)
- ✅ Display list of all device categories
- ✅ View devices by category
- ✅ Prevent deletion of categories with assigned devices

### Device Management
- ✅ Add, edit, delete devices
- ✅ Device information includes:
  - Device name
  - Unique device code
  - Device category
  - Status (In Use, Broken, Under Maintenance, Available)
  - Date of entry
  - Location
  - Serial number
  - Description
- ✅ Display list of devices by category
- ✅ Advanced search and filtering by:
  - Device name or code
  - Category
  - Status
  - Location
- ✅ Pagination support
- ✅ Unique device code validation

### User Management
- ✅ Add, edit, delete users (employees)
- ✅ User information includes:
  - Full name
  - Email address
  - Phone number
  - Department
  - Position
- ✅ Soft delete (deactivate users instead of permanent deletion)
- ✅ Email uniqueness validation
- ✅ Device assignment tracking

### Device Assignment System
- ✅ Assign devices to users
- ✅ Track assignment history
- ✅ Return devices functionality
- ✅ Prevent multiple assignments of same device
- ✅ Automatic status updates

### Search and Filter Features
- ✅ Search devices by name or device code
- ✅ Filter devices by status or category
- ✅ Search users by name or email
- ✅ Filter users by department
- ✅ Real-time search with pagination

## 🛠 Technology Stack

- **Framework**: .NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core
- **Frontend**: Bootstrap 5, HTML5, CSS3, JavaScript
- **Icons**: Bootstrap Icons
- **Architecture**: Clean Architecture with Service Layer
- **Patterns**: Repository Pattern, Dependency Injection

## 📁 Project Structure

```
DeviceManagementSoftware/
├── Controllers/           # MVC Controllers
│   ├── HomeController.cs
│   ├── DeviceCategoryController.cs
│   ├── DeviceController.cs
│   └── UserController.cs
├── Models/               # Domain Models
│   ├── Device.cs
│   ├── DeviceCategory.cs
│   ├── User.cs
│   ├── UserDevice.cs
│   └── ViewModels/       # View Models for UI
├── Services/             # Business Logic Layer
│   ├── IDeviceService.cs
│   ├── DeviceService.cs
│   ├── IUserService.cs
│   ├── UserService.cs
│   ├── IDeviceCategoryService.cs
│   └── DeviceCategoryService.cs
├── Data/                 # Data Access Layer
│   └── ApplicationDbContext.cs
├── Views/               # MVC Views
│   ├── Home/
│   ├── Device/
│   ├── DeviceCategory/
│   ├── User/
│   └── Shared/
└── wwwroot/            # Static Files
    ├── css/
    ├── js/
    └── lib/
```

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone [repository-url]
   cd DeviceManagementSoftware
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string** (if needed)
   - Edit `appsettings.json`
   - Update the `DefaultConnection` string

4. **Create and update database**
   ```bash
   dotnet ef database update
   ```

5. **Build the project**
   ```bash
   dotnet build
   ```

6. **Run the application**
   ```bash
   dotnet run
   ```

7. **Open your browser**
   - Navigate to `http://localhost:5233` or `https://localhost:7239`

## 🔧 Configuration

### Database Configuration

The application uses SQL Server LocalDB by default. Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DeviceManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Seed Data

The application includes seed data for:
- 5 device categories (Computer, Printer, Phone, Monitor, Network Equipment)
- 3 sample users with different departments

## 📊 Database Schema

### Tables

1. **DeviceCategories**
   - Id, Name, Description, DateCreated

2. **Devices**
   - Id, Name, Code, CategoryId, Status, DateOfEntry, Description, Location, SerialNumber

3. **Users**
   - Id, FullName, Email, PhoneNumber, Department, Position, DateCreated, IsActive

4. **UserDevices** (Assignment tracking)
   - Id, UserId, DeviceId, AssignedDate, ReturnDate, Notes, IsActive

## 🎨 User Interface

### Dashboard
- Device statistics (Total, In Use, Available, Broken)
- User and category counts
- Quick action buttons for common tasks

### Device Management
- Advanced search and filtering
- Status badges with color coding
- Bulk operations support
- Responsive design

### User Management
- User profiles with device assignment history
- Device assignment/return workflow
- Department-based filtering

## 🔐 Features & Validation

### Data Validation
- Required field validation
- Email format validation
- Unique constraints (device codes, emails)
- Length restrictions
- Business rule validation

### Error Handling
- User-friendly error messages
- Constraint violation handling
- Graceful error recovery

### Security Features
- Input validation
- SQL injection prevention
- XSS protection
- CSRF tokens

## 🚀 Usage Examples

### Adding a New Device
1. Navigate to Devices → Add New Device
2. Fill in device information
3. Select category and status
4. Save the device

### Assigning a Device
1. Navigate to Users → Assign Device
2. Select user and available device
3. Add assignment notes (optional)
4. Confirm assignment

### Searching Devices
1. Use the search bar on Devices page
2. Apply filters by category, status, or location
3. Use pagination to navigate results

## 🔄 API Endpoints

The application follows RESTful conventions:

- `GET /Device` - List devices with search/filter
- `GET /Device/Create` - Create device form
- `POST /Device/Create` - Create device action
- `GET /Device/Edit/{id}` - Edit device form
- `POST /Device/Edit/{id}` - Update device action
- `GET /Device/Delete/{id}` - Delete confirmation
- `POST /Device/Delete/{id}` - Delete action

Similar patterns for DeviceCategory and User controllers.

## 📝 Clean Code Principles Applied

1. **Single Responsibility Principle**
   - Each service handles one domain area
   - Controllers focus on HTTP concerns only

2. **Dependency Injection**
   - Services are injected into controllers
   - Testable and maintainable code

3. **Separation of Concerns**
   - Business logic in services
   - Data access in repositories
   - UI logic in views and view models

4. **Validation**
   - Data annotations for model validation
   - Business rule validation in services

5. **Error Handling**
   - Consistent error messages
   - User-friendly feedback

## 🔮 Future Enhancements

- [ ] Role-based authentication and authorization
- [ ] Device maintenance scheduling
- [ ] Barcode/QR code generation for devices
- [ ] Email notifications for assignments
- [ ] Advanced reporting and analytics
- [ ] Export to Excel/PDF functionality
- [ ] API endpoints for mobile app integration
- [ ] Device photos and file attachments
- [ ] Audit trail for all changes

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📞 Support

For support and questions, please create an issue in the GitHub repository.

---

**Built with ❤️ using .NET Core MVC and Clean Architecture principles**
