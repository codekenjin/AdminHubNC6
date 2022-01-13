# AdminHubNC6
-- Database setup
Change the database connection string to fit your environment, the default database for this app is MSSQL Server 2017 Express
Run these in the NuGet Command Line Console:
update-database -context AuthDbContext
update-database -context TodoEventDbContext