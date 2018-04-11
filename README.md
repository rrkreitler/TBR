# TBR
Remote Repo Browser Test Program

This is a small demo program that reads a specific web API that has some built-in limitations. The goal is to successfully retrieve data from it while working within the API's limitations. 
Specifically, the API can only respond with up to a max of 100 records. If the query result is larger, only the first 100 records will be returned and no indication will be given that there are records that were not returned.

This program dynamically monitors query results and when the 100 record threshold is met, it breaks the query into mulitple smaller queries and resubmits them to ensure no result set is larger than 100 records.

The user has the option of storing the records in a local data store. In this version the data store is a simple, in memory collection. An interface is provided so replacing the in memeory store with something permanent, like Entity Framework, can be done when needed.

One additional feature is that only unique items are allowed in the local data store. As items are imported, the program checks to see if the item is already in the data store and discards any duplicates.

### Configuration Notes
The URL for the API being tested is configured in appsettings.json file. There is no UI option to set the URL from within the program.

There is also an option to set the page size in the appsettings.json file. The page size limits the number of records displayed in the browser as you browse through the list. The default value is 25.
