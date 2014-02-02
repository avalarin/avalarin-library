USE [master]
GO
/****** Object:  Database [AvalarinTestDatabase]    Script Date: 02.02.14 21:39:17 ******/
CREATE DATABASE [AvalarinTestDatabase]
 CONTAINMENT = PARTIAL
 ON  PRIMARY 
( NAME = N'AvalarinTestDatabase', FILENAME = N'D:\dev\avalarin\AvalarinTestDatabase.mdf' , SIZE = 4160KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'AvalarinTestDatabase_log', FILENAME = N'D:\dev\avalarin\AvalarinTestDatabase.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [AvalarinTestDatabase] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [AvalarinTestDatabase].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [AvalarinTestDatabase] SET ANSI_NULL_DEFAULT ON 
GO
ALTER DATABASE [AvalarinTestDatabase] SET ANSI_NULLS ON 
GO
ALTER DATABASE [AvalarinTestDatabase] SET ANSI_PADDING ON 
GO
ALTER DATABASE [AvalarinTestDatabase] SET ANSI_WARNINGS ON 
GO
ALTER DATABASE [AvalarinTestDatabase] SET ARITHABORT ON 
GO
ALTER DATABASE [AvalarinTestDatabase] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [AvalarinTestDatabase] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [AvalarinTestDatabase] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET CURSOR_DEFAULT  LOCAL 
GO
ALTER DATABASE [AvalarinTestDatabase] SET CONCAT_NULL_YIELDS_NULL ON 
GO
ALTER DATABASE [AvalarinTestDatabase] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET QUOTED_IDENTIFIER ON 
GO
ALTER DATABASE [AvalarinTestDatabase] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET  DISABLE_BROKER 
GO
ALTER DATABASE [AvalarinTestDatabase] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [AvalarinTestDatabase] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET RECOVERY FULL 
GO
ALTER DATABASE [AvalarinTestDatabase] SET  MULTI_USER 
GO
ALTER DATABASE [AvalarinTestDatabase] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [AvalarinTestDatabase] SET DB_CHAINING OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET DEFAULT_FULLTEXT_LANGUAGE = 1033 
GO
ALTER DATABASE [AvalarinTestDatabase] SET DEFAULT_LANGUAGE = 1033 
GO
ALTER DATABASE [AvalarinTestDatabase] SET NESTED_TRIGGERS = ON 
GO
ALTER DATABASE [AvalarinTestDatabase] SET TRANSFORM_NOISE_WORDS = OFF 
GO
ALTER DATABASE [AvalarinTestDatabase] SET TWO_DIGIT_YEAR_CUTOFF = 2049 
GO
ALTER DATABASE [AvalarinTestDatabase] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [AvalarinTestDatabase] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [AvalarinTestDatabase]
GO
/****** Object:  StoredProcedure [dbo].[ChangeDepartmentEmployeesCount]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ChangeDepartmentEmployeesCount]
	@departmentId int,
    @newCount int
AS
	UPDATE Department SET EmployeesCount = @newCount WHERE Id = @departmentId;
RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[ChangeEmployeeDepartment]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ChangeEmployeeDepartment]
	@employeeId int,
    @newDepartmentId int
AS
    UPDATE Employee SET DepartmentId = @newDepartmentId WHERE Id = @employeeId;
RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[ClearDatabase]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ClearDatabase]
AS
	DELETE FROM Employee;
    DELETE FROM Department;
RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[CreateDepartment]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CreateDepartment]
	@name nvarchar(50)
AS
	INSERT INTO Department (Name) VALUES (@name);
    EXEC GetDepartmentById @@IDENTITY;
RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[CreateEmployee]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CreateEmployee]
	@name nvarchar(50)
AS
	INSERT INTO Employee (Name) VALUES (@name);
    EXEC GetEmployeeById @@IDENTITY;
RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[GetDepartmentById]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetDepartmentById]
	@id int
AS
	SELECT TOP 1 Department.Id, Department.Name, ManagerId, Employee.Name AS 'ManagerName'
        FROM Department
        LEFT JOIN Employee ON Department.ManagerId = Employee.Id
        WHERE Department.Id = @id;
RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[GetDepartmentEmployeesCount]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetDepartmentEmployeesCount]
	@departmentId int
AS
	SELECT TOP 1 EmployeesCount FROM Department WHERE Id = @departmentId;
RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[GetDepartments]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetDepartments]
	@page int,
    @pageSize int,
    @totalCount int OUTPUT
AS
	SELECT Department.Id, Department.Name, ManagerId, Employee.Name AS 'ManagerName'
        FROM Department
        LEFT JOIN Employee ON Department.ManagerId = Employee.Id
        ORDER BY Department.Name
        OFFSET @page * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY;
    SELECT @totalCount = COUNT(*) FROM Department;
RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[GetEmployeeById]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetEmployeeById]
	@id int
AS
	SELECT TOP 1 Employee.Id, Employee.Name, DepartmentId, Department.Name AS 'DepartmentName'
        FROM Employee
        LEFT JOIN Department ON Employee.DepartmentId = Department.Id
        WHERE Employee.Id = @id;
RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[GetEmployees]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetEmployees]
    @page int,
    @pageSize int,
    @totalCount int OUTPUT
AS
	SELECT Employee.Id, Employee.Name, DepartmentId, Department.Name AS 'DepartmentName'
        FROM Employee
        LEFT JOIN Department ON Employee.DepartmentId = Department.Id
        ORDER BY Employee.Name
        OFFSET @page * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY;
    SELECT @totalCount = COUNT(*) FROM Employee;
RETURN 0
GO
/****** Object:  StoredProcedure [dbo].[SetDepartmentEmployeesCount]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SetDepartmentEmployeesCount]
	@departmentId int,
    @newCount int
AS
	UPDATE Department SET EmployeesCount = @newCount WHERE Id = @departmentId;
RETURN 0
GO
/****** Object:  Table [dbo].[Department]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Department](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ManagerId] [int] NULL,
	[EmployeesCount] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Employee]    Script Date: 02.02.14 21:39:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[DepartmentId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Department] ADD  DEFAULT ((0)) FOR [EmployeesCount]
GO
ALTER TABLE [dbo].[Department]  WITH CHECK ADD  CONSTRAINT [FK_Department_Employee] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Employee] ([Id])
ON UPDATE SET NULL
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Department] CHECK CONSTRAINT [FK_Department_Employee]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Department] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Department] ([Id])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Department]
GO
USE [master]
GO
ALTER DATABASE [AvalarinTestDatabase] SET  READ_WRITE 
GO
