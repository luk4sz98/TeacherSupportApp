INSERT INTO Teachers (FirstName, LastName, Age)
VALUES ('Jan', 'Kowalski', 40),
       ('Marta', 'Nowak', 35);

INSERT INTO Groups DEFAULT VALUES;
INSERT INTO Groups DEFAULT VALUES;

-- pierwsza grupa
INSERT INTO Students (FirstName, LastName, Age, GroupId)
VALUES ('Adam', 'Nowak', 20, 1),
       ('Kasia', 'Kowalska', 21, 1),
       ('Piotr', 'Wójcik', 22, 1),
       ('Anna', 'Kowal', 21, 1),
       ('Michał', 'Wiśniewski', 23, 1);

-- druga grupa
INSERT INTO Students (FirstName, LastName, Age, GroupId)
VALUES ('Karolina', 'Mazur', 19, 2),
       ('Dawid', 'Olszewski', 20, 2),
       ('Monika', 'Adamczyk', 22, 2);

-- przedmioty
INSERT INTO Subjects (Name)
VALUES ('Matematyka'),
       ('Informatyka'),
       ('Język angielski');

-- powiązanie przedmiotów z grupami
INSERT INTO SubjectGroups (GroupId, SubjectId)
VALUES (1, 1),
       (2, 2),
       (2, 3);

-- powiązanie przedmiotów z nauczycielami
INSERT INTO SubjectTeachers (SubjectId, TeacherId)
VALUES (1, 1),
       (2, 1),
       (3, 2);

INSERT INTO TeacherGroups (TeacherId, GroupId)
VALUES (1, 1),
       (2, 2);