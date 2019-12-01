# InsightlyContactScript
Script for collecting recently added contacts to a csv from the Insightly API


This .NET CORE console application pulls a list of contacts from the Insighty API: 

https://api.insightly.com/v3.1/Help#!/Contacts/GetEntities

The current flow is: Access API utilizing hard coded credentials > Gather all contacts into objects within a collection > delimit the 'recently added' contacts (added within the last 24 hours) > write all recently added contacts to a local CSV.

>>NAMESPACES/**CLASSES/~~METHODS:

>>ContactsScript
  **[Static]Program: executes the script
      ~~ Process Request: Takes an HTTPWebResponse and returns a stringified version
      
  **Basic Request: Manages the request against hardcoded authorization values
      ~~ MakeRequest: Performs GET request against the API, returning Response Value
      
  **Contact Collection: Generic List for storing InsightlyContact objects
      ~~ AddContacts: Add InsightlyContact object to collection
      ~~ DateDelimited: Creates a generic list delimited by date (currently the last 24hrs)
      ~~ GenerateContactsFromResponseString: Generates contacts from a stringified HTTPWebResponse
      ~~ ContactCollection: Contstructor
      ~~ NewContacts: returns NewlyAdded
      Properties: 
        Contacts (get all new contacts)
        NewlyAdded (get all newly added contacts
  
  **CSVCreator: Generates a local CSV from ContactCollection (currently defaults to taking new contacts
      ~~ WriteCSV: Takes a ContactCollection and writes the 'NewContacts' list to a local CSV (contacts.CSV). This is an overwrite, not an append.
      
   **[Static]Logging: Logs excptions and bad responses:
      ~~LogWrite: takes a string list and writes to local txt files ("contactsScriptLog.txt")

>> QuickType: This is the namespace for the contact objects. The structure was generated using 
     
      
      

