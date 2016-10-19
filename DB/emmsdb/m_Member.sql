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

Date: 2016-10-19 09:16:55
*/


-- ----------------------------
-- Table structure for m_Member
-- ----------------------------
DROP TABLE [dbo].[m_Member]
GO
CREATE TABLE [dbo].[m_Member] (
[MemberID] bigint NOT NULL IDENTITY(1,1) ,
[MemberName] nvarchar(32) NOT NULL ,
[MemberGradeID] int NULL ,
[Password] nvarchar(64) NULL ,
[RemainingSum] float(53) NULL ,
[InvoiceDate] nvarchar(8) NULL ,
[Type] int NULL ,
[MemberComanyID] bigint NULL ,
[Surname] nvarchar(32) NULL ,
[GivenNames] nvarchar(32) NULL ,
[Salutation] nvarchar(8) NULL ,
[FullName_Cn] nvarchar(32) NULL ,
[FullName_Tm] nvarchar(32) NULL DEFAULT ((102016100001.)) ,
[IDNumber] nvarchar(32) NULL ,
[BuildName] nvarchar(64) NULL ,
[Street] nvarchar(64) NULL ,
[StreetNumber] nvarchar(8) NULL ,
[SeatNO] nvarchar(8) NULL ,
[Floor] nvarchar(8) NULL ,
[RoomNO] nvarchar(8) NULL ,
[HouseNO] nvarchar(8) NULL ,
[Area] nvarchar(16) NULL ,
[City] nvarchar(16) NULL ,
[Province] nvarchar(16) NULL ,
[CountryID] nvarchar(32) NULL ,
[Address] nvarchar(256) NULL ,
[PostalCode] nvarchar(16) NULL ,
[HomeTel] nvarchar(32) NULL ,
[OfficeTel] nvarchar(32) NULL ,
[Fax] nvarchar(32) NULL ,
[MobilePhone] nvarchar(32) NULL ,
[Email] nvarchar(64) NULL ,
[Purpose1] int NULL ,
[Purpose2] int NULL ,
[Purpose3] int NULL ,
[Purpose4] int NULL ,
[Purpose5] int NULL ,
[Purpose6] int NULL ,
[Purpose7] int NULL ,
[Pathway1] int NULL ,
[Pathway2] int NULL ,
[Pathway3] int NULL ,
[Pathway4] int NULL ,
[Pathway5] int NULL ,
[Pathway6] int NULL ,
[PicPath1] nvarchar(64) NULL ,
[PicPath2] nvarchar(64) NULL ,
[PicPath3] nvarchar(64) NULL ,
[PaymentWay] int NULL ,
[AcceptAgreement] int NULL ,
[EmailVerification] int NULL ,
[LastSeachTime] datetime NULL ,
[Enable] int NULL ,
[adduser] nvarchar(32) NULL ,
[addtime] datetime NULL ,
[upduser] nvarchar(32) NULL ,
[updtime] datetime NULL ,
[SecretKey] varchar(128) NULL ,
[Remark] nvarchar(256) NULL 
)


GO
DBCC CHECKIDENT(N'[dbo].[m_Member]', RESEED, 10555)
GO

-- ----------------------------
-- Indexes structure for table m_Member
-- ----------------------------
CREATE INDEX [m_Member_FullName_Cn] ON [dbo].[m_Member]
([FullName_Cn] ASC) 
GO
CREATE INDEX [m_Member_FullName_Tm] ON [dbo].[m_Member]
([FullName_Tm] ASC) 
GO
CREATE INDEX [m_Member_Surname] ON [dbo].[m_Member]
([Surname] ASC) 
GO
CREATE INDEX [m_Member_GivenNames] ON [dbo].[m_Member]
([GivenNames] ASC) 
GO
CREATE INDEX [m_Member_BuildName] ON [dbo].[m_Member]
([BuildName] ASC) 
GO
CREATE INDEX [m_Member_Street] ON [dbo].[m_Member]
([Street] ASC) 
GO

-- ----------------------------
-- Primary Key structure for table m_Member
-- ----------------------------
ALTER TABLE [dbo].[m_Member] ADD PRIMARY KEY ([MemberName])
GO

-- ----------------------------
-- Uniques structure for table m_Member
-- ----------------------------
ALTER TABLE [dbo].[m_Member] ADD UNIQUE ([MemberID] ASC)
GO
