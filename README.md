# Invoice_System
 DecoFurnSA Invoice Manager
# Overview 
The Invoice System is a C# console application that imports invoice data from a CSV file into a Microsoft SQL Server database. The application utilizes Entity Framework for data access and LINQ for data manipulation. It ensures that invoices and their corresponding lines are not duplicated in the database and provides logging and error handling for a robust user experience.

Features CSV Data Import: Reads invoice data from a CSV file and imports it into the database. Database Structure: Utilizes two main tables: InvoiceHeader and InvoiceLines. Data Validation: Checks for duplicate invoices and invoice lines before importing. Logging: Logs errors and informational messages to a file for troubleshooting. Total Validation: After importing, it checks that the total quantity and price of invoice lines match the total amounts.

# Instructions for Running the Application
To successfully run the application, you need to ensure that the file path for data.csv is correctly set to the location where your CSV file is stored. By default, the application is configured to look for the file at the following path:
string csvFilePath = @"C:\Users\Desktop\data.csv";

#Steps to Change the File Path Locate Your CSV File: First, find the location of your data.csv file on your computer. Make sure you know the full path to the file.

Open the Source Code: Open the source code of the application in your preferred code editor or IDE.

Modify the File Path:

Look for the line of code that defines the csvFilePath. It should look like the example above. Replace the existing path with the path to your data.csv file. For example:

string csvFilePath = @"C:\Path\To\Your\File\data.csv";

Save Your Changes: After updating the file path, save the changes to the source code. Run the Application: Now you can run the application, and it should be able to locate and read the data.csv file from the specified path.

Usage Setup Database: Execute the SQL scripts to create the database and tables. Configure Connection String: Update the appsettings.json file with the correct SQL Server connection string. Run the Application: Execute the console application, providing the path to the CSV file containing the invoice data.

Error Handling The application includes exception handling to manage file not found errors and other exceptions during the import process. Errors are logged to a specified log file for review.
