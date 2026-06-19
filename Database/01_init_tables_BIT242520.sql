USE MID_BIT242520;
GO

CREATE TABLE dbo.RoomTypes_BIT242520
(
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(1000) NULL,

    CONSTRAINT PK_RoomTypes_BIT242520
        PRIMARY KEY (Id),

    CONSTRAINT CK_RoomTypes_BIT242520_Name_NotEmpty
        CHECK (LEN(LTRIM(RTRIM(Name))) > 0),

    CONSTRAINT UQ_RoomTypes_BIT242520_Name
        UNIQUE (Name)
);
GO

CREATE TABLE dbo.Rooms_BIT242520
(
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(10,0) NOT NULL,
    Area DECIMAL(10,2) NOT NULL,
    IsAvailable BIT NOT NULL
        CONSTRAINT DF_Rooms_BIT242520_IsAvailable DEFAULT (1),
    Description NVARCHAR(1000) NULL,
    RoomTypeId INT NOT NULL,

    CONSTRAINT PK_Rooms_BIT242520
        PRIMARY KEY (Id),

    CONSTRAINT FK_Rooms_BIT242520_RoomTypes_BIT242520
        FOREIGN KEY (RoomTypeId)
        REFERENCES dbo.RoomTypes_BIT242520(Id),

    CONSTRAINT CK_Rooms_BIT242520_Name_NotEmpty
        CHECK (LEN(LTRIM(RTRIM(Name))) > 0),

    CONSTRAINT CK_Rooms_BIT242520_Price_Positive
        CHECK (Price > 0),

    CONSTRAINT CK_Rooms_BIT242520_Area_Positive
        CHECK (Area > 0),

    CONSTRAINT UQ_Rooms_BIT242520_RoomTypeId_Name
        UNIQUE (RoomTypeId, Name)
);
GO

CREATE TABLE dbo.RoomImages_BIT242520
(
    Id INT IDENTITY(1,1) NOT NULL,
    ImageUrl NVARCHAR(1000) NOT NULL,
    IsThumbnail BIT NOT NULL
        CONSTRAINT DF_RoomImages_BIT242520_IsThumbnail DEFAULT (0),
    RoomId INT NOT NULL,

    CONSTRAINT PK_RoomImages_BIT242520
        PRIMARY KEY (Id),

    CONSTRAINT FK_RoomImages_BIT242520_Rooms_BIT242520
        FOREIGN KEY (RoomId)
        REFERENCES dbo.Rooms_BIT242520(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_RoomImages_BIT242520_ImageUrl_NotEmpty
        CHECK (LEN(LTRIM(RTRIM(ImageUrl))) > 0)
);
GO

-- Mỗi phòng chỉ nên có tối đa 1 ảnh đại diện.
CREATE UNIQUE INDEX UX_RoomImages_BIT242520_OneThumbnailPerRoom
ON dbo.RoomImages_BIT242520(RoomId)
WHERE IsThumbnail = 1;
GO
