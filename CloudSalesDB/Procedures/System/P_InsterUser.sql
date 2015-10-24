Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_InsterUser')
BEGIN
	DROP  Procedure  P_InsterUser
END

GO
/***********************************************************
过程名称： P_InsterUser
功能描述： 添加云销用户
参数说明：	 
编写日期： 2015/4/10
程序作者： Allen
调试记录： exec P_InsterUser 
************************************************************/
CREATE PROCEDURE [dbo].[P_InsterUser]
@UserID nvarchar(64),
@LoginName nvarchar(200)='',
@LoginPWD nvarchar(64)='',
@Name nvarchar(200),
@Mobile nvarchar(64)='',
@Email nvarchar(200)='',
@CityCode nvarchar(10)='',
@Address nvarchar(200)='',
@Jobs nvarchar(200)='',
@RoleID nvarchar(64)='',
@DepartID nvarchar(64)='',
@ParentID nvarchar(64)='',
@AgentID nvarchar(64)='',
@MDUserID nvarchar(64)='',
@MDProjectID nvarchar(64)='',
@IsAppAdmin int=0,
@CreateUserID nvarchar(64)='',
@ClientID nvarchar(64)='',
@Result int output --0：失败，1：成功，2 账号已存在
AS

begin tran

set @Result=0

declare @Err int=0

--账号已存在
if(@LoginName<>'' and exists(select UserID from Users where LoginName=@LoginName))
begin
	set @Result=2
	rollback tran
	return
end

--明道账号已存在
if(@MDUserID<>'' and exists(select UserID from Users where MDUserID=@MDUserID))
begin
	set @Result=2
	rollback tran
	return
end

set @Err+=@@error


if(@CreateUserID='') set @CreateUserID=@UserID

if(@MDProjectID<>'' and @AgentID='')
begin
	select @AgentID=AgentID,@ClientID=ClientID from Agents where MDProjectID=@MDProjectID
end

if(@RoleID='' and @IsAppAdmin=1)
begin
	select @RoleID=RoleID from Role where AgentID=@AgentID and IsDefault=1
end

insert into Users(UserID,LoginName,LoginPWD,Name,MobilePhone,Email,CityCode,Address,Jobs,Allocation,Status,IsDefault,ParentID,RoleID,DepartID,CreateUserID,MDUserID,MDProjectID,AgentID,ClientID)
             values(@UserID,@LoginName,@LoginPWD,@Name,@Mobile,@Email,@CityCode,@Address,@Jobs,1,1,0,@ParentID,@RoleID,@DepartID,@CreateUserID,@MDUserID,@MDProjectID,@AgentID,@ClientID)

--部门关系
if(@DepartID<>'')
begin
	insert into UserDepart(UserID,DepartID,CreateUserID,ClientID) values(@UserID,@DepartID,@CreateUserID,@AgentID)  
	set @Err+=@@error
end
   
--角色关系
if(@RoleID<>'')
begin
	insert into UserRole(UserID,RoleID,CreateUserID,ClientID) values(@UserID,@RoleID,@CreateUserID,@AgentID) 
	set @Err+=@@error
end
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