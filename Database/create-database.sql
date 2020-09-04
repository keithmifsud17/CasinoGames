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
	[url] [varchar](2000) NOT NULL,
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

INSERT INTO [dbo].[Games] (name, thumbnail, image, url)
VALUES ('Game 1', 'https://picsum.photos/200', 'https://picsum.photos/800/400', 'https://localhost:5001/api/game/play/1'),
('Game 2', 'https://picsum.photos/200', 'https://picsum.photos/800/400', 'https://localhost:5001/api/game/play/2'),
('Game 3', 'https://picsum.photos/200', 'https://picsum.photos/800/400', 'https://localhost:5001/api/game/play/3'),
('Game 4', 'https://picsum.photos/200', 'https://picsum.photos/800/400', 'https://localhost:5001/api/game/play/4'),
('Game 5', 'https://picsum.photos/200', 'https://picsum.photos/800/400', 'https://localhost:5001/api/game/play/5'),
('Game 6', 'https://picsum.photos/200', 'https://picsum.photos/800/400', 'https://localhost:5001/api/game/play/6'),
('Game 7', 'https://picsum.photos/200', 'https://picsum.photos/800/400', 'https://localhost:5001/api/game/play/7'),
('Game 8', 'https://picsum.photos/200', 'https://picsum.photos/800/400', 'https://localhost:5001/api/game/play/8')