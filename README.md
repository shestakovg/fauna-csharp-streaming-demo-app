# Fauna database streaming demo application

This is a sample C# application that demostrates how we can use fauna driver to listen to update events of a document in a database.
Editor application makes changes in database:
- creates sample collection Categories
- creates index all_Categories
- creates one sample document in collection

Watcher is listening changes of the document

## Setup a database for this project

You should create empty Fauna database. Visit to https://dashboard.fauna.com/ and hit the button "New database". ![image](https://user-images.githubusercontent.com/11041454/116414049-d29fe680-a840-11eb-9a35-b0a9d32b2ed3.png). 
Enter any database name and hit Save.

After that go to Security page and create new key for role Admin. Copy new key value.

Open file /fauna-csharp-streaming-demo-app/Common/FaunaDbInitializer.cs and put your key value into row public static string ROOT_KEY = "PUT YOUR ROOT KEY HERE"; 
Just change value PUT YOUR ROOT KEY HERE to secret key value.

## Run the application
A repo contains a C# solution (with .NET 5.0 target framework) that you can open in your favourite IDE.
You can also switch to a project directory in your terminal and run:

cd ./fauna-csharp-streaming-demo-app/Editor
dotnet run 

and 

cd ./fauna-csharp-streaming-demo-app/Watcher
dotnet run 

If you set up everything correctly you should see a "start" event in the terminal for Watcher application:
```
ObjectV(type: StringV(start),txn: LongV(1619548421450000),event: LongV(1619548421450000))
```
and "Enter new category name" for Editor application. 
Make some changes in Editor application (Enter new category name and rating) and you see update events in Watcher.

ObjectV(type: StringV(version),txn: LongV(1619618510540000),event: ObjectV(action: StringV(update),document: ObjectV(ref: RefV(id = "297124070162432513", collection = RefV(id = "Categories", collection = RefV(id = "collections"))),ts: LongV(1619618510540000),data: ObjectV(name: StringV(category 22),rating: LongV(1234)))))

