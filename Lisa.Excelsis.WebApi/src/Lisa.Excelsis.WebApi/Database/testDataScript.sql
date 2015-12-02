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
    [Created]       DATETIME       DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Criteria] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Order]       INT            NULL,
    [Title]       NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Weight]       NVARCHAR (MAX) NULL,
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
    [Assessment_Id] INT            NULL,
    [Created]       DATETIME       DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Marks] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
	[Observation_Id]INT            NULL,
	[Name]          NVARCHAR (MAX) NULL,
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

INSERT INTO Exams (Name, Cohort, Crebo, [Subject])
    VALUES 
    ('Spreken','2015','','Nederlands'),
    ('Lezen & Luisteren','2015','','Nederlands'),
    ('Gesprekken voeren','2015','','Nederlands'),
    ('Schrijven','2015','','Nederlands'),
    ('Spreken','2015','','Engels'),
    ('Gesprekken voeren','2015','','Engels'),
    ('Schrijven','2015','','Engels'),
    ('Lezen & Luisteren','2015','','Engels'),
    ('Schrijven','2015','','Engels'),
    ('Ontwerpen van een applicatie','2015','246859','Applicatieontwikkeling'),
    ('Realiseren van een applicatie','2015','246859','Applicatieontwikkeling'),
    ('Opleveren van een applicatie','2015','246859','Applicatieontwikkeling'),
    ('Hoofdrekenen','2015','','Rekenen'),
    ('Getallen','2015','','Rekenen'),
    ('Spreken','2014','','Nederlands'),
    ('Lezen & Luisteren','2014','','Nederlands'),
    ('Gesprekken voeren','2014','','Nederlands'),
    ('Schrijven','2014','','Nederlands'),
    ('Spreken','2014','','Engels'),
    ('Gesprekken voeren','2014','','Engels'),
    ('Schrijven','2014','','Engels'),
    ('Lezen & Luisteren','2014','','Engels'),
    ('Schrijven','2014','','Engels'),
    ('Ontwerpen van een applicatie','2014','246859','Applicatieontwikkeling'),
    ('Realiseren van een applicatie','2014','246859','Applicatieontwikkeling'),
    ('Opleveren van een applicatie','2014','246859','Applicatieontwikkeling'),
    ('Hoofdrekenen','2014','','Rekenen'),
    ('Getallen','2014','','Rekenen'),
    ('Spreken','2013','','Nederlands'),
    ('Lezen & Luisteren','2013','','Nederlands'),
    ('Gesprekken voeren','2013','','Nederlands'),
    ('Schrijven','2013','','Nederlands'),
    ('Spreken','2013','','Engels'),
    ('Gesprekken voeren','2013','','Engels'),
    ('Schrijven','2013','','Engels'),
    ('Lezen & Luisteren','2013','','Engels'),
    ('Schrijven','2013','','Engels'),
    ('Ontwerpen van een applicatie','2013','246859','Applicatieontwikkeling'),
    ('Realiseren van een applicatie','2013','246859','Applicatieontwikkeling'),
    ('Opleveren van een applicatie','2013','246859','Applicatieontwikkeling'),
    ('Hoofdrekenen','2013','','Rekenen'),
    ('Getallen','2013','','Rekenen'),
    ('Spreken','2012','','Nederlands'),
    ('Lezen & Luisteren','2012','','Nederlands'),
    ('Gesprekken voeren','2012','','Nederlands'),
    ('Schrijven','2012','','Nederlands'),
    ('Spreken','2012','','Engels'),
    ('Gesprekken voeren','2012','','Engels'),
    ('Schrijven','2012','','Engels'),
    ('Lezen & Luisteren','2012','','Engels'),
    ('Schrijven','2012','','Engels'),
    ('Ontwerpen van een applicatie','2012','246859','Applicatieontwikkeling'),
    ('Realiseren van een applicatie','2012','246859','Applicatieontwikkeling'),
    ('Opleveren van een applicatie','2012','246859','Applicatieontwikkeling'),
    ('Hoofdrekenen','2012','','Rekenen'),
    ('Getallen','2012','','Rekenen');

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

				INSERT INTO Criteria ([Order], [Title], [Description], [Weight], [ExamId], [CategoryId])
				VALUES (@CriteriaLoopCount, 'De kandidaat moet voldoen aan:', 'Beschrijving van de vraag','excellent', @ExamLoopCount, @CategoryId)
            END
			SET @CriteriaLoopCount = 0
        END
        SET @CategoryLoopCount = 0
    END;
    GO