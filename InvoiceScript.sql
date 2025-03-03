-- Create the database
CREATE DATABASE InvoiceDB;
GO

-- Use the newly created database
USE InvoiceDB;
GO

-- Set ANSI_NULLS and QUOTED_IDENTIFIER options
SET ANSI_NULLS ON;
GO


SET QUOTED_IDENTIFIER ON;
GO

-- Create the InvoiceHeader table
CREATE TABLE [dbo].[InvoiceHeader](
    [InvoiceId] [int] IDENTITY(1,1) NOT NULL,
    [InvoiceNumber] [varchar](50) NOT NULL,
    [InvoiceDate] [date] NULL,
    [Address] [varchar](50) NULL,
    [InvoiceTotal] [float] NULL,
    CONSTRAINT [PK_InvoiceHeader] PRIMARY KEY CLUSTERED 
    (
        [InvoiceId] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, 
            IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, 
            ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
GO

-- Set ANSI_NULLS and QUOTED_IDENTIFIER options again
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

-- Create the InvoiceLines table
CREATE TABLE [dbo].[InvoiceLines](
    [LineId] [int] IDENTITY(1,1) NOT NULL,
    [InvoiceNumber] [varchar](50) NOT NULL,
    [Description] [varchar](100) NULL,
    [Quantity] [float] NULL,
    [UnitSellingPriceExVAT] [float] NULL,
    CONSTRAINT [PK_InvoiceLines] PRIMARY KEY CLUSTERED 
    (
        [LineId] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, 
            IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, 
            ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
GO


ALTER TABLE [dbo].[InvoiceLines]
ADD [InvoiceId] [int] NOT NULL;

ALTER TABLE [dbo].[InvoiceLines]
ADD CONSTRAINT FK_InvoiceLines_InvoiceHeader FOREIGN KEY (InvoiceId) REFERENCES [dbo].[InvoiceHeader](InvoiceId);

