USE master

IF EXISTS(select * from sys.databases where name='ExcelsisDb')
DROP DATABASE ExcelsisDb;

CREATE DATABASE ExcelsisDb;
GO

USE ExcelsisDb;

CREATE TABLE [dbo].[Exams] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (MAX) NULL,
    [Cohort]        NVARCHAR (MAX) NULL,
    [Crebo]         NVARCHAR (MAX) NULL,
    [Subject]       NVARCHAR (MAX) NULL,
	[Status]		NVARCHAR (MAX) Null,
    [Created]       DATETIME       DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Criteria] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Order]       INT            NULL,
    [Title]       NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Value]       NVARCHAR (MAX) NULL,
    [ExamId]      INT            NULL,
    [CategoryId]  INT            NULL,
    [Created]     DATETIME       DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Assessors] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [UserName]      NVARCHAR (MAX) NULL,
    [Email]         NVARCHAR (MAX) NULL,
    [Created]       DATETIME       DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[AssessmentsAssessors] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Assessment_Id] INT            NULL,
    [Assessor_Id]   INT            NULL,
    CONSTRAINT [PK_AssessmentsAssessors] PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Assessments] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [StudentName]   NVARCHAR (MAX) NULL,
    [StudentNumber] NVARCHAR (MAX) NULL,
    [Assessed]      DATETIME       NULL,
    [Exam_Id]       INT            NULL,
    [Created]       DATETIME       DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Observations] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Criterion_Id]  INT            NULL,
    [Result]        NVARCHAR (MAX) NULL,
    [Marks]         NVARCHAR (MAX) NULL,
    [Assessment_Id] INT            NULL,
    [Created]       DATETIME       DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Categories]
(
    [Id]            INT            NOT NULL PRIMARY KEY IDENTITY, 
    [Order]         INT            NULL, 
    [Name]          NVARCHAR (MAX) NULL,
	[ExamId]        INT            NULL,
	[Created]       DATETIME       DEFAULT (getutcdate()) NULL,
)

GO
INSERT INTO Assessors (UserName, Email)
    VALUES
    ('joostronkesagerbeek', 'joostronkesagerbeek@davinci.nl'),
    ('petersnoek', 'petersnoek@davinci.nl'),
    ('fritssilano', 'fritssilano@davinci.nl'),
    ('chantaltouw', 'chantaltouw@davinci.nl');

