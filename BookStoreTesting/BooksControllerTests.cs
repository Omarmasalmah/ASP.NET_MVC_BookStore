using BookStore.Controllers;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace BookStoreTesting
{
    [TestClass]
    public class BooksControllerTests
    {
        private BooksController _controller;
        private BookStoreContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookStoreContext>()
                .UseInMemoryDatabase(databaseName: "BookStoreTestDb")
                .Options;

            _context = new BookStoreContext(options);

            ClearAndAddBooks(
                CreateTestBook(
                    "The End of Story",
                    "5694552890",
                    "James",
                    26.3M,
                    "Description for The End of Story",
                    "Science",
                    "http://example.com/image1.jpg",
                    new DateTime(2021, 1, 1),
                    200,
                    "English"
                ),
                CreateTestBook(
                    "Speed",
                    "0987654321",
                    "Mick",
                    19.8M,
                    "Description for Speed",
                    "Fiction",
                    "http://example.com/image2.jpg",
                    new DateTime(2022, 2, 2),
                    340,
                    "Spanish"
                )
            );
            _controller = new BooksController(_context);
        }
        [TestMethod]
        public async Task GetBooks_ReturnsAllBooksAsync()
        {
            // Act: call the GetBooks method
            var getResult = await _controller.GetBooks();

            // Assert:
            var okResult = getResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");

            var returnBooks = okResult.Value as IEnumerable<Book>;
            Assert.IsNotNull(returnBooks, "Expected a list of books");
            Assert.AreEqual(2, returnBooks.Count(), "Expected two books in the result");
        }

        [TestMethod]
        public async Task GetBooks_ReturnsEmptyList()
        {
            ClearAndAddBooks();

            // Act
            var getResult = await _controller.GetBooks();

            // Assert
            var okResult = getResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");

            var returnBooks = okResult.Value as IEnumerable<Book>;
            Assert.IsNotNull(returnBooks, "Expected a list of books");
            Assert.AreEqual(0, returnBooks.Count(), "Expected no books in the result");
        }

        [TestMethod]
        public async Task GetBooks_ReturnsSingleBook()
        {
            ClearAndAddBooks(
                CreateTestBook(
                    "Single Book",
                    "1234567890",
                    "John Doe",
                    15.99M,
                    "Description for Single Book",
                    "Fiction",
                    "http://example.com/image.jpg",
                    new DateTime(2023, 8, 1),
                    150,
                    "English"
                )
            );
            var controller = new BooksController(_context);

            // Act
            var getResult = await controller.GetBooks();

            // Assert
            var okResult = getResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");

            var returnBooks = okResult.Value as IEnumerable<Book>;
            Assert.IsNotNull(returnBooks, "Expected a list of books");
            Assert.AreEqual(1, returnBooks.Count(), "Expected one book in the result");

            var book = returnBooks.First();
            Assert.AreEqual("Single Book", book.Title, "Expected matching book title");
        }

        [TestMethod]
        public async Task GetBooks_ThrowsException_ReturnsServerError()
        {
            _context.Dispose();
            // Act
            var getResult = await _controller.GetBooks();

            // Assert
            var result = getResult.Result as ObjectResult;
            Assert.IsNotNull(result, "Expected ObjectResult");
            Assert.AreEqual(500, result.StatusCode, "Expected 500, Internal Server Error");
        }

        [TestMethod]
        public async Task PostBook_ValidBook_ReturnsCreatedBook()
        {
            // Arrange
            var newBook = CreateTestBook(
                "New Book",
                "1982336525",
                "New Author",
                 63.5M,
                "Description for New Book",
                "Mystery",
                "http://example.com/newbook.jpg",
                new DateTime(2024, 8, 4),
                160,
                "English"
            );
            // Act
            var postResult = await _controller.CreateBook(newBook);

            // Assert
            var createdResult = postResult.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult, "Expected Created At ActionResult");

            var returnedBook = createdResult.Value as Book;
            Assert.IsNotNull(returnedBook, "Expected a Book object");
            Assert.AreEqual(newBook.Title, returnedBook.Title, "Expected matching book title");
            Assert.AreEqual(newBook.ISBN, returnedBook.ISBN, "Expected matching book ISBN");
        }

        [TestMethod]
        public async Task PostBook_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var invalidBook = CreateTestBook("",
                "7522334365",
                "New Author",
                65.4M,
                "Description for New Book",
                "Mystery",
                "http://example.com/newbook.jpg",
                new DateTime(2024, 1, 1),
                320,
                "English"
            );


            _controller.ModelState.AddModelError("Title", "Required");

            // Act
            var postResult = await _controller.CreateBook(invalidBook);
            // Assert
            var badRequestResult = postResult.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult");
        }

        [TestMethod]
        public async Task PostBook_DuplicateISBN_ReturnsBadRequest()
        {
            // Arrange
            var duplicateBook = CreateTestBook
            (
                "Duplicate Book",
                "5694552890", // ISBN already exists
                "Another Author",
                35.99M,
                "Another description",
                "Drama",
                "http://example.com/duplicatebook.jpg",
                new DateTime(2024, 2, 1),
                250,
                "English"
            );
            // Act
            var postResult = await _controller.CreateBook(duplicateBook);
            // Assert
            var badRequestResult = postResult.Result as BadRequestObjectResult;

            Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult");
        }

        [TestMethod]
        public async Task PutBook_ValidData_ReturnsNoContent()
        {
            // Arrange
            var existingBook = _context.Books.First(); // Assume a book already exists

            var updatedBook = CreateTestBook(

                "Updated Title",
                "9988776655",
                "Updated Author",
                39.29M,
                "Updated Description",
                "Updated Category",
                "http://example.com/updatedbook.jpg",
                new DateTime(2024, 3, 1),
                400,
                "Updated Language",
                existingBook.Id
            );

            // Act
            var putResult = await _controller.UpdateBook(existingBook.Id, updatedBook);

            // Assert
            var noContentResult = putResult as NoContentResult;
            Assert.IsNotNull(noContentResult, "Expected NoContentResult");

            // Verify the update
            var bookInDb = await _context.Books.FindAsync(existingBook.Id);
            Assert.AreEqual("Updated Title", bookInDb.Title, "Expected updated title");
            Assert.AreEqual("Updated Author", bookInDb.Author, "Expected updated author");
        }

        [TestMethod]
        public async Task PutBook_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var existingBook = _context.Books.First(); // Assume a book already exists

            var updatedBook = CreateTestBook(

                 "Updated Title",
                 "9988776655",
                 "Updated Author",
                 14.9M,
                 "Updated Description",
                 "Updated Category",
                 "http://example.com/updatedbook.jpg",
                 new DateTime(2024, 3, 1),
                 400,
                 "Updated Language",
                 existingBook.Id + 5 // Mismatched ID

            );

            // Act
            var putResult = await _controller.UpdateBook(existingBook.Id, updatedBook);

            // Assert
            var badRequestResult = putResult as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult");
            Assert.AreEqual("Book ID doesn't match", badRequestResult.Value, "Expected specific error message");
        }

        [TestMethod]
        public async Task PutBook_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var existingBook = _context.Books.First(); // Assume a book already exists

            var updatedBook = CreateTestBook(

                // Missing required fields like Title
                "",
               "9988776655",
                "Updated Author",
                 39.99M,
                "Updated Description",
                 "Updated Category",
                 "http://example.com/updatedbook.jpg",
                 new DateTime(2024, 3, 1),
                 400,
                 "Updated Language",
                 existingBook.Id

            );
            _controller.ModelState.AddModelError("Title", "Required");

            // Act
            var putResult = await _controller.UpdateBook(existingBook.Id, updatedBook);

            // Assert
            var badRequestResult = putResult as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult");
        }
        [TestMethod]
        public async Task DeleteBook_ValidId_DeletesBookFromDatabase()
        {
            // Arrange
            var existingBook = _context.Books.First(); // Assume a book already exists

            // Act
            var deleteResult = await _controller.DeleteBook(existingBook.Id);

            // Assert
            var noContentResult = deleteResult as NoContentResult;
            Assert.IsInstanceOfType(deleteResult, typeof(NoContentResult), "Expected NoContentResult");

            // Verify the book is no longer in the database
            var bookInDb = await _context.Books.FindAsync(existingBook.Id);
            Assert.IsNull(bookInDb, "Expected the book to be removed from the database");
        }

        [TestMethod]
        public async Task DeleteBook_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = 60; // Assume this ID doesn't exist

            // Act
            var deleteResult = await _controller.DeleteBook(nonExistentId);

            // Assert
            var notFoundResult = deleteResult as NotFoundResult;
            Assert.IsNotNull(notFoundResult, "Expected NotFoundResult");
        }
        [TestMethod]
        public async Task DeleteAllBooks_Sequentially_ReturnsNoContentForEach()
        {
            // Arrange
            var allBooks = _context.Books.ToList(); // Get all books

            foreach (var book in allBooks)
            {
                // Act
                var deleteResult = await _controller.DeleteBook(book.Id);

                // Assert
                var noContentResult = deleteResult as NoContentResult;
                Assert.IsNotNull(noContentResult, $"Expected NoContentResult for book with ID {book.Id}");
            }

            // Verify the database is empty
            Assert.AreEqual(0, _context.Books.Count(), "Expected no books in the database");
        }
        private Book CreateTestBook(string title, string isbn, string author, decimal price,
                            string description, string category, string coverImageUrl, DateTime publicationDate,
                            int numberOfPages, string language, int? id = null)
        {
            var book = new Book
            {
                Title = title,
                ISBN = isbn,
                Author = author,
                Price = price,
                Description = description,
                Category = category,
                CoverImageUrl = coverImageUrl,
                PublicationDate = publicationDate,
                NumberOfPages = numberOfPages,
                Language = language
            };

            if (id.HasValue)
            {
                book.Id = id.Value;
            }

            return book;
        }

        private void ClearAndAddBooks(params Book[] books)
        {
            _context.Books.RemoveRange(_context.Books);
            _context.Books.AddRange(books);
            _context.SaveChanges();
        }
    }
}