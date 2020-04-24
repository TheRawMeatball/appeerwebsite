-- SQLite
UPDATE Users SET Admin=1;
SELECT Id, FirstName, LastName, Username, AvatarPath, Grade, Admin, Instructor, PasswordHash, PasswordSalt
FROM `Users`;