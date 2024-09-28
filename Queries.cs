
using library.Entities;
using Npgsql;

namespace library.Queries;

public static class Queries
{

    static string connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION")!;
    public static void GetAllBooks()
    {
        List<BookEntity> books = new List<BookEntity>();
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            string query = @"
              SELECT b.Id,b.Title,a.Id,a.Author 
              FROM books b 
              JOIN authors a on b.AuthorId=a.Id 
              WHERE b.userId = @UserId
              ";


            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", AuthHandler.globalUserId!);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var book = new BookEntity
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            AuthorId = reader.GetInt32(2),
                            Author = new AuthorEntity
                            {
                                Id = reader.GetInt32(2),
                                Author = reader.GetString(3)
                            }
                        };
                        books.Add(book);
                    }
                }
            }

        }
        if (books.Count <= 0)
        {
            Console.WriteLine("There are currently 0 Books in your library !");
            return;
        }
        else
        {
            Console.WriteLine($"There are currently {books.Count} Books in your library !");
            int index = 1;
            foreach (var book in books)
            {
                Console.WriteLine($"{index}. '{book.Title}' by '{book.Author!.Author}'");
                index++;
            }
        }
    }

    public static void CreateBook()
    {
        Console.Write("Book Title : ");
        string? title = Console.ReadLine();

        Console.Write("Book Author : ");
        string? author = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrEmpty(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrEmpty(author))
        {
            Console.WriteLine("Invalid Input");
            return;
        }

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            // Check if author exists 
            int authorId;

            string checkAuthorQuery = @"
            SELECT Id 
            FROM authors 
            WHERE Author = @AuthorName";

            using (var checkAuthorCmd = new NpgsqlCommand(checkAuthorQuery, conn))
            {
                checkAuthorCmd.Parameters.AddWithValue("@AuthorName", author);

                var result = checkAuthorCmd.ExecuteScalar();

                if (result == null)
                {
                    // if the author does not exist, create the author
                    string insertAuthorQuery = @"
                INSERT INTO authors (Author) 
                VALUES (@AuthorName) 
                RETURNING Id
                ";
                    using (var createAuthorCmd = new NpgsqlCommand(insertAuthorQuery, conn))
                    {
                        createAuthorCmd.Parameters.AddWithValue("@AuthorName", author);
                        authorId = (int)createAuthorCmd.ExecuteScalar()!;
                    }
                }
                else
                {
                    authorId = (int)result;
                }
            }

            string insertBookQuery = @"
            INSERT INTO books (title,authorId,userId) 
            VALUES (@Title,@AuthorId,@UserId);
            ";
            using (var insertBookCmd = new NpgsqlCommand(insertBookQuery, conn))
            {
                insertBookCmd.Parameters.AddWithValue("@Title", title);
                insertBookCmd.Parameters.AddWithValue("@AuthorId", authorId);
                insertBookCmd.Parameters.AddWithValue("@UserId", AuthHandler.globalUserId!);

                insertBookCmd.ExecuteNonQuery();
            }
            Console.WriteLine($"Book '{title}' by '{author}' has been added to your library !");
        }


    }

    public static void DeleteBook()
    {
        List<BookEntity> books = new List<BookEntity>();

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            string getallBooks = @"
              SELECT b.id,b.title,a.Id,a.Author 
              FROM books b 
              JOIN authors a ON b.AuthorId = a.Id 
              WHERE userId = @UserId";
            
            using (var getAllBooksCmd = new NpgsqlCommand(getallBooks,conn))
            {
              getAllBooksCmd.Parameters.AddWithValue("@UserId", AuthHandler.globalUserId!);

              using(var reader = getAllBooksCmd.ExecuteReader())
              {
                while(reader.Read())
                {
                  books.Add(new BookEntity{
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    AuthorId = reader.GetInt32(2),   
                    Author = new AuthorEntity {
                      Id = reader.GetInt32(2),
                      Author = reader.GetString(3)
                    }
                  });
                }
              }
            }
            int index = 1;
            foreach(var book in books)
            {
              Console.WriteLine($"{index}. '{book.Title}' by '{book.Author!.Author}'");
            }

            Console.WriteLine("\n~To delete, enter the number of the book from the list.\n");
            Console.Write("Input : ");
            if (!int.TryParse(Console.ReadLine(), out int bookIndex) || bookIndex < 1 || bookIndex > books.Count)
            {
                Console.WriteLine("Invalid input. Please enter a valid Book Id");
                return;
            }

            int bookId = books[bookIndex - 1].Id;


            string deleteBookQuery = @"
            DELETE FROM books 
            WHERE books.id = @bookId 
            AND books.userId = @UserId
             ";
            using (var deleteBookCmd = new NpgsqlCommand(deleteBookQuery, conn))
            {
                deleteBookCmd.Parameters.AddWithValue("@bookId", bookId);
                deleteBookCmd.Parameters.AddWithValue("@UserId", AuthHandler.globalUserId!);
                int rowsAffected = deleteBookCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Book with Id '{bookId}' successfully deleted !");
                    return;
                }
                else
                {
                    Console.WriteLine("An error occured. Book was not deleted.");
                    return;
                }
            }
        }
    }
}
