/*
Navicat SQL Server Data Transfer

Source Server         : S1.6
Source Server Version : 120000
Source Host           : 192.168.1.6:1433
Source Database       : EMMS_UpdateTest
Source Schema         : dbo

Target Server Type    : SQL Server
Target Server Version : 120000
File Encoding         : 65001

Date: 2016-10-28 09:53:23
*/


-- ----------------------------
-- Table structure for m_Case_items
-- ----------------------------
DROP TABLE [dbo].[m_Case_items]
GO
CREATE TABLE [dbo].[m_Case_items] (
[Tid] bigint NOT NULL IDENTITY(1,1) ,
[Language] bigint NOT NULL ,
[tkeyNo] nvarchar(128) NOT NULL ,
[tIndex] bigint NOT NULL ,
[htmlID] bigint NOT NULL ,
[SerialNo] int NOT NULL ,
[CourtID] nvarchar(MAX) NULL ,
[Judge] nvarchar(MAX) NULL ,
[Year] nvarchar(MAX) NULL ,
[CourtDay] date NULL ,
[Hearing] nvarchar(MAX) NULL ,
[CaseNo] nvarchar(MAX) NULL ,
[CaseTypeID] nvarchar(MAX) NULL ,
[Parties] nvarchar(MAX) NULL ,
[PlainTiff] nvarchar(MAX) NULL ,
[P_Address] nvarchar(MAX) NULL ,
[Defendant] nvarchar(MAX) NULL ,
[D_Address] nvarchar(MAX) NULL ,
[Nature] nvarchar(MAX) NULL ,
[Representation] nvarchar(MAX) NULL ,
[Currency] nvarchar(MAX) NULL ,
[Amount] nvarchar(MAX) NULL ,
[tname] nvarchar(MAX) NULL ,
[ttype] nvarchar(300) NULL ,
[Other] nvarchar(MAX) NULL ,
[Other1] nvarchar(MAX) NULL ,
[Remark] nvarchar(MAX) NULL ,
[tStatus] int NOT NULL ,
[ClientIP] nvarchar(MAX) NULL ,
[adduser] nvarchar(128) NULL ,
[upduser] nvarchar(128) NULL ,
[addtime] datetime NOT NULL DEFAULT (getdate()) ,
[updtime] datetime NOT NULL ,
[Representation_P] nvarchar(MAX) NULL ,
[Representation_D] nvarchar(MAX) NULL 
)


GO
DBCC CHECKIDENT(N'[dbo].[m_Case_items]', RESEED, 2118842)
GO

-- ----------------------------
-- Indexes structure for table m_Case_items
-- ----------------------------
CREATE INDEX [IX_htmlID] ON [dbo].[m_Case_items]
([htmlID] ASC) 
GO
CREATE INDEX [IX_addtime] ON [dbo].[m_Case_items]
([addtime] ASC) 
GO
CREATE INDEX [IX_m_Case_itemsTkeyNo] ON [dbo].[m_Case_items]
([tkeyNo] ASC) 
GO

-- ----------------------------
-- Primary Key structure for table m_Case_items
-- ----------------------------
ALTER TABLE [dbo].[m_Case_items] ADD PRIMARY KEY ([Tid], [Language], [tkeyNo], [tIndex])
GO
