CREATE TABLE [dbo].[Teacher]
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Name] VARCHAR(50) NOT NULL,
    [Skills] VARCHAR(250) NOT NULL,
    [TotalStudents] INT NOT NULL,
    [Salary] MONEY NOT NULL,
    [AddedOn] DATE NOT NULL DEFAULT GETDATE()
)
