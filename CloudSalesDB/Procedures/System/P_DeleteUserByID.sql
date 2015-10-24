Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_DeleteUserByID')
BEGIN
	DROP  Procedure  P_DeleteUserByID
END

GO
/***********************************************************
过程名称： P_DeleteUserByID
功能描述： 删除员工
参数说明：	 
编写日期： 2015/10/24
程序作者： Allen
调试记录： exec P_DeleteUserByID 
************************************************************/
CREATE PROCEDURE [dbo].[P_DeleteUserByID]
@UserID nvarchar(64),
@AgentID nvarchar(64),
@Result int output --0：失败，1：成功
AS

begin tran

set @Result=0

declare @Err int=0

Update Users set Status=9,ParentID='' where UserID=@UserID and AgentID=@AgentID

Update Users set ParentID='' where ParentID=@UserID

set @Err+=@@error

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