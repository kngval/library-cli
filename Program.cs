
using library.Entities;
using library.Queries;

bool active = true;

while (active)
{

    Console.WriteLine("\nLibrary System - CLI");
    Console.WriteLine("1. List All Book");
    Console.WriteLine("2. Add a Book");
    Console.WriteLine("3. Delete a Book");
    Console.WriteLine("4. Exit");

    Console.Write("\nChoose An Option : ");
    string? input = Console.ReadLine()?.ToString();

    // if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
    // {
    //     Console.WriteLine("Invalid input");
    // }

    switch (input)
    {
        case "1":
            Queries.GetAllBooks();
            Console.Write("\nPress 'Enter' to go back to menu.");
            Console.ReadLine();
            break;

        case "2":
            Queries.CreateBook();
            break;

        case "3":
            Queries.DeleteBook();
            Console.Write("\nPress 'Enter' to go back to menu.");
            Console.ReadLine();
            break;
        case "4":
            Console.WriteLine("Exiting program...");
            active = false;
            break;
        
        default : 
            Console.WriteLine("~ Invalid Input");
            break;
    }
}
