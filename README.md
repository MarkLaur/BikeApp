# Work-in-progress
This is still incomplete and filled with garbage code so try to focus on the good bits (if there are any).

# BikeApp
Welcome to my bikeapp project. This is my first Asp.NET Core project so I'm probably not doing everything sensibly.

Critique is welcome; I want to learn more.

This was made as a pre-assignment for Solita Dev Academy.

# Features
- Separate web app, API server and MySQL server
- Bike trip list view
	- Lists all bike trips and shows their data
- Station list view
	- Lists all stations and shows their data
- Station search
	- Searchable by ID
		- Name search not yet implemented
	- Shows station info
	- Shows trips starting or ending at the station
	- Pagination not yet implemented
- Data uploader
	- Supports csv input
	- 100MB bike trip file takes about 2 minutes to upload on my system.
		- After clicking the button the site appears frozen, but it's working. You just have to wait. See the console windows for status updates.
			- This will be fixed when I make the pages load stuff on the client side.
	- Manual data entry form not yet implemented
	
# TODO
- Figure out how to send page to client early without data and do api requests late.
- Fix bike stations page layout
- Add pagination to station search page
- Station and trip entry form
- Tests
- DB user with limited permissions

# Installation instructions
1. Download this github repository
2. Download usbwebserver
3. Install visual studio and whatever packages Asp.NET requires. (.exe files will be available later)
4. Launch usbwebserver, add a 'bikeapp' database, and add tables in 'bikestations.sql' and 'biketrips.sql' files to it. (There will be a tool to automate this later)
5. Open the ApiServer project and click the button with the green arrow that says 'https' to run the project.
6. A console window and a browser window will open, leave them open. The console window should say "Database connection good.". If it says "Database couldn't be reached. Make sure it is online." then something is broken in usbwebserver.
7. Open the WebApp project and click the button with the green arrow that says 'https' to run the project.
8. A browser window with the bike app site on it will automatically open.

# FAQ
1. Why is the api server separate from the web app server?
    - Mostly just because I thought it would be neat. If I created an android app as well then the web app server could go down without taking the api with it, leaving the android app operational.
2. Why do the pages freeze if the API server is unresponsive?
	- Because I haven't yet had the time to move the API requests to the client side of things. The pages are completely rendered server side, which takes forever if the API doesn't respond.
3. Why are the binaries so massive?
	- The .NET runtime is included in the build so that the user doesn't have to worry about having it installed. Also, I couldn't enable unused code/library trimming because it broke the MySQL libary for some reason.
