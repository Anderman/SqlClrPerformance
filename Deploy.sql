DROP PROCEDURE [dbo].[SqlStoredProcedure1];
GO
DROP ASSEMBLY [SqlSP];
GO
CREATE ASSEMBLY [SqlSP] FROM 'c:\git\sqlsp\SqlClrPerformance\bin\release\testapp.exe' WITH PERMISSION_SET = UNSAFE;
GO
CREATE PROCEDURE [dbo].[SqlStoredProcedure1] AS EXTERNAL NAME [SqlSP].[testApp.StoredProcedures].SqlStoredProcedure1
GO
exec [dbo].[SqlStoredProcedure1] 