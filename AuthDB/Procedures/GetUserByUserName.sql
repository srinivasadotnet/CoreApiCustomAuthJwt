﻿CREATE PROCEDURE GetUserByUserName
@UserName		VARCHAR(30) = NULL,
@Id				INT = 0
AS
BEGIN
IF @Id = 0
BEGIN
 SELECT * FROM AppUser WHERE Username = @UserName
END
ELSE
BEGIN
SELECT * FROM AppUser WHERE Id = @Id
END
END