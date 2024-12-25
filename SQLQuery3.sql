-- Step 1: Clear all relevant tables
DELETE FROM WaitingList;
DELETE FROM BorrowedBooks;
DELETE FROM BoughtBooks;
DELETE FROM Books;
DELETE FROM Users;

-- Step 2: Insert new books into the Books table
INSERT INTO Books (Name, Author, AvailableCopies, BuyingPrice, BorrowPrice, Sale, ImagePath) VALUES
('The Hunger Games', 'Suzanne Collins', 5, 19.99, 4.99, 0, '\Content\Images\img1.jpg'),
('Harry Potter and the Order of the Phoenix', 'J.K. Rowling', 7, 24.99, 5.99, 0, '\Content\Images\img2.jpg'),
('Pride and Prejudice', 'Jane Austen', 10, 15.99, 3.99, 0, '\Content\Images\img3.jpg'),
('To Kill a Mockingbird', 'Harper Lee', 8, 20.99, 4.49, 0, '\Content\Images\img4.jpg'),
('The Book Thief', 'Markus Zusak', 6, 29.99, 6.99, 0, '\Content\Images\img5.jpg'),
('Twilight', 'Stephenie Meyer', 4, 14.99, 3.99, 0, '\Content\Images\img6.jpg'),
('Animal Farm', 'George Orwell', 12, 9.99, 2.99, 0, '\Content\Images\img7.jpg'),
('J.R.R. Tolkien 4-Book Boxed Set', 'J.R.R. Tolkien', 3, 59.99, 15.99, 0, '\Content\Images\img8.jpg'),
('The Chronicles of Narnia', 'C.S. Lewis', 9, 34.99, 8.99, 0, '\Content\Images\img9.jpg'),
('The Fault in Our Stars', 'John Green', 5, 19.99, 4.99, 0, '\Content\Images\img10.jpg');

-- Step 3: Insert new users into the Users table
INSERT INTO Users (Name, Mail, Admin, Age, Password) VALUES
('Alice Smith', 'alice.smith@example.com', 0, 30, 'password123'),
('Bob Johnson', 'bob.johnson@example.com', 0, 25, 'securePass456');
