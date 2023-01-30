# Work-in-progress
This is still incomplete and filled with garbage code so try to focus on the good bits (if there are any).

# BikeApp
Welcome to my bikeapp project. This is my first Asp.NET Core project so I'm probably not doing everything sensibly.

Critique is welcome; I want to learn more.

This was made as a pre-assignment for Solita Dev Academy.

# Features
- Separate web app, API server and MySQL server
- Bike trip list view
	- Pagination support not yet complete
- Station list view
	- Pagination support not yet complete
- Station search
	- Pagination support not yet complete
	- Searchable by ID
		- Name search not yet complete
	- Shows station info
	- Shows trips starting or ending at the station
- Data uploader
	- Supports csv input
	- Manual data entry form not yet complete

# The plan tm:
- ASP.NET core webapp server that talks to the separate api server, full source code in git repo
- Separate api server that talks to the database, full source code in git repo
- Database implemented with usbwebserver: only table export and setup script in git repo, user will download usbwebserver and run the not-yet-implemented setup script

# TODO
- Figure out how to send page to client early without data and do api requests late.

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
    - Mostly just because I thought it would be neat. If I created an android app aswell then the web app server could go down without taking the api with it, leaving the android app operational. Also, I've read that in larger operations there are load balancing reasons to separate different aspects of the service.
2. Why do the pages freeze if the API server is unresponsive?
	- Because I haven't yet had the time to move the API requests to the client side of things. The pages are completely rendered server side, which takes forever if the API doesn't respond.