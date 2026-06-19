USE MID_BIT242520;
GO

/* Dữ liệu mẫu tối thiểu: 3 loại phòng, 5 phòng */
INSERT INTO dbo.RoomTypes_BIT242520 (Name, Description)
VALUES
    (N'Phòng đơn', N'Phòng dành cho một người'),
    (N'Phòng đôi', N'Phòng dành cho hai người'),
    (N'Phòng cao cấp', N'Phòng có diện tích lớn và tiện nghi tốt hơn');
GO

INSERT INTO dbo.Rooms_BIT242520
    (Name, Price, Area, IsAvailable, Description, RoomTypeId)
VALUES
    (N'Phòng 101', 1500000, 18.50, 1, N'Phòng đơn cơ bản, phù hợp một người ở', 1),
    (N'Phòng 102', 1650000, 20.00, 1, N'Phòng đơn có cửa sổ', 1),
    (N'Phòng 201', 2500000, 28.00, 1, N'Phòng đôi rộng rãi', 2),
    (N'Phòng 202', 2700000, 30.00, 0, N'Phòng đôi đang được giữ chỗ', 2),
    (N'Phòng 301', 4000000, 40.00, 1, N'Phòng cao cấp đầy đủ tiện nghi', 3);
GO

INSERT INTO dbo.RoomImages_BIT242520
    (ImageUrl, IsThumbnail, RoomId)
VALUES
    (N'/images/rooms/101-main.jpg', 1, 1),
    (N'/images/rooms/101-2.jpg', 0, 1),
    (N'/images/rooms/102-main.jpg', 1, 2),
    (N'/images/rooms/201-main.jpg', 1, 3),
    (N'/images/rooms/202-main.jpg', 1, 4),
    (N'/images/rooms/301-main.jpg', 1, 5),
    (N'/images/rooms/301-2.jpg', 0, 5);
GO

/* Query kiểm tra nhanh */
SELECT COUNT(*) AS TotalRoomTypes
FROM dbo.RoomTypes_BIT242520;

SELECT COUNT(*) AS TotalRooms
FROM dbo.Rooms_BIT242520;

SELECT
    r.Id,
    r.Name,
    r.Price,
    r.Area,
    CAST(r.Price / r.Area AS DECIMAL(18,2)) AS PricePerSquareMeter,
    r.IsAvailable,
    rt.Name AS RoomTypeName
FROM dbo.Rooms_BIT242520 AS r
INNER JOIN dbo.RoomTypes_BIT242520 AS rt
    ON r.RoomTypeId = rt.Id
ORDER BY r.Id;
GO