using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using LibraryManagementSystem.Models;
using System.Data;

namespace LibraryManagementSystem.Services
{
    public class BookService
    {
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=library_db;Uid=root;Pwd=Abc10089;";

        public List<Book> GetBooks()
        {
            var books = new List<Book>();
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "SELECT * FROM books";
                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    books.Add(new Book
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Author = reader.GetString("author"),
                        IsAvailable = reader.GetBoolean("is_available")
                    });
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to load books from database.", ex);
            }
            return books;
        }

        public void AddBook(Book book)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "INSERT INTO books (id, title, author, is_available) VALUES (@id, @title, @auth, @avail)";
                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", book.Id);
                command.Parameters.AddWithValue("@title", book.Title);
                command.Parameters.AddWithValue("@auth", book.Author);
                command.Parameters.AddWithValue("@avail", book.IsAvailable);

                command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to add book to database.", ex);
            }
        }

        public void DeleteBook(int id)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "DELETE FROM books WHERE id = @id";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to delete book from database.", ex);
            }
        }

        public void SetAvailability(int bookId, bool available)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "UPDATE books SET is_available = @avail WHERE id = @id";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@avail", available);
                command.Parameters.AddWithValue("@id", bookId);

                int affectedRows = command.ExecuteNonQuery();

                if (affectedRows == 0)
                {
                    throw new Exception($"No book found with ID {bookId}. Cannot update availability.");
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to update book availability.", ex);
            }
        }
    }

    public class MemberService
    {
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=library_db;Uid=root;Pwd=Abc10089;";

        public List<Member> GetMembers()
        {
            var members = new List<Member>();
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "SELECT * FROM members";
                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    members.Add(new Member
                    {
                        Id = reader.GetInt32("id"),
                        FullName = reader.GetString("full_name"),
                        Email = reader.GetString("email"),
                        JoinDate = reader.GetDateTime("join_date")
                    });
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to load members from database.", ex);
            }
            return members;
        }

        public void AddMember(Member member)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "INSERT INTO members (id, full_name, email, join_date) VALUES (@id, @name, @mail, @join)";
                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", member.Id);
                command.Parameters.AddWithValue("@name", member.FullName);
                command.Parameters.AddWithValue("@mail", member.Email);
                command.Parameters.AddWithValue("@join", member.JoinDate);

                command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to add member to database.", ex);
            }
        }

        public void DeleteMember(int id)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "DELETE FROM members WHERE id = @id";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to delete member from database.", ex);
            }
        }
    }

    public class LoanService
    {
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=library_db;Uid=root;Pwd=Abc10089;";

        public List<Loan> GetLoans()
        {
            var loans = new List<Loan>();
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "SELECT * FROM loans";
                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    loans.Add(new Loan
                    {
                        Id = reader.GetInt32("id"),
                        MemberId = reader.GetInt32("member_id"),
                        BookId = reader.GetInt32("book_id"),
                        BorrowDate = reader.GetDateTime("borrow_date"),
                        ReturnDate = reader.IsDBNull("return_date") ? null : reader.GetDateTime("return_date")
                    });
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to load loans from database.", ex);
            }
            return loans;
        }

        public void AddLoan(Loan loan)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string checkQuery = "SELECT is_available FROM books WHERE id = @id";
                using var checkCommand = new MySqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@id", loan.BookId);

                var isAvailable = checkCommand.ExecuteScalar();

                if (isAvailable == null)
                    throw new Exception("Book not found.");

                if (!(bool)isAvailable)
                    throw new Exception("Book is currently not available for loan.");

                string query = "INSERT INTO loans (member_id, book_id, borrow_date, return_date) VALUES (@mid, @bid, @borrow, @return)";
                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@mid", loan.MemberId);
                command.Parameters.AddWithValue("@bid", loan.BookId);
                command.Parameters.AddWithValue("@borrow", loan.BorrowDate);
                command.Parameters.AddWithValue("@return", (object?)loan.ReturnDate ?? DBNull.Value);

                command.ExecuteNonQuery();
                new BookService().SetAvailability(loan.BookId, false);
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to add loan to database.", ex);
            }
        }

        public void UpdateLoan(Loan loan)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "UPDATE loans SET member_id = @mid, book_id = @bid, borrow_date = @borrow, return_date = @return WHERE id = @id";
                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@mid", loan.MemberId);
                command.Parameters.AddWithValue("@bid", loan.BookId);
                command.Parameters.AddWithValue("@borrow", loan.BorrowDate);
                command.Parameters.AddWithValue("@return", (object?)loan.ReturnDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@id", loan.Id);

                command.ExecuteNonQuery();

                new BookService().SetAvailability(loan.BookId, true);
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to update loan in database.", ex);
            }
        }

        public void DeleteLoan(int id)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "DELETE FROM loans WHERE id = @id";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to delete loan from database.", ex);
            }
        }
    }
}
