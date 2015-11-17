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
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Order]         INT            NULL,
	[Title]	        NVARCHAR(MAX)  NULL,
    [Description]   NVARCHAR (MAX) NULL,
    [Value]         NVARCHAR (MAX) NULL,
    [ExamId]        INT            NULL,
    [Created]       DATETIME       DEFAULT (getutcdate()) NULL,
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

    DECLARE @timesToLoop INT
    DECLARE @innerLoopCount INT

    SET @timesToLoop = 0;
    SET @innerLoopCount = 0;

    WHILE @timesToLoop < 56
    BEGIN
		SET @timesToLoop = @timesToLoop + 1
        WHILE @innerLoopCount < 15
        BEGIN
            SET @innerLoopCount = @innerLoopCount + 1
            INSERT INTO Criteria ([Order], Title,[Description], Value, ExamId)
            VALUES (@innerLoopCount, 'De kandidaat moet voldoen aan:', 'Beschrijving van de vraag','Goed', @timesToLoop)
        END
        SET @innerLoopCount = 0
    END;
    GO
