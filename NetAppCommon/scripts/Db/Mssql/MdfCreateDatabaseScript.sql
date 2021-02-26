BEGIN TRY
	IF EXISTS (SELECT * FROM sys.databases WHERE name = '%InitialCatalog%')
	BEGIN
		ALTER DATABASE %InitialCatalog% SET OFFLINE;
		EXEC sp_detach_db %InitialCatalog%;
	END
END TRY

BEGIN CATCH
	SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
END CATCH;

BEGIN TRY
	IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '%InitialCatalog%')
	BEGIN
		CREATE DATABASE %InitialCatalog%
		ON PRIMARY (
			NAME = '%InitialCatalog%',
			FILENAME = '%AttachDBFilename%',
			SIZE = %Size%,
			MAXSIZE = %MaxSize%,
			FILEGROWTH = %FileGrowTh%
		)
		LOG ON(
			NAME = '%LogInitialCatalog%',
			FILENAME = '%LogAttachDBFilename%',
			SIZE = %LogSize%,
			MAXSIZE = %LogMaxSize%,
			FILEGROWTH = %LogFileGrowTh%
		)
        FOR ATTACH
	END
END TRY

BEGIN CATCH
	SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
END CATCH;