
Author 1 : Hanyuan Zhang , Student number: s3757573
Author 2 : Zalik Fakri , Student number: s3778065


1. Acknowledgement: 
Many thanks to Shekhar Kalra and Matthew Bolger(WDT teaching team). The code examples provided by them in the course materials was a huge help in the initial structure of our website Application. In addition, the example code in learning materials also gives us a lot of inspiration.



2. Project structure

This is a Web Application for a bank. We provide customer login and administrator login at two different sites. This allows administrators to manage accounts in parallel while users are using the site. And all sites share the same database.

The structural diagram of the project is shown below ：
               --- Assignment2_WDT(solution) 
                       >>>> AdminWeb(the project for the admin site)
                       >>>> Assignment2_WDT(the project for the client site)
                       >>>> BankApi (the API for admin site reading the data from database)

All three projects were compiled using the MVC development pattern, and the main language of compilation was C#. For the database language, choose Entity FrameworkCore. 
                                
   -- In the client site, we mainly provided the following functions
         a. Users can deposit, withdraw and transfer freely (ATM Server)
         b. Users can create their own bills to pay and schedule the payment time.
         c. Users can freely view their own transfer records.
         d. Users can view their bill payment list freely.
         e. The background service will automatically pay the user's bills. 
         f. Users are notified by email when new transactions are sent.  
   
   -- In the administrator site, we mainly provide the following functions.
         a. Administrators are free to view all Customer information.
         b. The administrator can freely view Customer's account information.
         c. The administrator can lock Customer's account for 1 minute.
         d. The administrator can query Customer's transfer record.
         e. The administrator can query the corresponding transfer records according to the specific time period required.
         f. The administrator can freely view the customer's billpays payment situation, and can block the billpays, and return the necessary message to the user.                            



3. Starting points

First, “Assignment2_WDT project”. It's actually a complete site. This includes user authentication, which means that if you are logged in as an administrator, you will see a different page than the normal user. So, it's really a standalone Web, so you can just run it and start your operations.

Second, for the AdminWeb and BankAPI projects, these two projects are actually functional tests of an external API (another way to get data from a database). Because the data in AdminWeb is actually passed in through the API, you need to run BankAPI before running AdminWeb. This ensures that the Web can successfully read data from the API. In addition, you need to keep both projects running in parallel for them to work properly.


4. Install instructions

You can download our Application from two sources(Github and RMIT Canvas). For example, if you use GitHub, you can access our repro. Click "Download Zip" from the code. When the download is complete, double-click "assignment2_wdt.sln" to run it directly. Visual Studio will help you install all the components.


5. The using of records(C# new feature)

In the traditional object-oriented programming core idea, the object becomes the core of the program. This means that we have to make objects more flexible so that they can be referenced during programming. But sometimes, our hopes are just the opposite. For example, the Login class. The Login class is kind of the starting point for all the classes in the system. So we have to keep the class safe and reduce the risk. So when I wrapped the Login class, I used the Record keyword to define this type. This means that the properties and state in login are immutable, so any future operation on login must use the WITH key to change the replicated version. This provides thread safety.



6. Code reference list:

Docs.microsoft.com. 2021. Part 6, controller methods and views in ASP.NET Core. [online] Available at: <https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/controller-methods-views?view=aspnetcore-5.0> [Accessed 30 January 2021].

WDT teaching Team (2021), "SessionDemo", lecture 6 learning material, code example 05

WDT teaching Team (2021), "CustErrPageDemo", lecture 7 learning material, code example 03.

WDT teaching Team (2021), "BackgroundServiceExample" , Lecture 8 learning material, code example 04.

WDT teaching Team (2021), "WebAPI&Client" , Lecture 9 learning material, code example.

WDT teaching Team (2021), "ArchiveToCopy_2" Tutorial 9 learning material, Week9.


