CREATE DATABASE CasinoGames;
GO

USE [master]
GO
CREATE LOGIN [admin] WITH PASSWORD=N'admin', DEFAULT_DATABASE=[CasinoGames], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [CasinoGames]
GO
CREATE USER [admin] FOR LOGIN [admin]
GO
USE [master]
GO
CREATE LOGIN [user] WITH PASSWORD=N'user', DEFAULT_DATABASE=[CasinoGames], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [CasinoGames]
GO
CREATE USER [user] FOR LOGIN [user]
GO

USE [CasinoGames];
GO

CREATE TABLE [dbo].[Games](
	[gameId] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](80) NOT NULL,
	[thumbnail] [varchar](2000) NOT NULL,
	[image] [varchar](2000) NOT NULL,
	[url] AS (concat('https://localhost:5001/api/game/play/',[gameId])),
	[dateCreated] [datetime] NOT NULL,
	[enabled] [bit] NOT NULL
 CONSTRAINT [PK_Games] PRIMARY KEY CLUSTERED 
(
	[gameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Statistics](
	[statisticId] [int] IDENTITY(1,1) NOT NULL,
	[gameId] [int] NOT NULL,
	[sessionId] [varchar](100) NOT NULL,
	[dateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Statistics] PRIMARY KEY CLUSTERED 
(
	[statisticId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Statistics]  WITH CHECK ADD  CONSTRAINT [FK_Statistics_Games] FOREIGN KEY([gameId])
REFERENCES [dbo].[Games] ([gameId])
GO

ALTER TABLE [dbo].[Statistics] CHECK CONSTRAINT [FK_Statistics_Games]
GO

GRANT DELETE ON [dbo].[Games] TO [admin]
GO
GRANT INSERT ON [dbo].[Games] TO [admin]
GO
GRANT SELECT ON [dbo].[Games] TO [admin]
GO
GRANT UPDATE ON [dbo].[Games] TO [admin]
GO
GRANT SELECT ON [dbo].[Games] TO [user]
GO

GRANT INSERT ON [dbo].[Statistics] TO [admin]
GO
GRANT SELECT ON [dbo].[Statistics] TO [admin]
GO
GRANT INSERT ON [dbo].[Statistics] TO [user]
GO
GRANT SELECT ON [dbo].[Statistics] TO [user]
GO

INSERT INTO [dbo].[Games] (name, thumbnail, image, dateCreated, enabled) VALUES 
('Game 1', 'https://i.picsum.photos/id/404/200/200.jpg?hmac=7TesL9jR4uM2T_rW-vLbBjqvfeR37MJKTYA4TV-giwo', 'https://i.picsum.photos/id/553/800/400.jpg?hmac=AR25n_HUgkWt5xqrOVTTx2RQpvmHhW-6V_ZupsS0H3w', GETUTCDATE(), 1),
('Game 2', 'https://i.picsum.photos/id/293/200/200.jpg?hmac=6YL5khsW332VGbJLkqIfYLzyXyT1kj358PA64TJtKuw', 'https://i.picsum.photos/id/804/800/400.jpg?hmac=qpTbSz_UM7xDR-nXqB3MbYbEJkIICqxhjyLXyImzxqc', GETUTCDATE(), 1),
('Game 3', 'https://i.picsum.photos/id/804/200/200.jpg?hmac=73qw3Bnt67aOsdWd033BvfX9Gq0gIJ6FSL3Dp3gA97E', 'https://i.picsum.photos/id/520/800/400.jpg?hmac=BSTUknrVhJPVPqDWxz15EFjQduKtli7xWfvl49CcfTQ', GETUTCDATE(), 1),
('Game 4', 'https://i.picsum.photos/id/1025/200/200.jpg?hmac=lPP7DRqIRSrMTmBMEg5NbVzguwqQQs2meA5kSrgLAhc', 'https://i.picsum.photos/id/692/800/400.jpg?hmac=N0E_rmSsJRuOFGezvOU-5Q_JModU3oataM4_vpM_n_0', GETUTCDATE(), 1),
('Game 5', 'https://i.picsum.photos/id/116/200/200.jpg?hmac=l2LJ3qOoccUXmVmIcUqVK6Xjr3cIyS-Be89ySMCyTQQ', 'https://i.picsum.photos/id/197/800/400.jpg?hmac=pYbk4ToV0UEmDjllYJo54__l8gpvqTfnQLW8KYydW6o', GETUTCDATE(), 1),
('Game 6', 'https://i.picsum.photos/id/429/200/200.jpg?hmac=9FwQwE20mRBTbcAmKXOhnDdpvTgru3vSGriKkpK0kI4', 'https://i.picsum.photos/id/563/800/400.jpg?hmac=tJBEPAgSwjDhU8BUH3FFdPxgW38aYAX_arZUAC9KbBc', GETUTCDATE(), 1),
('Game 7', 'https://i.picsum.photos/id/169/200/200.jpg?hmac=MquoCIcsCP_IxfteFmd8LfVF7NCoRre282nO9gVD0Yc', 'https://i.picsum.photos/id/994/800/400.jpg?hmac=6fPDLBP85-5uapwrHMWvjZyf19cy0L5OHsbp3zjSFFE', GETUTCDATE(), 1),
('Game 8', 'https://i.picsum.photos/id/1060/200/200.jpg?hmac=M0E6SK-_reDe8rAPtwDpww5ihTgL6yewgERGc7eX5z8', 'https://i.picsum.photos/id/244/800/400.jpg?hmac=pzJSsMIdspjk2pc92pcqUbwufidmZ4R4VT8_EGDlOAs', GETUTCDATE(), 1);

GO

CREATE TABLE [dbo].[Jackpots](
	[gameId] [int] NOT NULL,
	[jackpotId] [int] IDENTITY(1,1) NOT NULL,
	[value] [money] NOT NULL,
 CONSTRAINT [PK_Jackpots] PRIMARY KEY CLUSTERED 
(
	[gameId] ASC,
	[jackpotId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Jackpots]  WITH CHECK ADD  CONSTRAINT [FK_Jackpots_Games] FOREIGN KEY([gameId])
REFERENCES [dbo].[Games] ([gameId])
GO

ALTER TABLE [dbo].[Jackpots] CHECK CONSTRAINT [FK_Jackpots_Games]
GO

INSERT INTO [dbo].[Jackpots] (gameId, value) VALUES
(1, 100000),
(4, 150000),
(7, 80000);

GO
GRANT SELECT ON [dbo].[Jackpots] TO [user]
GO

CREATE VIEW [dbo].[GamesView]
AS
WITH stats AS (SELECT COUNT(1) AS totalPlays, gameId FROM dbo.[Statistics] GROUP BY gameId)
SELECT g.*, ISNULL(stats.totalPlays, 0) AS totalPlays FROM dbo.Games AS g LEFT OUTER JOIN stats ON g.gameId = stats.gameId
GO

CREATE NONCLUSTERED INDEX [isStatisticsGameId] ON [dbo].[Statistics]
(
	[gameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

GRANT DELETE ON [dbo].[GamesView] TO [admin]
GO
GRANT INSERT ON [dbo].[GamesView] TO [admin]
GO
GRANT SELECT ON [dbo].[GamesView] TO [admin]
GO
GRANT UPDATE ON [dbo].[GamesView] TO [admin]
GO
GRANT SELECT ON [dbo].[GamesView] TO [user]
GO