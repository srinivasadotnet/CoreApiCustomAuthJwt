CREATE TABLE AppUser
(
	Id				INT IDENTITY(1,1) PRIMARY KEY
	,FirstName		VARCHAR(100)
	,LastName		VARCHAR(100)
	,UserName		VARCHAR(30)
	,PasswordHash	BINARY(64)
	,PasswordSalt	BINARY(128)
)