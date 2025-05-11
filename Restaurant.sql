
-- Sử dụng database restaurant
USE Restaurant;
GO
-- 1. DishCategory
CREATE TABLE DishCategory (
    CategoryID INT PRIMARY KEY,
    CategoryName NVARCHAR(255) NOT NULL
);

-- 2. Dish
CREATE TABLE Dish (
    DishID INT PRIMARY KEY,
    DishName NVARCHAR(255) NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    Img VARCHAR(255),
    CreatedAt DATETIME NOT NULL,
    CategoryID INT NOT NULL,
    FOREIGN KEY (CategoryID) REFERENCES DishCategory(CategoryID)
);

-- 3. Customer
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY,
    CustomerName NVARCHAR(255) NOT NULL,
    Address NVARCHAR(255),
    PhoneNumber VARCHAR(15),
    Img VARCHAR(255),
    DateOfBirth DATE
);

-- 4. DiningTable
CREATE TABLE DiningTable (
    TableID INT PRIMARY KEY,
    TableName VARCHAR(100),
    Location NVARCHAR(100),
    Img VARCHAR(255),
    Quantity INT NOT NULL,
    NumberOfCustomer INT NOT NULL,
    IsAvailable BIT DEFAULT 1
);

-- 5. Employee
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY,
    FullName NVARCHAR(255),
    BirthDate DATE,
    PhoneNumber VARCHAR(15),
    Img VARCHAR(255),
    Detail NVARCHAR(MAX)
);

-- 6. OrderService
CREATE TABLE OrderService (
    ServiceID INT PRIMARY KEY,
    TableID INT NOT NULL,
    CustomerID INT NOT NULL,
    StartTime DATETIME,
    EndTime DATETIME,
    FOREIGN KEY (TableID) REFERENCES DiningTable(TableID),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);

-- 7. OrderDish
CREATE TABLE OrderDish (
    ServiceID INT NOT NULL,
    DishID INT NOT NULL,
    Quantity INT DEFAULT 1,
    UnitPrice DECIMAL(10, 2),
    Note NVARCHAR(MAX),
    PRIMARY KEY (ServiceID, DishID),
    FOREIGN KEY (ServiceID) REFERENCES OrderService(ServiceID),
    FOREIGN KEY (DishID) REFERENCES Dish(DishID)
);

-- 8. Payment
CREATE TABLE Payment (
    PaymentID INT PRIMARY KEY,
    ServiceID INT NOT NULL,
    EmployeeID INT NOT NULL,
    TotalAmount DECIMAL(10, 2),
    Discount DECIMAL(5, 2) DEFAULT 0,
    PaymentTime DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ServiceID) REFERENCES OrderService(ServiceID),
    FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID)
);

-- 9. UserAccount
CREATE TABLE UserAccount (
    UserID INT PRIMARY KEY,
    Username VARCHAR(50) UNIQUE,
    PasswordHash VARCHAR(255),
    Email VARCHAR(100),
    Role VARCHAR(50) DEFAULT 'Customer',
    CreatedAt DATETIME DEFAULT GETDATE(),
    CustomerID INT,
    EmployeeID INT,
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID)
);

-- 10. TableReservation
CREATE TABLE TableReservation (
    ReservationID INT PRIMARY KEY,
    CustomerID INT NOT NULL,
    TableID INT NOT NULL,
    ServiceID INT,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    EmployeeID INT,
    Status INT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (TableID) REFERENCES DiningTable(TableID),
    FOREIGN KEY (ServiceID) REFERENCES OrderService(ServiceID),
    FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID)
);

-- 11. Feedback
CREATE TABLE Feedback (
    FeedbackID INT PRIMARY KEY,
    CustomerID INT,
    ReservationID INT NOT NULL,
    Content NVARCHAR(MAX),
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (ReservationID) REFERENCES TableReservation(ReservationID)
);
INSERT INTO DiningTable (TableID, TableName, Location, Img, Quantity, NumberOfCustomer, IsAvailable) VALUES
(1, N'Bàn 1', N'Tầng trệt - góc trái', 'table-1.png', 12, 4, 1),
(2, N'Bàn 2', N'Tầng trệt - gần cửa sổ', 'table-2.jpg', 10, 4, 1),
(3, N'Bàn 3', N'Lầu 1 - giữa phòng', 'table-3.jpg', 10, 6, 1),
(4, N'Bàn VIP', N'Phòng riêng', 'table-vip.png', 5, 20, 1);
INSERT INTO DishCategory (CategoryID, CategoryName) VALUES
(1, N'Khai vị'),
(2, N'Món chính'),
(3, N'Tráng miệng'),
(4, N'Đồ uống');
INSERT INTO Dish (DishID, DishName, UnitPrice, CategoryID, Img, CreatedAt) VALUES
(1, N'Gỏi cuốn', 30000, 1, 'menu-item-2.png', GETDATE()),
(2, N'Đậu hủ sốt Tứ Xuyên', 45000, 2, 'menu-item-1.png', GETDATE()),
(3, N'Gỏi thái', 60000, 1, 'menu-item-3.png', GETDATE()),
(4, N'Salad rau củ', 55000, 1, 'menu-item-4.png', GETDATE()),
(5, N'BeefSteak', 230000, 2, 'menu-item-5.png', GETDATE()),
(6, N'Salad gà', 35000, 1, 'menu-item-6.png', GETDATE()),
(7, N'Chè dưỡng nhan', 25000, 3, 'menu-item-7.png', GETDATE()),
(8, N'Mì nước Dương Châu', 20000, 2, 'menu-item-8.jpg', GETDATE()),
(58, N'Hủ tiếu', 150000, 2, 'item-1.jpg', GETDATE());
INSERT INTO UserAccount (UserID, Username, PasswordHash, Email, Role, CreatedAt, EmployeeID) VALUES
(1, 'hoaitran', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'hoaitran@example.com', 'Employee', GETDATE(), 1),//123456
(2, 'thangnguyen', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'thang@example.com', 'Employee', GETDATE(), 3),
(3, 'vana', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'vana@example.com', 'Chef', GETDATE(), 4),
(4, 'levanc', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'levanc@example.com', 'Waiter', GETDATE(), 6),
(5, 'phamthid', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'dthu@example.com', 'Cashier', GETDATE(), 7),
(6, 'admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'admin@example.com', 'Admin', GETDATE(), 10);//admin123

