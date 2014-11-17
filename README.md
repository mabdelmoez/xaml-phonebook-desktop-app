xaml-phonebook-desktop-app
==========================

Xaml C#.Net Phonebook App with XSD, XML used for data validation

The Application is a WPF application which runs on windows. It uses a schema “XSD file” to validate the XML file data as well as it uses its own form validation techniques to validate the form data when entered before adding them to the XML file. The Application triggers form validation errors by using a Model class for the XSD File to serialize and validate the data by setters and getters throwing “Application Exception” on non-valid data entry to be triggered by the Application UI upon any form field entry with binding the “ExceptionValidationRule” in each UI Element in the XAML of the UI (MainWindow).

Each XML Element has ID, PhoneNo, Name and Address. XML Data & Form Constraints are as follows:

-No Special Characters are allowed.
-ID: Unique int32 and auto incremental as it has been set as each element attribute to locate elements easily as well as it was set as the primary key. It has a value of -1 “AutoIncrementStep” property as well as a value of -1 “AutoIncrementSeed”, in order to not to interfere with the already made ones.
-PhoneNo: Must be a Swiss phone number containing the 9 digits without zero. Ex:(0)766683551.
-Name: Must contain letters only.
-Address: Could contain letters & numbers.

When the Application is opened, the files are going to be validated if they do exist and if permitted the program follows into the next step. For Example: XSD File must be present, XML File must be valid if it does exist and if it does not exist the Application will create a new XML File automatically and you can start your data entry.
After entering the correct & validated data for the first time or even if you have appended data to the XML File. The user can always display the data on the data grid which also makes advantage of the good features of WPF as it uses the “DataContext” property to bind the loaded XML to itself easily.

Afterwards, the user can at any time sort (alphabetically by name just like any phonebook) and export the XML Data to Text File if the XMLFile is valid. In case, if the text file exists it is going to be recreated. The data will be read from the already loaded XDocument object in memory, and then by using LINQ To XML, it will re-order this already loaded XDocument object into a new XDocument object while the data will be extracted from the new XDocument object and appended by a String Builder to the text file as each line represents an XML Entry, For example: ID, Name, Address, PhoneNo. The user also can erase the XML File and start all over at any time.

Apart from interactive UI on form validation, The UI is as interactive as possible because possible actions with the application are being shown based on the user interaction and if he/she performed any action with the application as well. For Example: The user will notice the buttons appear and disappear based on the interaction with them or others.
Last but not least, the application has very strict rules on validation. So it implements a file watcher to watch both the XML and XSD files if being changed or renamed while the application is running and it will stop running and request for revalidation if possible. Not to mention, the Application Errors providing techniques.

How to use:

1- Open the project in visual studio.
2- Make sure that the XSD & XML Files is present in the root directory of the application(/bin/debug).
3- Start Debugging / Run the project
4- Enter Data as specified / View Data / Export Data / Erase Data and Start All Over Again

Notes: 

Default File names are “PhoneBookEntries.txt”, “PhoneBookEntries.xml”, “PhoneBookEntries.xsd” which are located in the root directory of the application, and please watch closely for the schema (XSD File) as it has to be present which is very important for launching and dealing with the application.

-You can change their variables (The Default specified files above) which are located in one place (MainWindow.xaml.cs) for ease of access and maintenance and recompile if opened from the visual studio.
-The Code has inline comments for explanation and documentation purposes.


