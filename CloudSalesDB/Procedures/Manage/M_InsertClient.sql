Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'M_InsertClient')
BEGIN
	DROP  Procedure  M_InsertClient
END

GO
/***********************************************************
过程名称： M_InsertClient
功能描述： 添加客户端
参数说明：	 
编写日期： 2015/4/10
程序作者： Allen
调试记录： exec M_InsertClient 
************************************************************/
CREATE PROCEDURE [dbo].[M_InsertClient]
@ClientID nvarchar(64),
@CompanyName nvarchar(200),
@MobilePhone nvarchar(64),
@Industry nvarchar(64)='',
@CityCode nvarchar(10)='',
@Address nvarchar(200)='',
@Description nvarchar(200)='',
@ContactName nvarchar(50),
@LoginName nvarchar(200)='',
@LoginPWD nvarchar(64)='',
@Email nvarchar(200)='',
@MDUserID nvarchar(64)='',
@MDProjectID nvarchar(64)='',
@CreateUserID nvarchar(64)='',
@Result int output --0：失败，1：成功，2 账号已存在
AS

begin tran

set @Result=0

declare @Err int ,@DepartID nvarchar(64),@RoleID nvarchar(64),@UserID nvarchar(64),@AgentID nvarchar(64)

select @Err=0,@DepartID=NEWID(),@RoleID=NEWID(),@UserID=NEWID(),@AgentID=NEWID()


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

--明道网络已存在
if(@MDProjectID<>'' and exists(select AgentID from Agents where MDProjectID=@MDProjectID))
begin
	set @Result=2
	rollback tran
	return
end

--客户端
insert into Clients(ClientID,CompanyName,ContactName,MobilePhone,Status,Industry,CityCode,Address,Description,AgentID,CreateUserID) 
				values(@ClientID,@CompanyName,@ContactName,@MobilePhone,1,@Industry,@CityCode,@Address,@Description,@AgentID,@CreateUserID)

set @Err+=@@error

--直营代理商
insert into Agents(AgentID,CompanyName,Status,IsDefault,MDProjectID,ClientID) 
			values(@AgentID,'公司直营',1,1,@MDProjectID,@ClientID)

--部门
insert into Department(DepartID,Name,Status,CreateUserID,AgentID,ClientID) values (@DepartID,'系统管理',1,@UserID,@AgentID,@ClientID)
set @Err+=@@error

--角色
insert into Role(RoleID,Name,Status,IsDefault,CreateUserID,AgentID,ClientID) values (@RoleID,'管理员',1,1,@UserID,@AgentID,@ClientID)

set @Err+=@@error

insert into Users(UserID,LoginName,LoginPWD,Name,MobilePhone,Email,Allocation,Status,IsDefault,DepartID,RoleID,CreateUserID,MDUserID,MDProjectID,AgentID,ClientID)
             values(@UserID,@LoginName,@LoginPWD,@ContactName,@MobilePhone,@Email,1,1,1,@DepartID,@RoleID,@UserID,@MDUserID,@MDProjectID,@AgentID,@ClientID)

--部门关系
insert into UserDepart(UserID,DepartID,CreateUserID,ClientID) values(@UserID,@DepartID,@UserID,@ClientID)  
set @Err+=@@error
   
--角色关系
insert into UserRole(UserID,RoleID,CreateUserID,ClientID) values(@UserID,@RoleID,@UserID,@ClientID) 
set @Err+=@@error

--客户来源
insert into CustomSource(SourceID,SourceCode,SourceName,IsSystem,IsChoose,Status,CreateUserID,ClientID)
					values(NEWID(),'Source-Activity','活动',1,0,1,@UserID,@ClientID)
					
insert into CustomSource(SourceID,SourceCode,SourceName,IsSystem,IsChoose,Status,CreateUserID,ClientID)
					values(NEWID(),'Source-Manual','手动添加',1,1,1,@UserID,@ClientID)

--客户阶段
insert into CustomStage(StageID,StageName,Sort,Status,Mark,PID,CreateUserID,ClientID)
values(NEWID(),'新客户',1,1,1,'',@UserID,@ClientID)

insert into CustomStage(StageID,StageName,Sort,Status,Mark,PID,CreateUserID,ClientID)
values(NEWID(),'机会客户',2,1,0,'',@UserID,@ClientID)

insert into CustomStage(StageID,StageName,Sort,Status,Mark,PID,CreateUserID,ClientID)
values(NEWID(),'成交客户',3,1,2,'',@UserID,@ClientID)

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