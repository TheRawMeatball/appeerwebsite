-- SQLite
DELETE from SessionSlots;
SELECT Id, HostId, MaxAttendees, Start, End, Subjects, Description, Grade
FROM `SessionSlots`;