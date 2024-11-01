USE [master]
GO
/****** Object:  Database [MicroCreditDB]    Script Date: 01/11/2024 22:53:16 ******/
ALTER DATABASE [MicroCreditDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MicroCreditDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MicroCreditDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MicroCreditDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MicroCreditDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MicroCreditDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MicroCreditDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [MicroCreditDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MicroCreditDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MicroCreditDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MicroCreditDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MicroCreditDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MicroCreditDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MicroCreditDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MicroCreditDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MicroCreditDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MicroCreditDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MicroCreditDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MicroCreditDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MicroCreditDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MicroCreditDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MicroCreditDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MicroCreditDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MicroCreditDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MicroCreditDB] SET RECOVERY FULL 
GO
ALTER DATABASE [MicroCreditDB] SET  MULTI_USER 
GO
ALTER DATABASE [MicroCreditDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MicroCreditDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MicroCreditDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MicroCreditDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [MicroCreditDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [MicroCreditDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'MicroCreditDB', N'ON'
GO
ALTER DATABASE [MicroCreditDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [MicroCreditDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [MicroCreditDB]
GO
/****** Object:  Table [dbo].[Credits]    Script Date: 01/11/2024 22:53:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Credits](
	[credit_id] [nvarchar](max) NOT NULL,
	[NIF] [nvarchar](max) NOT NULL,
	[credit_taken] [float] NOT NULL,
	[credit_payed] [float] NOT NULL,
	[credit_request_date] [date] NOT NULL,
	[credit_term] [date] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GovPT]    Script Date: 01/11/2024 22:53:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GovPT](
	[NIF] [varchar](max) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[monthly_income] [float] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[GetCreditLimit]    Script Date: 01/11/2024 22:53:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetCreditLimit]
    @NIF VARCHAR(MAX),  
    @CreditLimit FLOAT OUTPUT  
AS
BEGIN
	DECLARE @MonthlyIncome FLOAT;
	DECLARE @MissingCredit FLOAT;
	DECLARE @SUMCreditTaken FLOAT;
	DECLARE @SUMCreditPayed FLOAT;

    Select @MonthlyIncome = monthly_income
	FROM GovPT
	WHERE NIF = @NIF;

	Select @SUMCreditTaken = SUM(credit_taken), @SUMCreditPayed = SUM(credit_payed)
	FROM Credits
	WHERE NIF = @NIF

	IF @MonthlyIncome IS NULL
	BEGIN
		SET @CreditLimit = 0;
		RETURN -1;
	END
	IF @SUMCreditPayed IS NULL OR @SUMCreditTaken IS NULL
		SET @MissingCredit = 0;
	ELSE
		SET @MissingCredit = @SUMCreditTaken - @SUMCreditPayed;

	IF @MonthlyIncome < 0
		SET @CreditLimit = 0;
    ELSE IF @MonthlyIncome <= 1000
        SET @CreditLimit = 1000 - @MissingCredit; --DUE TO OTHER SYSTEM CONDITIONS I DOUBT THAT THIS WILL EVER TURN NEGATIVE
    ELSE IF @MonthlyIncome > 1000 AND @MonthlyIncome <= 2000
        SET @CreditLimit = 2000 - @MissingCredit;
    ELSE
        SET @CreditLimit = 5000 - @MissingCredit;

	RETURN 0;
END;
GO
/****** Object:  StoredProcedure [dbo].[PayCredit]    Script Date: 01/11/2024 22:53:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PayCredit]
    @NIF VARCHAR(MAX),               
    @credit_id VARCHAR(MAX),         
    @paymentAmount FLOAT,            
    @newCreditPayed FLOAT OUTPUT     
AS
BEGIN
    DECLARE @currentCreditPayed FLOAT;
    DECLARE @currentCreditTaken FLOAT;
    DECLARE @outstandingBalance FLOAT;
	DECLARE @outstandingBalanceText VARCHAR(20);
  
    SELECT @currentCreditTaken = credit_taken, 
           @currentCreditPayed = credit_payed
    FROM Credits
    WHERE nif = @NIF AND credit_id = @credit_id;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('No credit record found for the specified NIF and Credit ID.', 16, 1);
        RETURN; 
    END

    SET @outstandingBalance = @currentCreditTaken - @currentCreditPayed;
	SET @outstandingBalanceText = CONVERT(VARCHAR(20), @outstandingBalance);

    IF @paymentAmount > @outstandingBalance
    BEGIN
        RAISERROR('Payment exceeds the outstanding balance. Current balance is %s.', 16, 1, @outstandingBalanceText);
        RETURN; 
    END

    UPDATE Credits
    SET credit_payed = credit_payed + @paymentAmount
    WHERE nif = @NIF AND credit_id = @credit_id;

    SELECT @newCreditPayed = (credit_taken - credit_payed)
    FROM Credits
    WHERE nif = @NIF AND credit_id = @credit_id;
END;
GO
USE [master]
GO
ALTER DATABASE [MicroCreditDB] SET  READ_WRITE 
GO
