--New Database: HoangHaPCVN
-- Bang User: quan ly ca tai khoan cua Khach hang va Admin
CREATE TABLE [dbo].[User] (
    [Username]  NVARCHAR (255) NOT NULL,
    [Password]  NVARCHAR (255) NOT NULL,
    [UserRole]  NVARCHAR (20)  NOT NULL, -- 'Admin' hoac 'Customer'
    PRIMARY KEY CLUSTERED ([Username] ASC)
);

--Bang Customer: luu thong tin khach hang
CREATE TABLE [dbo].[Customer] (
    [CustomerID]      INT IDENTITY (1, 1) NOT NULL,
    [CustomerName]    NVARCHAR (MAX) NOT NULL,
    [CustomerPhone]   NVARCHAR (15)  NOT NULL,
    [CustomerEmail]   NVARCHAR (MAX) NULL,
    [CustomerAddress] NVARCHAR (MAX) NULL,
    [Username]        NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([CustomerID] ASC),
    CONSTRAINT [FK_User_Customer] FOREIGN KEY ([Username]) REFERENCES [dbo].[User] ([Username])
);

--Bang Category: luu thong tin danh muc san pham
CREATE TABLE [dbo].[Category] (
    [CategoryID]   INT IDENTITY (1, 1) NOT NULL,
    [CategoryName] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([CategoryID] ASC)
);

--Bang Product: luu thong tin san pham
CREATE TABLE [dbo].[Product] (
    [ProductID]          INT IDENTITY (1, 1) NOT NULL,
    [CategoryID]         INT             NOT NULL,
    [ProductName]        NVARCHAR (MAX)  NOT NULL,
    [ProductDescription] NVARCHAR (MAX)  NOT NULL,
    [ProductPrice]       DECIMAL (18, 2) NOT NULL,
    [ProductImage]       NVARCHAR (MAX)  NULL,
    PRIMARY KEY CLUSTERED ([ProductID] ASC),
    CONSTRAINT [FK_Pro_Category] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category] ([CategoryID])
);

--Bang Order: luu thong tin don hang cua khach hang
CREATE TABLE [dbo].[Order] (
    [OrderID]         INT IDENTITY (1, 1) NOT NULL,
    [CustomerID]      INT             NOT NULL,
    [OrderDate]       DATETIME        NOT NULL,
    [TotalAmount]     DECIMAL (18, 2) NOT NULL,
    [PaymentStatus]   NVARCHAR (MAX)  NULL,
    [ShippingAddress] NVARCHAR (MAX)  NOT NULL,
    [DeliveryMethod]  NVARCHAR (100)  NULL,
    [PaymentMethod]   NVARCHAR (100)  NULL,
    [OrderStatus]     NVARCHAR (50)   NULL, -- Cho thanh toan, Dang xu ly, Dang van chuyen, Da giao, Da huy
    PRIMARY KEY CLUSTERED ([OrderID] ASC),
    FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([CustomerID])
);

--Bang OrderDetail: luu thong tin chi tiet cac san pham co trong don hang
CREATE TABLE [dbo].[OrderDetail] (
    [ID]         INT IDENTITY (1, 1) NOT NULL,
    [ProductID]  INT             NOT NULL,
    [OrderID]    INT             NOT NULL,
    [Quantity]   INT             NOT NULL,
    [UnitPrice]  DECIMAL (18, 2) NOT NULL,
    [TotalPrice] AS ([Quantity] * [UnitPrice]) PERSISTED,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ProductID]),
    FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([OrderID])
);

-- Du lieu ban dau: 1 tai khoan admin va 8 danh muc theo sidebar trang chu
INSERT INTO [dbo].[User] VALUES (N'admin', N'123456', N'Admin');
INSERT INTO [dbo].[Category] (CategoryName) VALUES
 (N'PC Giá Rẻ'), (N'CPU - Bộ Vi Xử Lý'), (N'VGA - Card Màn Hình'), (N'Linh Kiện Máy Tính'),
 (N'Màn Hình Máy Tính'), (N'Tản Nhiệt Cooling'), (N'Gaming Gear'), (N'Thiết Bị Mạng');

ALTER AUTHORIZATION ON DATABASE::HoangHaPCVN TO sa;