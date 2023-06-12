// See https://aka.ms/new-console-template for more information
using MessageLogger;
//Intro
Console.WriteLine("Welcome to Message Logger!");
Console.WriteLine();
Console.WriteLine("Let's create a user pofile for you.");

//DRY1CreateUserMethod
//Insert User into Database
Console.Write("What is your name? ");
string name = Console.ReadLine();
Console.Write("What is your username? (one word, no spaces!) ");
string username = Console.ReadLine();
User user = new User(name, username);

Console.WriteLine();
Console.WriteLine("To log out of your user profile, enter `log out`.");

Console.WriteLine();
Console.Write("Add a message (or `quit` to exit): ");

string userInput = Console.ReadLine();
List<User> users = new List<User>() { user };

//have to logout before quiting.
while (userInput.ToLower() != "quit")
{
    while (userInput.ToLower() != "log out")
    {
        user.Messages.Add(new Message(userInput));

        foreach (var message in user.Messages)
        {
            //Read from Database
            Console.WriteLine($"{user.Name} {message.CreatedAt:t}: {message.Content}");
        }

        Console.Write("Add a message: ");
        //insert Into Database if !log out
        userInput = Console.ReadLine();
        Console.WriteLine();
    }

    Console.Write("Would you like to log in a `new` or `existing` user? Or, `quit`? ");
    userInput = Console.ReadLine();
    if (userInput.ToLower() == "new")
    {
        //DRY1 CreateUserMethod
        //Insert User into Database
        Console.Write("What is your name? ");
        name = Console.ReadLine();
        Console.Write("What is your username? (one word, no spaces!) ");
        username = Console.ReadLine();
        user = new User(name, username);
        users.Add(user);

        //Insert Message Into Database if !log out
        Console.Write("Add a message: ");
        userInput = Console.ReadLine();

    }
    else if (userInput.ToLower() == "existing")
    {
        //loging in doesn't display users Messages
        Console.Write("What is your username? ");
        username = Console.ReadLine();
        user = null;
        //Read From Database to Login
        foreach (var existingUser in users)
        {
            if (existingUser.Username == username)
            {
                user = existingUser;
            }
        }
        
        if (user != null)
        {
            //Insert Message Into Database if !log out
            Console.Write("Add a message: ");
            userInput = Console.ReadLine();
        }
        else
        {
            //If Reached Program Quits;
            Console.WriteLine("could not find user");
            userInput = "quit";

        }
        
    }

}

//Read From Database
Console.WriteLine("Thanks for using Message Logger!");
foreach (var u in users)
{
    Console.WriteLine($"{u.Name} wrote {u.Messages.Count} messages.");
}
