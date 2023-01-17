# BikeApp
Pre-assignment for Solita Dev Academy

The plan tm:
ASP.NET core webapp that talks to the separate api service, full source code in git repo
Some kind of api service that talks to the database, full source code in git repo
Database implemented with usbwebserver: only table export in git repo, user will download usbwebserver and import table export, usbwebserver will be left at default settings.


# TODO
Figure out how to send page to client early without data and do api requests late.
Bike stations api
Single station page with the station's trips listed

#Installation instructions
1. Download this github repository
2. Download usbwebserver
3. Install visual studio and whatever packages Asp.NET requires. (.exe files will be available later)
4. Launch usbwebserver, add a 'bikeapp' database, and add tables in 'bikestations.sql' and 'biketrips.sql' files to it. (There will be a tool to automate this later)
5. Open the ApiServer project and click the button with the green arrow that says 'https' to run the project.
6. A console window and a browser window will open, leave them open. The console window should say "Database connection good.". If it says "Database couldn't be reached. Make sure it is online." then something is broken in usbwebserver.
7. Open the WebApp project and click the button with the green arrow that says 'https' to run the project.
8. A browser window with the bike app site on it will automatically open.