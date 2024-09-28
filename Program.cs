
using library.Entities;
using library.Queries;

bool active = true;
bool isLoggedIn = false;
while (active)
{
    Console.WriteLine("\nLibrary System - AUTHENTICATION !");
    Console.WriteLine("1. Register");
    Console.WriteLine("2. Login");
    Console.WriteLine("3. Exit");

    Console.Write("\nInput : ");
    string? authInput = Console.ReadLine();


    switch (authInput)
    {
        case "1":
            Console.Write("Enter a username : ");
            string? authUsername = Console.ReadLine();
            Console.Write("Enter your password : ");
            string? authPassword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(authUsername))
            {
                Console.WriteLine("~Invalid Username");
                return;
            }
            if (string.IsNullOrWhiteSpace(authPassword))
            {
                Console.WriteLine("Invalid Password");
                return;
            }

            AuthHandler.Register(authUsername, authPassword);
            break;

        case "2":
            Console.Write("Enter your username : ");
            string? loginUsername = Console.ReadLine();
            Console.Write("Enter your password : ");
            string? loginPassword = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(loginUsername))
            {
                Console.WriteLine("Invalid Username");
                return;
            }
            if (string.IsNullOrWhiteSpace(loginPassword))
            {
                Console.WriteLine("~Invalid Password");
                return;
            }
            isLoggedIn = AuthHandler.Login(loginUsername,loginPassword);
            break;

        case "3":
            Console.WriteLine("\n~Exiting Program... \n");
            active = false;
            break;

        default : 
            Console.WriteLine("~Invalid Input.");
            break;
    }
}

while (isLoggedIn)
{

    Console.WriteLine("\nLibrary System - DASHBOARD !");
    Console.WriteLine("1. List All Book");
    Console.WriteLine("2. Add a Book");
    Console.WriteLine("3. Delete a Book");
    Console.WriteLine("4. Exit");

    Console.Write("\nChoose An Option : ");
    string? input = Console.ReadLine();

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
            Console.WriteLine("~Exiting program...");
            isLoggedIn = false;
            break;

        default:
            Console.WriteLine("~Invalid Input");
            break;
    }
}

