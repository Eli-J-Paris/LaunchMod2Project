using MessageLogger;
using Microsoft.EntityFrameworkCore;

string userInput = string.Empty;
HourWithMostMessages();
//MostPopularWordUser();
//MostPopularWord();
//EachWordCount();
//OrderMessages();


//Intro();
//User user = NewOrExisting();
//LogInMessage();


//while (userInput.ToLower() != "quit")
//{
//    while (userInput.ToLower() != "log out")
//    {
//        ChirpClear();
//        DisplayAllUserMessages(user);
//        userInput = AddUserMessage(user);
//    }
//    ChirpClear();

//    user = NewOrExisting();
//    if(user == null)
//    {
//        userInput = "quit";
//    }
//    else
//    {
//        userInput = string.Empty;
//    }
   
//}

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
        Console.WriteLine("Please Enter in the Username of the User you are trying to find");
        var user = LogInAdmin();
        if(user != null)
        {
            //var dbUser = context.Users.Find(user.Id);
            var SpecificUser = context.Users.Include(u => u.Messages)
                .First(u => u.Id == user.Id);

            var words = new List<string>();

            foreach(var word in SpecificUser.Messages)
            {
                words.Add(word.Content);
            }
            var mostPopular = words.GroupBy(s => s).OrderByDescending(g => g.Count()).First();
            Console.WriteLine($"The most common word writen by {user.Username} is '{mostPopular.Key}' writen {mostPopular.Count()} times");

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
        //this is probably just ordering by first occurence
        var time = context.Messages.OrderBy(t => t.CreatedAt).ToList();
        Console.WriteLine($"The hour with the most messages written is {time[0].CreatedAt.ToLocalTime():h tt}");
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
