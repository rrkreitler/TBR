# TBR
Remote Repo Browser Test Program

This is a small demo program that reads a specific web API that has some built-in limitations. The goal is to successfully retrieve data from it while working within the API's limitations. 
Specifically, the API can only respond with up to a max of 100 records. If the query result is larger, only the first 100 records will be returned and no indication will be given that there are records that were not returned.

This program dynamically monitors query results and when the 100 record threshold is met, it breaks the query into mulitple smaller queries and resubmits them to ensure no result set is larger than 100 records.

### Features

#### Configuration Options
There are two settings available in the appsettings.json file.  
`RemoteDataUri` is used to configure the URL for the remote API being tested. There is no UI option to set the URL from within the program.  
`PageSize` limits the number of records displayed in the browser as you browse through the list. The default value is 25.

#### Importing
The **Import** menu button displays the Import options. The user sets start and end date/times and then has two options:
**Preview** will return the number of items found in the date/time range but does not import anything to the local data store.
**Import** will retreive the items in the date/time range and import them into the local data store. Only a single instance of an item will be imported. Any duplicates found during import will not be imported a second time.

#### Browsing
For better performance while browsing, the list of messages in the local data store is broken into pages. The default page size is configured in the settings file (see _Configuration Options_ above). There is a **Show All** button available if you do not want the view paginated. Be aware that using the **Show All** button may result in slower browser performance.

Sorting options are available by clicking on the **ID** and **Date/Time** column headings. Each click will toggle between ascending and descending order based on that column.

#### Searching
A simple search option is available for the message list via the **Search** menu button. This button will only be visible if there are messages to browse in the local data store. During search, the ID, Date/Time, and Message fields will be searched. Any record where one or more fields contains the search text will be returned. Search results can be sorted and paginated as well.
