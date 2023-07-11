USE [Proyecto]
GO

/****** Object:  Table [dbo].[Usuarios]    Script Date: 5/7/2023 16:40:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Usuarios](
	[usuario_id] [bigint] IDENTITY(1,1) NOT NULL,
	[usu_correo] [varchar](50) NOT NULL,
	[usu_clave] [varchar](25) NOT NULL,
	[usu_nombre] [varchar](100) NOT NULL,
	[rol_id] [int] NOT NULL,
	[usu_identificacion] [varchar](20) NOT NULL,
	[usu_claveTemporal] [bit] NULL,
	[usu_caducidad] [datetime] NULL,
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[usuario_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY([rol_id])
REFERENCES [dbo].[Roles] ([rol_id])
GO

ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuario_Rol]
GO


