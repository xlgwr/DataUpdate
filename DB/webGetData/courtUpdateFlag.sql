/*
Navicat SQL Server Data Transfer

Source Server         : S1.6
Source Server Version : 120000
Source Host           : 192.168.1.6:1433
Source Database       : eCM
Source Schema         : dbo

Target Server Type    : SQL Server
Target Server Version : 120000
File Encoding         : 65001

Date: 2016-11-11 15:55:33
*/


-- ----------------------------
-- Table structure for courtUpdateFlag
-- ----------------------------
DROP TABLE [dbo].[courtUpdateFlag]
GO
CREATE TABLE [dbo].[courtUpdateFlag] (
[Actiondate] datetime NOT NULL 
)


GO

-- ----------------------------
-- Indexes structure for table courtUpdateFlag
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table courtUpdateFlag
-- ----------------------------
ALTER TABLE [dbo].[courtUpdateFlag] ADD PRIMARY KEY ([Actiondate])
GO
