docker build -t casino-games-database .
docker run --name database -p 1433:1433 -d casino-games-database
docker wait database