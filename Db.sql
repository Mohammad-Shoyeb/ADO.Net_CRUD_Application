Create Table Students
(
	StudentId INT PRIMARY KEY,
	StudentName NVARCHAR(40) NOT NULL,
	DateofBirth DATE NOT NULL,
	IsContinuing BIT NOT NULL,
	Phone NVARCHAR (20)  NOT NULL,
	Email NVARCHAR(50) NOT NULL,
	Picture NVARCHAR(150) NOT NULL
	)
GO

Create Table Subjects
(
	SubjectId INT PRIMARY KEY ,
	SubjectName NVARCHAR(50) NOT NULL,
	NumberOfStudent INT NOT NULL,
	StudentId INT NOT NULL REFERENCES Students(StudentId)
	)
GO