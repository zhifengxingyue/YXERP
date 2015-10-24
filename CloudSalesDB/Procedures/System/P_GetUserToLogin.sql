Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_GetUserToLogin')
BEGIN
	DROP  Procedure  P_GetUserToLogin
END

GO
/***********************************************************
过程名称： P_GetUserToLogin
功能描述： 验证云销系统登录并返回信息
参数说明：	 
编写日期： 2015/4/22
程序作者： Allen
调试记录： exec P_GetUserToLogin 'Admin','ADA9D527563353B415575BD5BAAE0469'
************************************************************/
CREATE PROCEDURE [dbo].[P_GetUserToLogin]
@LoginName nvarchar(200),
@LoginPWD nvarchar(64)
AS

declare @UserID nvarchar(64),@ClientID nvarchar(64),@AgentID nvarchar(64)

select @UserID = UserID,@ClientID=ClientID,@AgentID=AgentID from Users where LoginName=@LoginName and LoginPWD=@LoginPWD

if(@UserID is not null)
begin
    select RoleID into #Roles from UserRole where UserID=@UserID and Status=1

	--会员信息
	select * from Users where UserID=@UserID

	--权限信息

end

 

