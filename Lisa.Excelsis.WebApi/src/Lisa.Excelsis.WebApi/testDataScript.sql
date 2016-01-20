USE master

IF EXISTS(select * from sys.databases where name='ExcelsisDb')
DROP DATABASE ExcelsisDb;

CREATE DATABASE ExcelsisDb;
GO

USE ExcelsisDb;

CREATE TABLE [dbo].[Exams] (
    [Id]            INT				IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (MAX)	NULL,
    [NameId]        NVARCHAR (MAX)	NULL,
    [Cohort]        NVARCHAR (MAX)	NULL,
    [Crebo]         NVARCHAR (MAX)	NULL,
    [Subject]       NVARCHAR (MAX)	NULL,
    [SubjectId]     NVARCHAR (MAX)	NULL,
	[Status]		NVARCHAR (MAX)	Null,
    [Created]       DATETIME		DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Criteria] (
    [Id]			INT				IDENTITY (1, 1) NOT NULL,
    [Order]			INT				NULL,
    [Title]			NVARCHAR (MAX)	NULL,
    [Description]	NVARCHAR (MAX)	NULL,
    [Weight]		NVARCHAR (MAX)	NULL,
    [ExamId]		INT				NULL,
    [CategoryId]	INT				NULL,
    [Created]		DATETIME		DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Assessors] (
    [Id]            INT				IDENTITY (1, 1) NOT NULL,
    [UserName]      NVARCHAR (MAX)	NULL,
    [Email]         NVARCHAR (MAX)	NULL,
    [Created]       DATETIME		DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[AssessmentAssessors] (
    [Id]            INT				IDENTITY (1, 1) NOT NULL,
    [AssessmentId]	INT				NULL,
    [AssessorId]	INT				NULL,
    CONSTRAINT [PK_AssessmentsAssessors] PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Assessments] (
    [Id]            INT				IDENTITY (1, 1) NOT NULL,
    [StudentName]   NVARCHAR (MAX)	NULL,
    [StudentNumber] NVARCHAR (MAX)	NULL,
    [Assessed]      NVARCHAR (MAX)	NULL,
    [Name]          NVARCHAR (MAX)	NULL,   
    [Cohort]        NVARCHAR (MAX)	NULL,
    [Crebo]         NVARCHAR (MAX)	NULL,
    [Subject]       NVARCHAR (MAX)	NULL,
    [Created]       DATETIME		DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Observations] (
    [Id]            INT				IDENTITY (1, 1) NOT NULL,
	[AssessmentId]	INT				NULL,
    [CategoryId]	INT				NULL,
	[Order]			INT				NULL,
    [Title]			NVARCHAR (MAX)	NULL,
    [Description]	NVARCHAR (MAX)	NULL,
    [Weight]		NVARCHAR (MAX)	NULL,
    [Result]        NVARCHAR (MAX)	NULL,    
    [Created]       DATETIME		DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Marks] (
    [Id]            INT				IDENTITY (1, 1) NOT NULL,
	[ObservationId]	INT				NULL,
	[AssessmentId]	INT				NULL,
	[Name]          NVARCHAR (MAX)	NULL,
	[Created]       DATETIME		DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Categories]
(
    [Id]            INT				NOT NULL PRIMARY KEY IDENTITY, 
    [Order]         INT				NULL, 
    [Name]          NVARCHAR (MAX)	NULL,
	[ExamId]        INT				NULL,
	[Created]       DATETIME		DEFAULT (getutcdate()) NULL,
)

CREATE TABLE [dbo].[AssessmentCategories]
(
    [Id]            INT				NOT NULL PRIMARY KEY IDENTITY, 
    [Order]         INT				NULL, 
    [Name]          NVARCHAR (MAX)	NULL,
	[AssessmentId]  INT				NULL,
	[Created]       DATETIME		DEFAULT (getutcdate()) NULL,
)


GO
INSERT INTO Assessors ([UserName], [Email])
    VALUES
    ('joostronkesagerbeek', 'joostronkesagerbeek@davinci.nl'),
    ('petersnoek', 'petersnoek@davinci.nl'),
    ('fritssilano', 'fritssilano@davinci.nl'),
    ('chantaltouw', 'chantaltouw@davinci.nl');

INSERT INTO Exams ([Name], [Cohort], [Crebo], [Subject], [SubjectId], [NameId], [Status])
    VALUES 
    ('Spreken','2015','','Nederlands', 'nederlands', 'spreken', 'draft'),
    ('Lezen & Luisteren','2015','','Nederlands', 'nederlands', 'lezen-luisteren', 'draft'),
    ('Gesprekken voeren','2015','','Nederlands', 'nederlands', 'gesprekken-voeren', 'draft'),
    ('Schrijven','2015','','Nederlands', 'nederlands', 'schrijven', 'draft'),
    ('Spreken','2014','','Nederlands', 'nederlands', 'spreken', 'published'),
    ('Lezen & Luisteren','2014','','Nederlands', 'nederlands', 'lezen-luisteren', 'published'),
    ('Gesprekken voeren','2014','','Nederlands', 'nederlands', 'gesprekken-voeren', 'published'),
    ('Schrijven','2014','','Nederlands', 'nederlands', 'schrijven', 'published'),
    ('Spreken','2013','','Nederlands', 'nederlands', 'spreken', 'published'),
    ('Lezen & Luisteren','2013','','Nederlands', 'nederlands', 'lezen-luisteren', 'published'),
    ('Gesprekken voeren','2013','','Nederlands', 'nederlands', 'gesprekken-voeren', 'published'),
    ('Schrijven','2013','','Nederlands', 'nederlands', 'schrijven', 'published'),
    ('Spreken','2012','','Nederlands', 'nederlands', 'spreken', 'published'),
    ('Lezen & Luisteren','2012','','Nederlands', 'nederlands', 'lezen-luisteren', 'published'),
    ('Gesprekken voeren','2012','','Nederlands', 'nederlands', 'gesprekken-voeren', 'published'),
    ('Schrijven','2012','','Nederlands', 'nederlands', 'schrijven', 'published'),

    ('Spreken','2015','','Engels', 'engels', 'spreken', 'draft'),
    ('Gesprekken voeren','2015','','Engels', 'engels', 'gesprekken-voeren', 'draft'),
    ('Schrijven','2015','','Engels', 'engels', 'schrijven', 'draft'),
    ('Lezen & Luisteren','2015','','Engels', 'engels', 'lezen-luisteren', 'draft'),
    ('Schrijven','2015','','Engels', 'engels', 'schrijven', 'draft'),
    ('Spreken','2014','','Engels', 'engels', 'spreken', 'published'),
    ('Gesprekken voeren','2014','','Engels', 'engels', 'gesprekken-voeren', 'published'),
    ('Schrijven','2014','','Engels', 'engels', 'schrijven', 'published'),
    ('Lezen & Luisteren','2014','','Engels', 'engels', 'lezen-luisteren', 'published'),
    ('Schrijven','2014','','Engels', 'engels', 'schrijven', 'published'),
    ('Spreken','2013','','Engels', 'engels', 'spreken', 'published'),
    ('Gesprekken voeren','2013','','Engels', 'engels', 'gesprekken-voeren', 'published'),
    ('Schrijven','2013','','Engels', 'engels', 'schrijven', 'published'),
    ('Lezen & Luisteren','2013','','Engels', 'engels', 'lezen-luisteren', 'published'),
    ('Schrijven','2013','','Engels', 'engels', 'schrijven', 'published'),
    ('Spreken','2012','','Engels', 'engels', 'spreken', 'published'),
    ('Gesprekken voeren','2012','','Engels', 'engels', 'gesprekken-voeren', 'published'),
    ('Schrijven','2012','','Engels', 'engels', 'schrijven', 'published'),
    ('Lezen & Luisteren','2012','','Engels', 'engels', 'lezen-luisteren', 'published'),
    ('Schrijven','2012','','Engels', 'engels', 'schrijven', 'published'),

    ('Hoofdrekenen','2015','','Rekenen', 'rekenen', 'hoofdrekenen', 'draft'),
    ('Getallen','2015','','Rekenen', 'rekenen', 'getallen', 'draft'),
    ('Hoofdrekenen','2014','','Rekenen', 'rekenen', 'hoofdrekenen', 'published'),
    ('Getallen','2014','','Rekenen', 'rekenen', 'getallen', 'published'),
    ('Hoofdrekenen','2013','','Rekenen', 'rekenen', 'hoofdrekenen', 'published'),
    ('Getallen','2013','','Rekenen', 'rekenen', 'getallen', 'published'),
    ('Hoofdrekenen','2012','','Rekenen', 'rekenen', 'hoofdrekenen', 'published'),
    ('Getallen','2012','','Rekenen', 'rekenen', 'getallen', 'published'),

    ('Ontwerpen van een applicatie','2015','246859','Applicatieontwikkeling', 'applicatieontwikkeling', 'ontwerpen-van-een-applicatie', 'draft'),
    ('Realiseren van een applicatie','2015','246859','Applicatieontwikkeling', 'applicatieontwikkeling', 'realiseren-van-een-applicatie', 'draft'),
    ('Opleveren van een applicatie','2015','246859','Applicatieontwikkeling', 'applicatieontwikkeling', 'opleveren-van-een-applicatie', 'draft'),
    ('Ontwerpen van een applicatie','2014','246859','Applicatieontwikkeling', 'applicatieontwikkeling', 'ontwerpen-van-een-applicatie', 'published'),
    ('Realiseren van een applicatie','2014','246859','Applicatieontwikkeling', 'applicatieontwikkeling', 'realiseren-van-een-applicatie', 'published'),
    ('Opleveren van een applicatie','2014','246859','Applicatieontwikkeling', 'applicatieontwikkeling', 'opleveren-van-een-applicatie', 'published'),
    ('Ontwerpen van een applicatie','2013','246859','Applicatieontwikkeling', 'applicatieontwikkeling', 'ontwerpen-van-een-applicatie', 'published'),
    ('Realiseren van een applicatie','2013','246859','Applicatieontwikkeling', 'applicatieontwikkeling', 'realiseren-van-een-applicatie', 'published'),
    ('Opleveren van een applicatie','2013','246859','Applicatieontwikkeling', 'applicatieontwikkeling', 'opleveren-van-een-applicatie', 'published'),
    ('Ontwerpen van een applicatie','2012','246859','Applicatieontwikkeling', 'applicatieontwikkeling', 'ontwerpen-van-een-applicatie', 'published'),
    ('Realiseren van een applicatie','2012','246859','Applicatieontwikkeling', 'applicatieontwikkeling', 'realiseren-van-een-applicatie', 'published'),
    ('Opleveren van een applicatie','2012','246859','Applicatieontwikkeling', 'applicatieontwikkeling', 'opleveren-van-een-applicatie', 'published');

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