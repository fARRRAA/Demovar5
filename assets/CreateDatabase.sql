-- ================================================
-- Скрипт создания базы данных для ООО "МебельОрг"
-- Демонстрационный экзамен 2026
-- Вариант № 5
-- ================================================

USE master;
GO

-- Удаление БД если существует
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'MebelOrgDB')
BEGIN
    ALTER DATABASE MebelOrgDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MebelOrgDB;
END
GO

-- Создание БД
CREATE DATABASE MebelOrgDB;
GO

USE MebelOrgDB;
GO

-- ================================================
-- СОЗДАНИЕ ТАБЛИЦ
-- ================================================

-- Таблица: Роли пользователей
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- Таблица: Пользователи
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    RoleID INT NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Login NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);
GO

-- Таблица: Категории товаров
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- Таблица: Производители
CREATE TABLE Manufacturers (
    ManufacturerID INT PRIMARY KEY IDENTITY(1,1),
    ManufacturerName NVARCHAR(150) NOT NULL UNIQUE
);
GO

-- Таблица: Поставщики
CREATE TABLE Suppliers (
    SupplierID INT PRIMARY KEY IDENTITY(1,1),
    SupplierName NVARCHAR(150) NOT NULL UNIQUE
);
GO

-- Таблица: Единицы измерения
CREATE TABLE Units (
    UnitID INT PRIMARY KEY IDENTITY(1,1),
    UnitName NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- Таблица: Товары
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ArticleNumber NVARCHAR(50) NOT NULL UNIQUE,
    ProductName NVARCHAR(300) NOT NULL,
    UnitID INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL CHECK (Price >= 0),
    SupplierID INT NOT NULL,
    ManufacturerID INT NOT NULL,
    CategoryID INT NOT NULL,
    Discount INT NOT NULL DEFAULT 0 CHECK (Discount >= 0 AND Discount <= 100),
    QuantityInStock INT NOT NULL DEFAULT 0 CHECK (QuantityInStock >= 0),
    Description NVARCHAR(MAX),
    PhotoPath NVARCHAR(500),
    CONSTRAINT FK_Products_Units FOREIGN KEY (UnitID) REFERENCES Units(UnitID),
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
    CONSTRAINT FK_Products_Manufacturers FOREIGN KEY (ManufacturerID) REFERENCES Manufacturers(ManufacturerID),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
GO

-- Таблица: Пункты выдачи
CREATE TABLE PickupPoints (
    PickupPointID INT PRIMARY KEY IDENTITY(1,1),
    Address NVARCHAR(300) NOT NULL UNIQUE
);
GO

-- Таблица: Заказы
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    OrderNumber INT NOT NULL UNIQUE,
    OrderDate DATE NOT NULL,
    DeliveryDate DATE NOT NULL,
    PickupPointID INT NOT NULL,
    UserID INT NULL,
    ClientFullName NVARCHAR(200) NOT NULL,
    PickupCode INT NOT NULL,
    OrderStatus NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Orders_PickupPoints FOREIGN KEY (PickupPointID) REFERENCES PickupPoints(PickupPointID),
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- Таблица: Состав заказа (связь многие-ко-многим)
CREATE TABLE OrderProducts (
    OrderProductID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    CONSTRAINT FK_OrderProducts_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
    CONSTRAINT FK_OrderProducts_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
GO

-- ================================================
-- ЗАПОЛНЕНИЕ СПРАВОЧНЫХ ДАННЫХ
-- ================================================

-- Заполнение таблицы Roles
INSERT INTO Roles (RoleName) VALUES 
(N'Администратор'),
(N'Менеджер'),
(N'Авторизированный клиент');
GO

-- Заполнение таблицы Units
INSERT INTO Units (UnitName) VALUES (N'шт.');
GO

-- Заполнение таблицы Categories
INSERT INTO Categories (CategoryName) VALUES 
(N'Прихожая'),
(N'Диван'),
(N'Обувница'),
(N'Пуф'),
(N'Полка'),
(N'Стул');
GO

-- Заполнение таблицы Manufacturers
INSERT INTO Manufacturers (ManufacturerName) VALUES 
(N'SVМЕБЕЛЬ'),
(N'Мебелони'),
(N'Инвуд'),
(N'RIDBERG');
GO

-- Заполнение таблицы Suppliers
INSERT INTO Suppliers (SupplierName) VALUES 
(N'Стройландия'),
(N'Кромма'),
(N'ЗолотоеРуно'),
(N'KRYLOVMANUFACTURA');
GO

-- ================================================
-- ЗАПОЛНЕНИЕ ДАННЫМИ ИЗ IMPORT-ФАЙЛОВ
-- ================================================

-- Заполнение таблицы Users (из user_import.xlsx)
INSERT INTO Users (RoleID, FullName, Login, Password) VALUES 
(1, N'Никифорова Анна Семеновна', N'94d5ous@gmail.com', N'uzWC67'),
(1, N'Стелина Евгения Петровна', N'uth4iz@mail.com', N'2L6KZG'),
(1, N'Никифорова Весения Николаевна', N'5d4zbu@tutanota.com', N'rwVDh9'),
(2, N'Сазонов Руслан Германович', N'ptec8ym@yahoo.com', N'LdNyos'),
(2, N'Одинцов Серафим Артёмович', N'1qz4kw@mail.com', N'gynQMT'),
(2, N'Старикова Елена Павловна', N'4np6se@mail.com', N'AtnDjr'),
(3, N'Степанов Михаил Артёмович', N'yzls62@outlook.com', N'JlFRCZ'),
(3, N'Михайлюк Анна Вячеславовна', N'1diph5e@tutanota.com', N'8ntwUp'),
(3, N'Ситдикова Елена Анатольевна', N'tjde7c@yahoo.com', N'YOyhfR'),
(3, N'Ворсин Петр Евгеньевич', N'wpmrc3do@tutanota.com', N'RSbvHv');
GO

-- Заполнение таблицы Products (из Tovar.xlsx)
INSERT INTO Products (ArticleNumber, ProductName, UnitID, Price, SupplierID, ManufacturerID, CategoryID, Discount, QuantityInStock, Description, PhotoPath) VALUES 
(N'А112Т4', N'Прихожая Фаворит 1 1420х2056х352ммм Дуб Делано/Цемент Светлый SV-М 1 шт', 1, 9577.00, 1, 1, 1, 10, 0, N'Удивительно функциональная и практичная прихожая Фаворит 1, обладая характерными чертами Скандинавского стиля, выглядит эффектно и способна задать тон интерьеру дома, встречая вас и ваших гостей.', N'1.jpg'),
(N'G843H5', N'Прихожая в коридор Твист с зеркалом мебель со шкафами, 120х37х202 см', 1, 8803.00, 1, 2, 1, 25, 9, N'Этот стеллаж со шкафом в прихожую комнату станет отличным элементом для вашего интерьера. Мебель для дома обеспечивает удобное хранение перчаток, шапок, зонтов, сумок и других аксессуаров.', N'2.jpg'),
(N'D325D4', N'Угловой диван Кромма Инвуд Лайт, серый двухместный диван, Velutto 32', 1, 29125.00, 2, 3, 2, 5, 12, N'Угловой диван Инвуд Лайт 2 - стильный и комфортный диван подойдет для комнаты любого размера.', N'3.jpg'),
(N'S432T5', N'Обувница RIDBERG, с вешалкой, стальная, 170x60x26 см, 5 полок, вместимость до 15 пар', 1, 885.00, 2, 4, 3, 15, 15, N'Обувница Ridberg с 5 полками и вешалкой - идеальное решение для организации хранения обуви в прихожей или гардеробной.', N'4.jpg'),
(N'F325D4', N'Диван, Прямой диван, Диван-кровать Сити темно-коричневый. Квест-33', 1, 14322.00, 3, 3, 2, 18, 3, N'Прямой диван-кровать Сити - это современное и функциональное решение для вашего дома.', N'5.jpg'),
(N'G432G6', N'Пуф трансформер кровать раскладушка светло-коричневый велюр', 1, 6149.00, 4, 3, 4, 22, 3, N'Пуф трансформер 5в1 представляет собой уникальное сочетание функций, выступая в качестве пуфика, столика, кресла, шезлонга и дополнительного спального места.', N'6.jpg'),
(N'H542F5', N'Диван, Прямой диван, диван кровать, Рио симпл механизм Пантограф. Симпл-16', 1, 20708.00, 3, 3, 2, 4, 5, N'Диван Рио симпл от "Золотое Руно" - это сочетание комфорта, функциональности и стильного дизайна.', N'7.jpg'),
(N'C346F5', N'Полка настенная ромб Лофт, черная, 40 см', 1, 2843.00, 4, 4, 5, 5, 4, N'Полочки для цветов в стиле лофт. Подойдут как для цветов, так и в качестве декоративного элемента. Полки подойдут для дома, офиса, кафе, ресторана.', N'8.jpg'),
(N'F256G6', N'Стулья для кухни', 1, 4760.00, 4, 4, 6, 6, 2, N'Набор из четырех стульев в лофт-дизайне станет любимой мебелью для отдыха и подойдет для взрослых и детей.', N'9.jpg'),
(N'J532V5', N'Магнитная полка, для холодильника, металл, 3шт, универсальная, чёрная', 1, 1387.00, 4, 4, 5, 8, 6, N'Магнитная полка для холодильника - это удобный и практичный аксессуар, который поможет организовать пространство в вашем доме.', N'10.jpg');
GO

-- Заполнение таблицы PickupPoints (из Пункты выдачи_import.xlsx)
INSERT INTO PickupPoints (Address) VALUES 
(N'420151, г. Лесной, ул. Вишневая, 32'),
(N'125061, г. Лесной, ул. Подгорная, 8'),
(N'630370, г. Лесной, ул. Шоссейная, 24'),
(N'400562, г. Лесной, ул. Зеленая, 32'),
(N'614510, г. Лесной, ул. Маяковского, 47'),
(N'410542, г. Лесной, ул. Светлая, 46'),
(N'620839, г. Лесной, ул. Цветочная, 8'),
(N'443890, г. Лесной, ул. Коммунистическая, 1'),
(N'603379, г. Лесной, ул. Спортивная, 46'),
(N'603721, г. Лесной, ул. Гоголя, 41'),
(N'410172, г. Лесной, ул. Северная, 13'),
(N'614611, г. Лесной, ул. Молодежная, 50'),
(N'454311, г.Лесной, ул. Новая, 19'),
(N'660007, г.Лесной, ул. Октябрьская, 19'),
(N'603036, г. Лесной, ул. Садовая, 4'),
(N'394060, г.Лесной, ул. Фрунзе, 43'),
(N'410661, г. Лесной, ул. Школьная, 50'),
(N'625590, г. Лесной, ул. Коммунистическая, 20'),
(N'625683, г. Лесной, ул. 8 Марта'),
(N'450983, г.Лесной, ул. Комсомольская, 26'),
(N'394782, г. Лесной, ул. Чехова, 3'),
(N'603002, г. Лесной, ул. Дзержинского, 28'),
(N'450558, г. Лесной, ул. Набережная, 30'),
(N'344288, г. Лесной, ул. Чехова, 1'),
(N'614164, г.Лесной,  ул. Степная, 30'),
(N'394242, г. Лесной, ул. Коммунистическая, 43'),
(N'660540, г. Лесной, ул. Солнечная, 25'),
(N'125837, г. Лесной, ул. Шоссейная, 40'),
(N'125703, г. Лесной, ул. Партизанская, 49'),
(N'625283, г. Лесной, ул. Победы, 46'),
(N'614753, г. Лесной, ул. Полевая, 35'),
(N'426030, г. Лесной, ул. Маяковского, 44'),
(N'450375, г. Лесной ул. Клубная, 44'),
(N'625560, г. Лесной, ул. Некрасова, 12'),
(N'630201, г. Лесной, ул. Комсомольская, 17'),
(N'190949, г. Лесной, ул. Мичурина, 26');
GO

-- Заполнение таблицы Orders (из Заказ_import.xlsx)
-- Связь клиентов с пользователями выполняется по ФИО
INSERT INTO Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointID, UserID, ClientFullName, PickupCode, OrderStatus) VALUES 
(1, '2024-02-27', '2024-04-20', 1, 7, N'Степанов Михаил Артёмович', 901, N'Новый'),
(2, '2024-09-28', '2024-04-21', 11, 8, N'Михайлюк Анна Вячеславовна', 902, N'Новый'),
(3, '2024-03-21', '2024-04-22', 2, 9, N'Ситдикова Елена Анатольевна', 903, N'Новый'),
(4, '2024-02-20', '2024-04-23', 11, 10, N'Ворсин Петр Евгеньевич', 904, N'Завершен'),
(5, '2024-03-17', '2024-04-24', 2, 7, N'Степанов Михаил Артёмович', 905, N'Завершен'),
(6, '2024-03-01', '2024-04-25', 15, 8, N'Михайлюк Анна Вячеславовна', 906, N'Завершен'),
(7, '2024-02-29', '2024-04-26', 3, 9, N'Ситдикова Елена Анатольевна', 907, N'Завершен'),
(8, '2024-03-31', '2024-04-27', 19, 10, N'Ворсин Петр Евгеньевич', 908, N'Новый'),
(9, '2024-04-02', '2024-04-28', 5, 9, N'Ситдикова Елена Анатольевна', 909, N'Новый'),
(10, '2024-04-03', '2024-04-29', 19, 10, N'Ворсин Петр Евгеньевич', 910, N'Новый');
GO

-- Заполнение таблицы OrderProducts
-- Парсинг артикулов из "Артикул заказа" (формат: ArticleNumber, Quantity, ArticleNumber, Quantity...)
INSERT INTO OrderProducts (OrderID, ProductID, Quantity) VALUES 
-- Заказ 1: А112Т4, 2, G843H5, 2
(1, (SELECT ProductID FROM Products WHERE ArticleNumber = N'А112Т4'), 2),
(1, (SELECT ProductID FROM Products WHERE ArticleNumber = N'G843H5'), 2),
-- Заказ 2: G843H5, 1, А112Т4, 1
(2, (SELECT ProductID FROM Products WHERE ArticleNumber = N'G843H5'), 1),
(2, (SELECT ProductID FROM Products WHERE ArticleNumber = N'А112Т4'), 1),
-- Заказ 3: D325D4, 10, S432T5, 10
(3, (SELECT ProductID FROM Products WHERE ArticleNumber = N'D325D4'), 10),
(3, (SELECT ProductID FROM Products WHERE ArticleNumber = N'S432T5'), 10),
-- Заказ 4: F325D4, 5, D325D4, 4
(4, (SELECT ProductID FROM Products WHERE ArticleNumber = N'F325D4'), 5),
(4, (SELECT ProductID FROM Products WHERE ArticleNumber = N'D325D4'), 4),
-- Заказ 5: G432G6, 20, H542F5, 20
(5, (SELECT ProductID FROM Products WHERE ArticleNumber = N'G432G6'), 20),
(5, (SELECT ProductID FROM Products WHERE ArticleNumber = N'H542F5'), 20),
-- Заказ 6: А112Т4, 2, G843H5, 2
(6, (SELECT ProductID FROM Products WHERE ArticleNumber = N'А112Т4'), 2),
(6, (SELECT ProductID FROM Products WHERE ArticleNumber = N'G843H5'), 2),
-- Заказ 7: G843H5, 1, А112Т4, 1
(7, (SELECT ProductID FROM Products WHERE ArticleNumber = N'G843H5'), 1),
(7, (SELECT ProductID FROM Products WHERE ArticleNumber = N'А112Т4'), 1),
-- Заказ 8: D325D4, 10, S432T5, 10
(8, (SELECT ProductID FROM Products WHERE ArticleNumber = N'D325D4'), 10),
(8, (SELECT ProductID FROM Products WHERE ArticleNumber = N'S432T5'), 10),
-- Заказ 9: F325D4, 5, D325D4, 4
(9, (SELECT ProductID FROM Products WHERE ArticleNumber = N'F325D4'), 5),
(9, (SELECT ProductID FROM Products WHERE ArticleNumber = N'D325D4'), 4),
-- Заказ 10: G432G6, 20, H542F5, 20
(10, (SELECT ProductID FROM Products WHERE ArticleNumber = N'G432G6'), 20),
(10, (SELECT ProductID FROM Products WHERE ArticleNumber = N'H542F5'), 20);
GO

-- ================================================
-- СОЗДАНИЕ ИНДЕКСОВ ДЛЯ ОПТИМИЗАЦИИ
-- ================================================

CREATE INDEX IX_Products_ArticleNumber ON Products(ArticleNumber);
CREATE INDEX IX_Products_CategoryID ON Products(CategoryID);
CREATE INDEX IX_Products_Discount ON Products(Discount);
CREATE INDEX IX_Users_Login ON Users(Login);
CREATE INDEX IX_Orders_OrderNumber ON Orders(OrderNumber);
CREATE INDEX IX_Orders_UserID ON Orders(UserID);
CREATE INDEX IX_OrderProducts_OrderID ON OrderProducts(OrderID);
CREATE INDEX IX_OrderProducts_ProductID ON OrderProducts(ProductID);
GO

PRINT 'База данных MebelOrgDB успешно создана и заполнена данными!';
GO
