# Assumptions
* A jackpot is a price value for a game
* A game can have multiple jackpots
* Object-relational mapper frameworks means EntityFramework Core
* SQL Server means MS SQL
* Since only an api and website were requested, the idea of load balancing the jackpot provider as an external api with something like nginx seemed out of the scope.
For this reason, a round robin system was developed, and hooked up to DI to resolve the next implementation in line, everytime the service is requested.
* Since a jackpot provider was requested, a concept like CQRS seemed not viable
* Jackpot provider meant also providing the games
* Task 2 depended very much on structures built in task 1, so task 2 was branched from develop once task 1 was merged, tested and ready
* Task 4 also depended on structures built in task 3, and again was branched from develop. The idea was to prepare as much structure before starting the tasks, however it became 
apparant that it was futile as changed were always required.
