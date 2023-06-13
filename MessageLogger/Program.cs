using MessageLogger;
using Microsoft.EntityFrameworkCore;

string userInput = string.Empty;

Intro();
User user = NewOrExisting();
LogInMessage();


while (userInput.ToLower() != "quit")
{
    while (userInput.ToLower() != "log out")
    {
        ChirpClear();
        DisplayAllUserMessages(user);
        userInput = AddUserMessage(user);
    }
    ChirpClear();

    user = NewOrExisting();
    if(user == null)
    {
        userInput = "quit";
    }
    else
    {
        userInput = string.Empty;
    }
   
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
    if (exsists == false)
    {
        ChirpClear();
        Console.WriteLine("User Name Already Taken");
        user = NewOrExisting();
        return user;
    }
    else
    {
        using (var context = new MessageLoggerContext())
        {
            context.Users.Add(user);
            context.SaveChanges();
        }
        return user;
    }
}

static bool DoesUserExsist(User user)
{
    using(var context = new MessageLoggerContext())
    {
        User userExists;
        try
        {
             userExists = context.Users.First(u => u.Username == user.Username);
        }
        catch (InvalidOperationException)
        {
            userExists = null;
        }
        
       if(userExists != null)
        {
            return false;
        }
        else
        {
            return true;
        }
        
        
    }
}

static string AddUserMessage(User user)
{
    while (true)
    {
        Console.Write("Add a message: ");
        //insert Into Database if !log out
        string userInput = Console.ReadLine();
        Console.WriteLine();

        if (userInput.ToLower() != "log out")
        {
            using (var context = new MessageLoggerContext())
            {
                User databaseUser = context.Users.Find(user.Id);
                Message newMessage = new Message(userInput);
                newMessage.User = databaseUser;
                context.Messages.Add(newMessage);
                context.SaveChanges();
            }
        }
        else
        {
            return userInput;
        }
    }

}


static User LogIn()
{
    Console.WriteLine("Please enter in your username");
    string UserInput = Console.ReadLine();
    User user = null;
    using(var context = new MessageLoggerContext())
    {
        try
        {
            user = context.Users.First(u => u.Username == UserInput);
        }
        catch (InvalidOperationException)
        {
            user = null;
        }
       
        if(user != null)
        {
            return user;
        }
        else
        {
            ChirpClear();
            Console.WriteLine("USERNAME NOT FOUND");
            return user;
        }
    }
   

}

static User NewOrExisting()
{
    while (true)
    {
        Console.WriteLine("Would you like to log in a `new` or `existing` account? Or, `quit`?");
        string userInput = Console.ReadLine();
        User user;
        ChirpClear();
        if (userInput.ToLower() == "new")
        {
            user = CreateUser();
            return user;
        }
        else if (userInput.ToLower() == "existing")
        {
            user = LogIn();

            if(user != null)
            {
                return user;
            }
            
        }
        else if(userInput == "quit")
        {
            return user = null;
        }
        else
        {
            Console.WriteLine("Invalid Input");
        }
    }
    
}

static void DisplayAllUserMessages(User user)
{
    using (var context = new MessageLoggerContext())
    {
         User DBUser = context.Users.Include(m => m.Messages).First(u => u.Username == user.Username);
        //foreach (var message in context.Messages.Include(m => m.User))
        //{
        //    Console.WriteLine($"{message.User.Username}: {message.Content}");
        //    Console.WriteLine();
        //}
        foreach(var message in DBUser.Messages)
        {
            Console.WriteLine($"{message.User.Username}: {message.Content}");
            Console.WriteLine();

        }

    }
}







//Front End Methods
static void Intro()
{
    ChirpClear();
    Console.WriteLine("Let's create a user pofile for you.");
}

static void LogInMessage()
{
    Console.WriteLine();
    Console.WriteLine("To log out of your user profile, enter `log out`.");
    Console.WriteLine();
    Console.Write("Add a message (or `quit` to exit): ");
}

static void ChirpClear()
{
    Console.Clear();
    Console.WriteLine(" _______ _     _ _____  ______  _____      \r\n |       |_____|   |   |_____/ |_____]     \r\n |_____  |     | __|__ |    \\_ |           \r\n                                     ");
    Console.WriteLine();

}

static void NewUserWelcomeMessage(User user)
{
    Console.WriteLine($"Welcome to Chirp{user.Username}");
}
