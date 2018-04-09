# TBR
Remote Repo Browser Test Program

This is a small demo program that reads an intentionally poorly designed web API. The goal is to successfully retrieve data from the API while working within its limitations. 
Specifically, the API can only respond with up to a max of 100 records. If the query result is larger, only the first 100 records will be returned and no indication will be given that there are records that were not returned.

This program dynamically monitors query results and when the 100 record threshold is met, it breaks the query into mulitple smaller queries and resubmits them to ensure no result set is larger than 100 records.

The user has the option of storing the records in a local data store. In this version the data store is a simple, in memory collection. An interface is provided so replacing the in memeory store with something permanent, like Entity Framework, can be done when needed.

One additional feature is that only unique items are allowed in the local data store. As items are imported, the program checks to see if the item is already in the data store and discards any duplicates.
