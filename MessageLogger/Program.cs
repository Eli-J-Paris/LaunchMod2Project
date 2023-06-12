// See https://aka.ms/new-console-template for more information
using MessageLogger;
Intro();
//creating new user need to also find exsisting
User user = CreateUser();

LogInMessage();
while (true) 
{ AddUserMessage(user); }


//log in message
Console.WriteLine();
Console.WriteLine("To log out of your user profile, enter `log out`.");
Console.WriteLine();
Console.Write("Add a message (or `quit` to exit): ");

string userInput = Console.ReadLine();
//will I need this if reading from database?
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
        user = CreateUser();

        //Console.Write("What is your name? ");
        //name = Console.ReadLine();
        //Console.Write("What is your username? (one word, no spaces!) ");
        //username = Console.ReadLine();
        //user = new User(name, username);

        users.Add(user);

        //Insert Message Into Database if !log out
        Console.Write("Add a message: ");
        userInput = Console.ReadLine();

    }
    else if (userInput.ToLower() == "existing")
    {
        //loging in doesn't display users Messages
        Console.Write("What is your username? ");
        string username = Console.ReadLine();
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



//Back End Methods
static User CreateUser()
{
    Console.Write("What is your name? ");
    string name = Console.ReadLine();
    Console.Write("What is your username? (one word, no spaces!) ");
    string username = Console.ReadLine();
    User user = new User(name, username);
    bool exsists = DoesUserExsist(user);
    if(exsists == false)
    {
        Console.WriteLine("User Already Exsists");
        //log in method
    }
    else
    {
        using (var context = new MessageLoggerContext())
        {
            context.Users.Add(user);
            context.SaveChanges();
        }
    }
    return user;

}

static bool DoesUserExsist(User user)
{
    using(var context = new MessageLoggerContext())
    {
       if(context.Users.Contains(user) || !context.Users.Any())
        {
            return false;
        }
        else
        {
            return true;
        }
        
        
    }
}

static void AddUserMessage(User user)
{
    Console.Write("Add a message: ");
    //insert Into Database if !log out
    string userInput = Console.ReadLine();
    Console.WriteLine();
    
    using (var context = new MessageLoggerContext())
    {
        User databaseUser = context.Users.Find(user.Id);
        Message newMessage = new Message(userInput);
        newMessage.User = databaseUser;
        context.Messages.Add(newMessage);
        context.SaveChanges();
    }
}












//Front End Methods
static void Intro()
{
    Console.WriteLine("Welcome to Message Logger!");
    Console.WriteLine();
    Console.WriteLine("Let's create a user pofile for you.");
}

static void LogInMessage()
{
    Console.WriteLine();
    Console.WriteLine("To log out of your user profile, enter `log out`.");
    Console.WriteLine();
    Console.Write("Add a message (or `quit` to exit): ");
}

