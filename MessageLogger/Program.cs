using MessageLogger;
using Microsoft.EntityFrameworkCore;

string userInput = string.Empty;

Intro();
User user = NewOrExisting();
LogInMessage();
ChirpClear();

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
    if (user == null)
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
        Console.WriteLine("Username Already Taken");
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
        else if(userInput == "ADMIN")
        {
            AdminBackEnd();
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

        foreach(var message in DBUser.Messages)
        {
            Console.WriteLine($"{message.User.Username}: {message.Content}");
            Console.WriteLine();

        }

    }
}


//users ordered by number of messages created (most to least)
static void OrderMessages()
{
    using(var context = new MessageLoggerContext())
    {
        ChirpDB();
        var orderedUsers = context.Users
            .Include(u=>u.Messages)
            .OrderByDescending(u =>u.Messages.Count);
        Console.WriteLine("Users ordered by number of messages");
        foreach (var user in orderedUsers)
        {
            Console.WriteLine($"{user.Username} has written {user.Messages.Count} messages");
        }
    }
    
    
}


//most commonly used word for messages (by user and overall)

static void EachWordCount()
{
    using(var context = new MessageLoggerContext())
    {
        ChirpDB();
        var words = new List<string>();
        
        foreach(var message in context.Messages)
        {
            words.Add(message.Content);
        }

        var mostPopular = words.GroupBy(s=> s).OrderByDescending(g=>g.Count());
        mostPopular.ToList().ForEach(g => Console.WriteLine("{0}: {1}", g.Key, g.Count()));

        //Console.WriteLine($"The most common word across all accounts is: {mostpopularWord}");
    }
}

static void MostPopularWord()
{
    using (var context = new MessageLoggerContext())
    {
        ChirpDB();
        var words = new List<string>();

        foreach (var message in context.Messages)
        {
            words.Add(message.Content);
        }
       
        var mostPopular = words.GroupBy(s => s).OrderByDescending(g => g.Count()).First();
        Console.WriteLine($"The most common word across all accounts is '{mostPopular.Key}' writen {mostPopular.Count()} times");
    }
}

static void MostPopularWordUser()
{
    using(var context = new MessageLoggerContext())
    {
        ChirpDB();

        Console.Write("Please Enter in the Username of the User you are trying to find: ");
        var user = LogInAdmin();
        bool exists = DoesUserExsist(user);
        if(exists == false)
        {
            //var dbUser = context.Users.Find(user.Id);
            var SpecificUser = context.Users.Include(u => u.Messages)
                .First(u => u.Id == user.Id);

            string listOfWords = string.Empty;

            foreach(var word in SpecificUser.Messages)
            {
                listOfWords += " " + word.Content.ToLower();
            }

            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] words = listOfWords.Split(delimiterChars);

            var mostPopular = words.GroupBy(s => s).OrderByDescending(g => g.Count()).First();
            Console.WriteLine($"\nThe most common word writen by {user.Username} is '{mostPopular.Key}' writen {mostPopular.Count()} times");

        }

    }
}


static User LogInAdmin()
{
    string UserInput = Console.ReadLine();
    User user = null;
    using (var context = new MessageLoggerContext())
    {
        try
        {
            user = context.Users.First(u => u.Username == UserInput);
        }
        catch (InvalidOperationException)
        {
            user = null;
        }

        if (user != null)
        {
            return user;
        }
        else
        {
            Console.WriteLine("USERNAME NOT FOUND");
            return user;
        }
    }
}





//the hour with the most messages
static void HourWithMostMessages()
{
    using (var context = new MessageLoggerContext())
    {
        ChirpDB();

        var hours = context.Messages.GroupBy(t => t.CreatedAt.ToLocalTime().Hour);
        int mostmessage = 0;
        int hour = 0;
        foreach(var h in hours)
        {
            if(h.Count() > mostmessage)
            {
                mostmessage = h.Count();
                hour = h.Key;
            }
        }
        Console.WriteLine($"The hour with the most messages written is {hour}:00 with {mostmessage} messages");
    }
}



static string AdminBackEnd()
{
    Loading();
    while (true)
    {
        ChirpDB();
        ListOptions();
        string input = UserInput();
        if (input == "log out")
        {
            ChirpClear();
            return input;
        }

        Select(input);
    }

}


static string UserInput()
{
   string input =  Console.ReadLine();
   return input;
}

static void Select(string s)
{
    if(s == "1")
    {
        OrderMessages();
        BackButton();
    }
    else if(s == "2")
    {
        MostPopularWord();
        BackButton();
    }
    else if(s== "3")
    {
        MostPopularWordUser();
        BackButton();
    }
    else if (s == "4")
    {
        HourWithMostMessages();
        BackButton();
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
    Console.ForegroundColor = ConsoleColor.White;
    Console.BackgroundColor = ConsoleColor.Blue;
    Console.Clear();
    Console.WriteLine("   ____ _   _ ___ ____  ____    \r\n  / ___| | | |_ _|  _ \\|  _ \\   \r\n | |   | |_| || || |_) | |_) |  \r\n | |___|  _  || ||  _ <|  __/   \r\n  \\____|_| |_|___|_| \\_\\_|  ");
    Console.WriteLine();

}

static void NewUserWelcomeMessage(User user)
{
    Console.WriteLine($"Welcome to Chirp{user.Username}");
}

static void Loading()
{
    for (int i = 0; i < 2; i++)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
        Console.WriteLine("Loading Data");
        Thread.Sleep(250);
        Console.Clear();
        Console.WriteLine("Loading Data.");
        Thread.Sleep(250);
        Console.Clear();
        Console.WriteLine("Loading Data. .");
        Thread.Sleep(500);
        Console.Clear();
        Console.WriteLine("Loading Data. . .");
        Thread.Sleep(250);
        Console.Clear();
    }
}

static void ListOptions()
{
    Console.WriteLine("1: USERS ORDERED BY MESSAGE COUNT\n \n \n");
    Console.WriteLine("2: MOST POPULAR WORD\n \n \n");
    Console.WriteLine("3: USERS MOST POPULAR WORD\n \n \n");
    Console.WriteLine("4: HOUR WITH THE MOST MESSAGES");
}

static void ChirpDB()
{

    Console.Clear();
    Console.WriteLine("   ____ _   _ ___ ____  ____     ____    _  _____  _    ____    _    ____  _____ \r\n  / ___| | | |_ _|  _ \\|  _ \\   |  _ \\  / \\|_   _|/ \\  | __ )  / \\  / ___|| ____|\r\n | |   | |_| || || |_) | |_) |  | | | |/ _ \\ | | / _ \\ |  _ \\ / _ \\ \\___ \\|  _|  \r\n | |___|  _  || ||  _ <|  __/   | |_| / ___ \\| |/ ___ \\| |_) / ___ \\ ___) | |___ \r\n  \\____|_| |_|___|_| \\_\\_|      |____/_/   \\_\\_/_/   \\_\\____/_/   \\_\\____/|_____|");
    Console.WriteLine("\nWelcome admin to log out please enter 'log out'\n");
}

static void BackButton()
{
    Console.ReadLine();
}