DECLARE @LastId INT;
SELECT @LastId = MAX(Id) FROM [dbo].[Books];

IF @LastId IS NOT NULL
BEGIN
    DBCC CHECKIDENT ('Books', RESEED, @LastId);
END
ELSE
BEGIN
    -- If no rows exist, reseed to start from 1
    DBCC CHECKIDENT ('Users', RESEED, 0);
END;

DECLARE @LastId INT;
SELECT @LastId = MAX(Id) FROM [dbo].[Books];

IF @LastId IS NOT NULL
BEGIN
    DBCC CHECKIDENT ('Users', RESEED, @LastId);
END
ELSE
BEGIN
    -- If no rows exist, reseed to start from 1
    DBCC CHECKIDENT ('Users', RESEED, 0);
END;