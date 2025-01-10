-- Step 1: Delete all entries from related tables
DELETE FROM WaitingList;
DELETE FROM Reviews;
DELETE FROM BoughtBooks;

-- Step 2: Preserve the existing 10 books
DELETE FROM Books WHERE Id NOT IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

-- Step 3: Re-insert preserved entries
INSERT INTO Books (Id, ImagePath, Name, Author, Category, BuyingPrice, BorrowPrice, Sale, AvailableCopies, MinAge, BookFilePathpdf, BookFilePathepub, BookFilePathmobi, BookFilePathpf2b)
VALUES 
(1, '\\Content\\Images\\img1.jpg', 'The Hunger Games', 'Suzanne Collins', 'YA', 19.99, 15.00, 0, 0, 20, '\\Content\\Books\\43c3a56d-50cf-47b1-9ac9-c6ef1c33f4ea.pdf', NULL, NULL, NULL),
(2, '\\Content\\Images\\img2.jpg', 'Harry Potter and the Order of the Phoenix', 'J.K. Rowling', 'Fiction', 24.99, 5.99, 0, 18, 0, '\\Content\\Books\\43c3a56d-50cf-47b1-9ac9-c6ef1c33f4ea.pdf', NULL, NULL, NULL),
(3, '\\Content\\Images\\img3.jpg', 'Pride and Prejudice', 'Jane Austen', 'Romance', 15.99, 3.99, 50, 10, 0, NULL, NULL, NULL, NULL),
(4, '\\Content\\Images\\img4.jpg', 'To Kill a Mockingbird', 'Harper Lee', 'YA', 20.99, 4.49, 0, 13, 0, NULL, NULL, NULL, NULL),
(5, '\\Content\\Images\\img5.jpg', 'The Book Thief', 'Markus Zusak', 'Fiction', 29.99, 6.99, 0, 6, 0, NULL, NULL, NULL, NULL),
(6, '\\Content\\Images\\img6.jpg', 'Twilight', 'Stephenie Meyer', 'YA', 14.99, 3.99, 0, 4, 0, NULL, NULL, NULL, NULL),
(7, '\\Content\\Images\\img7.jpg', 'Animal Farm', 'George Orwell', 'Classic', 9.99, 2.99, 0, 12, 0, NULL, NULL, NULL, NULL),
(8, '\\Content\\Images\\img8.jpg', 'J.R.R. Tolkien 4-Book Boxed Set', 'J.R.R. Tolkien', 'Fiction', 59.99, 15.99, 0, 3, 0, NULL, NULL, NULL, NULL),
(9, '\\Content\\Images\\img9.jpg', 'The Chronicles of Narnia', 'C.S. Lewis', 'Fiction', 34.99, 8.99, 0, 9, 0, NULL, NULL, NULL, NULL),
(10, '\\Content\\Images\\img10.jpg', 'The Fault in Our Stars', 'John Green', 'Romance', 19.99, 4.99, 0, 5, 0, NULL, NULL, NULL, NULL);

-- Step 4: Add 15 random books
INSERT INTO Books (Id, ImagePath, Name, Author, Category, BuyingPrice, BorrowPrice, Sale, AvailableCopies, MinAge, BookFilePathpdf, BookFilePathepub, BookFilePathmobi, BookFilePathpf2b)
VALUES 
(11, '\\Content\\Images\\img11.jpg', 'The Great Gatsby', 'F. Scott Fitzgerald', 'Classic', 10.99, 2.49, 10, 0, NULL, NULL, NULL, NULL, NULL),
(12, '\\Content\\Images\\img12.jpg', '1984', 'George Orwell', 'Dystopian', 12.99, 3.49, NULL, 1, NULL, NULL, NULL, NULL, NULL),
(13, '\\Content\\Images\\img13.jpg', 'Moby-Dick', 'Herman Melville', 'Classic', 15.99, 4.99, NULL, 2, NULL, NULL, NULL, NULL, NULL),
(14, '\\Content\\Images\\img14.jpg', 'War and Peace', 'Leo Tolstoy', 'Historical', 25.99, 6.99, NULL, 5, NULL, NULL, NULL, NULL, NULL),
(15, '\\Content\\Images\\img15.jpg', 'The Catcher in the Rye', 'J.D. Salinger', 'Fiction', 18.99, 4.99, NULL, 7, NULL, NULL, NULL, NULL, NULL),
(16, '\\Content\\Images\\img16.jpg', 'Les Misérables', 'Victor Hugo', 'Classic', 29.99, 7.99, NULL, 3, NULL, NULL, NULL, NULL, NULL),
(17, '\\Content\\Images\\img17.jpg', 'Frankenstein', 'Mary Shelley', 'Horror', 9.99, 2.99, 15, 0, NULL, NULL, NULL, NULL, NULL),
(18, '\\Content\\Images\\img18.jpg', 'Dracula', 'Bram Stoker', 'Horror', 11.99, 3.99, NULL, 4, NULL, NULL, NULL, NULL, NULL),
(19, '\\Content\\Images\\img19.jpg', 'The Odyssey', 'Homer', 'Epic', 13.99, 4.99, NULL, 8, NULL, NULL, NULL, NULL, NULL),
(20, '\\Content\\Images\\img20.jpg', 'Jane Eyre', 'Charlotte Brontë', 'Romance', 17.99, 5.99, 20, 10, NULL, NULL, NULL, NULL, NULL),
(21, '\\Content\\Images\\img21.jpg', 'The Iliad', 'Homer', 'Epic', 14.99, 4.49, NULL, 6, NULL, NULL, NULL, NULL, NULL),
(22, '\\Content\\Images\\img22.jpg', 'The Hobbit', 'J.R.R. Tolkien', 'Fantasy', 19.99, 5.49, NULL, 12, NULL, NULL, NULL, NULL, NULL),
(23, '\\Content\\Images\\img23.jpg', 'Little Women', 'Louisa May Alcott', 'Classic', 16.99, 4.99, NULL, 9, NULL, NULL, NULL, NULL, NULL),
(24, '\\Content\\Images\\img24.jpg', 'Crime and Punishment', 'Fyodor Dostoevsky', 'Classic', 20.99, 6.49, NULL, 1, NULL, NULL, NULL, NULL, NULL),
(25, '\\Content\\Images\\img25.jpg', 'Brave New World', 'Aldous Huxley', 'Dystopian', 22.99, 6.99, 25, 0, NULL, NULL, NULL, NULL, NULL);
