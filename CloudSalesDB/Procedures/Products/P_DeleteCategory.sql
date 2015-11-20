﻿Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_DeleteCategory')
BEGIN
	DROP  Procedure  P_DeleteCategory
END

GO
/***********************************************************
过程名称： P_DeleteCategory
功能描述： 删除角色
参数说明：	 
编写日期： 2015/11/2
程序作者： Allen
调试记录： exec P_DeleteCategory 
************************************************************/
CREATE PROCEDURE [dbo].[P_DeleteCategory]
@CategoryID nvarchar(64),
@OperateID nvarchar(64),
@Result int output --0：失败，1：成功，10002 存在关联数据
AS

begin tran

set @Result=0

declare @Err int=0

--存在关联数据
if exists(select AutoID from Category where PID=@CategoryID and Status<>9)
begin
	set @Result=10002
	rollback tran
	return
end

set @Err+=@@error

Update Category set Status=9 where CategoryID=@CategoryID 

if(@Err>0)
begin
	set @Result=0
	rollback tran
end 
else
begin
	set @Result=1
	commit tran
end