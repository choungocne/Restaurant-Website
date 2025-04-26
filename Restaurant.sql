
-- Sử dụng database restaurant
USE Restaurant;
GO

-- 1. Bảng NHÂN VIÊN
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY,
    FullName NVARCHAR(255) NOT NULL,
    BirthDate DATE NOT NULL,
    PhoneNumber VARCHAR(15),
    Img VARCHAR(255),
    Detail NVARCHAR(MAX)
);

-- 2. Bảng KHÁCH HÀNG (có ảnh)
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY IDENTITY,
    CustomerName NVARCHAR(255) NOT NULL,
    Address NVARCHAR(255),
    PhoneNumber VARCHAR(15),
    Img VARCHAR(255) -- ảnh đại diện khách hàng
);

-- 3. Bảng TÀI KHOẢN NGƯỜI DÙNG (Login/Register)
CREATE TABLE UserAccount (
    UserID INT PRIMARY KEY IDENTITY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(100),
    Role VARCHAR(50) NOT NULL DEFAULT 'Customer',
    CreatedAt DATETIME DEFAULT GETDATE(),
    EmployeeID INT,
    CustomerID INT,
    FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);

-- 4. Danh mục món ăn
CREATE TABLE DishCategory (
    CategoryID INT PRIMARY KEY IDENTITY,
    CategoryName NVARCHAR(255) NOT NULL
);

-- 5. Món ăn
CREATE TABLE Dish (
    DishID INT PRIMARY KEY IDENTITY,
    DishName NVARCHAR(255) NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    CategoryID INT NOT NULL,
    FOREIGN KEY (CategoryID) REFERENCES DishCategory(CategoryID)
);

-- 6. Bàn ăn
CREATE TABLE DiningTable (
    TableID INT PRIMARY KEY IDENTITY,
    TableName VARCHAR(100),
    Location NVARCHAR(100)
);

-- 7. Dịch vụ gọi món (OrderService)
CREATE TABLE OrderService (
    ServiceID INT PRIMARY KEY IDENTITY,
    TableID INT NOT NULL,
    CustomerID INT NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME,
    FOREIGN KEY (TableID) REFERENCES DiningTable(TableID),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);

-- 8. Món ăn được gọi trong order
CREATE TABLE OrderDish (
    ServiceID INT,
    DishID INT,
    Quantity INT DEFAULT 1,
    UnitPrice DECIMAL(10,2),
    Note NVARCHAR(MAX),
    PRIMARY KEY (ServiceID, DishID),
    FOREIGN KEY (ServiceID) REFERENCES OrderService(ServiceID),
    FOREIGN KEY (DishID) REFERENCES Dish(DishID)
);

-- 9. Thanh toán
CREATE TABLE Payment (
    PaymentID INT PRIMARY KEY IDENTITY,
    ServiceID INT NOT NULL,
    EmployeeID INT NOT NULL,
    TotalAmount DECIMAL(10,2),
    Discount DECIMAL(5,2) DEFAULT 0.0,
    PaymentTime DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ServiceID) REFERENCES OrderService(ServiceID),
    FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID)
);

-- 10. Góp ý / phản hồi
CREATE TABLE Feedback (
    FeedbackID INT PRIMARY KEY IDENTITY,
    CustomerID INT,
    Content NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);
