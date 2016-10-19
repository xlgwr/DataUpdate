/*
Navicat SQL Server Data Transfer

Source Server         : S1.6
Source Server Version : 120000
Source Host           : 192.168.1.6:1433
Source Database       : EMMS
Source Schema         : dbo

Target Server Type    : SQL Server
Target Server Version : 120000
File Encoding         : 65001

Date: 2016-10-19 09:16:39
*/


-- ----------------------------
-- Table structure for m_ContactPerson
-- ----------------------------
DROP TABLE [dbo].[m_ContactPerson]
GO
CREATE TABLE [dbo].[m_ContactPerson] (
[ContactPersonID] bigint NOT NULL IDENTITY(1,1) ,
[MemberComanyID] bigint NULL ,
[Surname] nvarchar(32) NULL ,
[GivenNames] nvarchar(32) NULL ,
[Salutation] nvarchar(8) NULL ,
[FullName_Cn] nvarchar(32) NULL ,
[FullName_Tm] nvarchar(32) NULL ,
[IDNumber] nvarchar(32) NULL ,
[Position] nvarchar(16) NULL ,
[Department] nvarchar(16) NULL ,
[OfficeTel1] nvarchar(32) NULL ,
[OfficeTel2] nvarchar(32) NULL ,
[MobilePhone] nvarchar(32) NULL ,
[Fax] nvarchar(32) NULL ,
[Email] nvarchar(64) NOT NULL ,
[PicPath1] nvarchar(64) NULL ,
[PicPath2] nvarchar(64) NULL ,
[Default] int NULL 
)


GO
DBCC CHECKIDENT(N'[dbo].[m_ContactPerson]', RESEED, 10427)
GO

-- ----------------------------
-- Indexes structure for table m_ContactPerson
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table m_ContactPerson
-- ----------------------------
ALTER TABLE [dbo].[m_ContactPerson] ADD PRIMARY KEY ([ContactPersonID])
GO
