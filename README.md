# ObjectDB
Simple object data base

C# library.

ObjectDB - library project.
ObjectDB_Example - simple example of using.

For start using it just add reference to ObjectDB.dll.

If you want to save some object to DB you need:
1) Inherit it class from DBObject.
2) Mark all fields that you want to be saved by [ObjectDB] attribute.

For creating new DB you need to initialize ObjectDBConnection with path to place, where you want to hold it.
Where will be created folder for your DB and its descriptor (db.descriptor).

For working with objects there are ObjectDBManager. You should initialize it with opened connection instance (ObjectDBConnection).

For saving some instace to DB there are method SaveToDB in manager. First parameter is instance and second parameter is its name.

After first saving to DB your instance will have it own Guid and some additional info (it requires some extra memory).

When you save some object instance to DB where will be created 2 files for this object: *class name*.dat and *class name*.descriptor.
All instances, that belongs to same object will be written to .dat file. Information of their amount, positions in .dat file and unique instance Guid will be written to .descriptor.


There are different ways to read instance from DB by manager:
1) FetchFromDB(string className, string instaceName/Guid instanceGuid) by instance name or instance Guid.
   In this case you can hold instace in instance of ObjectDB type. It has methods to reading and changing its values.
   Also you can hold instance by dynamic type and use its values like *fetched instance*.*value name*.  
   
2) TryFetchFromDB<T>(ref T target, string instaceName/Guid instanceGuid) by instance name or instace Guid.
   For using this method you need to have initialized instance of object you need to fetch. Class of instance that you you pass by ref in      arguments should also be inherited from DBObject and fields that is written to DB should also be marked with attribute, BUT it can be      class that differ from class instance of what you wrote to DB before (Duck typing). This method returns bool value. If true, your          instance was fetched ok. After it instance fields will be filled with data.
  
You can check ObjectDB_Example project for examples of use.

All db files are blocked for outside editing while connection to DB is open.

Also there are log file in DB directory and you can check it if something went wrong.
