# BikeApp
Welcome to my bikeapp project. This is my first ASP.NET Core project so I'm probably not doing everything sensibly.

Critique is welcome; I want to learn more.

This was made as a pre-assignment for Solita Dev Academy.

# Features
- Separate web app, API server and MySQL server
- Bike trip list page
	- Lists all bike trips and shows their data
- Station list page
	- Lists all stations and shows their data
- Station search page
	- Searchable by ID or name
- Station Info page
	- Shows station info
	- Shows trips starting from or ending at the station
- Data upload page
	- Station and trip entry forms
	- Supports csv input
		- 100MB bike trip file takes about 2 minutes to upload on my system.
		- After clicking the button the site appears frozen, but it's working. You just have to wait. See the console windows for status updates. The text below the button updates when upload is complete.
			- This will be fixed when I make the pages load stuff on the client side.
	
# TODO
- Handle text from the database in a way that prevents xss attacks
- Do api requests on the front end to make the site more responsive
- Fix bike stations page layout
- Make station search form more user friendly
	- The page can only search by name OR id, yet the form allows both to be entered.
- Column sorting
- Tests
- DB user with limited permissions
- Improve bike trips database query performance

# Installation instructions
1. Download the latest build from the releases page and extract the .zip
2. Download UsbWebServer and extract the .zip. This project was tested with v8.6
4. Launch UsbWebServer and leave the window open.
5. Run DBInitializer.bat
	- A console window will open. if it says "Database initialized" then all is well. You can close this window.
6. Run ApiServer.bat
	- A console window will open. Leave it open. The console window should tell you that database connection is good. If it says anything else then something is broken in UsbWebServer.
7. Rum WebApp.bat
	- A console window will open. Leave it open.
8. Open a browser and go to http://localhost:5000
	- Chrome and edge have been tested to work.
9. You should now see my beautiful app
10. Navigate to the "Import data" page
11. Import a stations csv file
12. Import a trips csv file
	- This might take a few minutes. See the console windows for status updates.
	- You cannot import trips before stations as trips with unknown stations are not allowed.
13. The app should now be fully set up and functional.

# Build instructions
1. Make sure that the 'Produce single file' and 'Trim unused code' settings are turned off. They break the MySQL library.

# FAQ
1. Why is the api server separate from the web app server?
    - Mostly just because I thought it would be neat. If I created an android app as well then the web app server could go down without taking the api with it, leaving the android app operational.
2. Why do the pages freeze if the API server is unresponsive?
	- Because I haven't yet had the time to move the API requests to the client side of things. The pages are completely rendered server side, which takes forever if the API doesn't respond.
3. Why are the binaries so massive?
	- The .NET runtime is included in the build so that the user doesn't have to worry about having it installed. Also, I couldn't enable unused code/library trimming because it broke the MySQL libary for some reason.
