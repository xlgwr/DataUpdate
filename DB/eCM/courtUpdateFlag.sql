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

Date: 2016-10-28 09:52:55
*/


-- ----------------------------
-- Table structure for courtUpdateFlag
-- ----------------------------
DROP TABLE [dbo].[courtUpdateFlag]
GO
CREATE TABLE [dbo].[courtUpdateFlag] (
[courtid] bigint NOT NULL ,
[flag] int NOT NULL DEFAULT ((0)) 
)


GO

-- ----------------------------
-- Indexes structure for table courtUpdateFlag
-- ----------------------------
CREATE UNIQUE INDEX [courtidDesc] ON [dbo].[courtUpdateFlag]
([courtid] DESC) 
WITH (IGNORE_DUP_KEY = ON)
GO
CREATE INDEX [IX_courtUpdateFlag] ON [dbo].[courtUpdateFlag]
([courtid] DESC, [flag] ASC) 
GO

-- ----------------------------
-- Primary Key structure for table courtUpdateFlag
-- ----------------------------
ALTER TABLE [dbo].[courtUpdateFlag] ADD PRIMARY KEY ([courtid])
GO
