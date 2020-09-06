# CasinoGames
Lopoca Backend Developer Test for Keith Mifsud (keitmifs@gmail.com)

# Running the solution
Assuming docker-compose is setup locally `docker-compose up` in root folder will start three docker images.
* *Database* which holds an MS sql express instance and sets up the database
* *Api* which starts up an ASP.Net Core 3.1 Api
* *Website* which starts up an ASP.Net Core 3.1 Website

Database is setup to run its scripts 90 seconds after spawn, at which point, console will show `DATABASE READY`

Please navigate to http://localhost:5001 to access the website, and http://localhost:5001/management to access the admin panel

No users and roles have been set up and an authenticated user is assumed to be admin. An authenticated user does not require a username and password. Heading to http://localhost:5001/management will present a login button which simply authenticated the current session.

# Running the tests
In root folder run `dotnet test` which will scan all test projects and run them all

# Notes
* RoundRobin.DependencyFactory will log everytime an implementation is resolved to demonstate the jackpot provider load balancing is in effect.
* If for any reason, a debugging session is needed, the database docker image must be kept running, while api and website should be stopped to prevent port clashing

# Cleanup
* Exit the console running docker-compose to shut down the containers
* In root folder run `docker-compose down -v --rmi all --remove-orphans` to remove the three docker images and their cache.
