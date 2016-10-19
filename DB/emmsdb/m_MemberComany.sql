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

Date: 2016-10-19 09:17:08
*/


-- ----------------------------
-- Table structure for m_MemberComany
-- ----------------------------
DROP TABLE [dbo].[m_MemberComany]
GO
CREATE TABLE [dbo].[m_MemberComany] (
[MemberComanyID] bigint NOT NULL IDENTITY(1,1) ,
[FullName_En] nvarchar(128) NULL ,
[FullName_Cn] nvarchar(128) NULL ,
[FullName_Tm] nvarchar(128) NULL ,
[BusinessType] nvarchar(64) NULL ,
[CIBRNO] nvarchar(64) NULL ,
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
[Address] nvarchar(128) NULL ,
[PostalCode] nvarchar(16) NULL ,
[PicPath1] nvarchar(64) NULL ,
[PicPath2] nvarchar(64) NULL ,
[FinancialLicenseNo] nvarchar(32) NULL ,
[SecuritiesLicenceNo] nvarchar(32) NULL ,
[FValidityDate] datetime NULL ,
[SValidityDate] datetime NULL ,
[PicPath3] nvarchar(64) NULL ,
[PicPath4] nvarchar(64) NULL 
)


GO
DBCC CHECKIDENT(N'[dbo].[m_MemberComany]', RESEED, 10355)
GO

-- ----------------------------
-- Indexes structure for table m_MemberComany
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table m_MemberComany
-- ----------------------------
ALTER TABLE [dbo].[m_MemberComany] ADD PRIMARY KEY ([MemberComanyID])
GO
