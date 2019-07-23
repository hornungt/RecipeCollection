# RecipeCollection
A web application for storing and managing recipes found online. Recipes are stored as pdfs and generated based on the URL given. This project uses ASP.NET and Angular.

Can create, edit, and delete recipes.

Before running this project:
Install node.js. Clone the project, and run npm install inside ClientApp. Install mysql server and create a db called recipe_db. Then run the files under sql scripts to create the necessary tables. Enter the username and password for your mysql server in the default connection string in appsettings.json. When running the project, make sure that your mysql server is running.

Possible future goals:
Allow user to upload their own pdf recipes files.
Accept image files in addition to pdf files.

Note if you use/base your project off of this application:
In appsettings.json, configure DefaultConnectionString to "server=DB_ADDRESS;port=DB_PORT;database=DB_NAME;user=YOUR_USER;password=YOUR_PASSWORD"