INSERT INTO Exams (Name, Cohort, Crebo, [Subject], [Status])
    VALUES 
    ('Spreken','2015','','Nederlands', 'draft'),
    ('Lezen & Luisteren','2015','','Nederlands', 'draft'),
    ('Gesprekken voeren','2015','','Nederlands', 'draft'),
    ('Schrijven','2015','','Nederlands', 'draft'),
    ('Spreken','2015','','Engels', 'draft'),
    ('Gesprekken voeren','2015','','Engels', 'draft'),
    ('Schrijven','2015','','Engels', 'draft'),
    ('Lezen & Luisteren','2015','','Engels', 'draft'),
    ('Schrijven','2015','','Engels', 'draft'),
    ('Ontwerpen van een applicatie','2015','246859','Applicatieontwikkeling', 'draft'),
    ('Realiseren van een applicatie','2015','246859','Applicatieontwikkeling', 'draft'),
    ('Opleveren van een applicatie','2015','246859','Applicatieontwikkeling', 'draft'),
    ('Hoofdrekenen','2015','','Rekenen', 'draft'),
    ('Getallen','2015','','Rekenen', 'draft'),
    ('Spreken','2014','','Nederlands', 'published'),
    ('Lezen & Luisteren','2014','','Nederlands', 'published'),
    ('Gesprekken voeren','2014','','Nederlands', 'published'),
    ('Schrijven','2014','','Nederlands', 'published'),
    ('Spreken','2014','','Engels', 'published'),
    ('Gesprekken voeren','2014','','Engels', 'published'),
    ('Schrijven','2014','','Engels', 'published'),
    ('Lezen & Luisteren','2014','','Engels', 'published'),
    ('Schrijven','2014','','Engels', 'published'),
    ('Ontwerpen van een applicatie','2014','246859','Applicatieontwikkeling', 'published'),
    ('Realiseren van een applicatie','2014','246859','Applicatieontwikkeling', 'published'),
    ('Opleveren van een applicatie','2014','246859','Applicatieontwikkeling', 'published'),
    ('Hoofdrekenen','2014','','Rekenen', 'published'),
    ('Getallen','2014','','Rekenen', 'published'),
    ('Spreken','2013','','Nederlands', 'published'),
    ('Lezen & Luisteren','2013','','Nederlands', 'published'),
    ('Gesprekken voeren','2013','','Nederlands', 'published'),
    ('Schrijven','2013','','Nederlands', 'published'),
    ('Spreken','2013','','Engels', 'published'),
    ('Gesprekken voeren','2013','','Engels', 'published'),
    ('Schrijven','2013','','Engels', 'published'),
    ('Lezen & Luisteren','2013','','Engels', 'published'),
    ('Schrijven','2013','','Engels', 'published'),
    ('Ontwerpen van een applicatie','2013','246859','Applicatieontwikkeling', 'published'),
    ('Realiseren van een applicatie','2013','246859','Applicatieontwikkeling', 'published'),
    ('Opleveren van een applicatie','2013','246859','Applicatieontwikkeling', 'published'),
    ('Hoofdrekenen','2013','','Rekenen', 'published'),
    ('Getallen','2013','','Rekenen', 'published'),
    ('Spreken','2012','','Nederlands', 'published'),
    ('Lezen & Luisteren','2012','','Nederlands', 'published'),
    ('Gesprekken voeren','2012','','Nederlands', 'published'),
    ('Schrijven','2012','','Nederlands', 'published'),
    ('Spreken','2012','','Engels', 'published'),
    ('Gesprekken voeren','2012','','Engels', 'published'),
    ('Schrijven','2012','','Engels', 'published'),
    ('Lezen & Luisteren','2012','','Engels', 'published'),
    ('Schrijven','2012','','Engels', 'published'),
    ('Ontwerpen van een applicatie','2012','246859','Applicatieontwikkeling', 'published'),
    ('Realiseren van een applicatie','2012','246859','Applicatieontwikkeling', 'published'),
    ('Opleveren van een applicatie','2012','246859','Applicatieontwikkeling', 'published'),
    ('Hoofdrekenen','2012','','Rekenen', 'published'),
    ('Getallen','2012','','Rekenen', 'published');

	GO

	DECLARE @ExamLoopCount INT;
    DECLARE @CategoryLoopCount INT;
    DECLARE @CriteriaLoopCount INT;
    DECLARE @ExamLoopCountMAX INT;
    DECLARE @CategoryLoopCountMAX INT;
    DECLARE @CriteriaLoopCountMAX INT;
	DECLARE @CategoryId INT;

    SET @ExamLoopCount = 0;
    SET @CategoryLoopCount = 0;
    SET @CriteriaLoopCount = 0;

    SET @ExamLoopCountMAX = 56;
    SET @CategoryLoopCountMAX = 3;
    SET @CriteriaLoopCountMAX = 5;

	

    WHILE @ExamLoopCount < @ExamLoopCountMAX
    BEGIN
        SET @ExamLoopCount = @ExamLoopCount + 1

        WHILE @CategoryLoopCount < @CategoryLoopCountMAX
        BEGIN
			SET @CategoryLoopCount = @CategoryLoopCount + 1

			INSERT INTO Categories ([Order], [Name], [ExamId])
			VALUES (@CategoryLoopCount, 'categorie ' + CAST ( @CategoryLoopCount AS nvarchar(max) ) + '..............', @ExamLoopCount)
		    SET @CategoryId = (
				SELECT @@IDENTITY
			)

            WHILE @CriteriaLoopCount < @CriteriaLoopCountMAX
            BEGIN
				SET @CriteriaLoopCount = @CriteriaLoopCount + 1

				INSERT INTO Criteria ([Order], [Title], [Description], [Value], [ExamId], [CategoryId])
				VALUES (@CriteriaLoopCount, 'De kandidaat moet voldoen aan:', 'Beschrijving van de vraag','Goed', @ExamLoopCount, @CategoryId)
            END
			SET @CriteriaLoopCount = 0
        END
        SET @CategoryLoopCount = 0
    END;
    GO