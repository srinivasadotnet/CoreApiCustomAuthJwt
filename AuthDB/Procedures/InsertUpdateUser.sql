CREATE PROCEDURE InsertUpdateUser
@Id				INT = 0
,@FirstName		VARCHAR(100)
,@LastName		VARCHAR(100)
,@UserName		VARCHAR(30)
,@PasswordHash	BINARY(64)
,@PasswordSalt	BINARY(128)
AS
BEGIN
IF @Id = 0
BEGIN
	INSERT INTO AppUser 
	(
	 FirstName		
	,LastName		
	,UserName		
	,PasswordHash	
	,PasswordSalt	
	)
	VALUES
	(
	@FirstName		
	,@LastName		
	,@UserName		
	,@PasswordHash	
	,@PasswordSalt	
	)
END
ELSE
BEGIN
	UPDATE AppUser SET
	 FirstName		= @FirstName		
	,LastName		= @LastName		
	,UserName		= @UserName		
	,PasswordHash	= @PasswordHash
	,PasswordSalt	= @PasswordSalt
	WHERE Id = @Id
END
END