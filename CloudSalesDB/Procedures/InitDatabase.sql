﻿Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'InitDatabase')
BEGIN
	DROP  Procedure  InitDatabase
END

GO
/***********************************************************
过程名称： InitDatabase
功能描述： 初始化数据库
参数说明：	 
编写日期： 2015/4/10
程序作者： Allen
调试记录： exec InitDatabase 
************************************************************/
CREATE PROCEDURE [dbo].[InitDatabase]
AS
/*清空数据库*/
SELECT identity(int ,1,1) as  id , name Into #table FROM sysobjects  WHERE (xtype = 'u')
declare @tablename nvarchar(100)
declare @execSQL nvarchar(300)
declare @id int 
declare @count int
select @count=max(id) from #table
set @id=1
while @id<=@count
begin
	select @tablename=[name] from #table where id=@id
	set @id=@id+1
	if @tablename in ('City','Country','Menu','Permission','Modules','ModulesMenu','Dictionary','Industry','ExpressCompany')  
	begin
		continue
	end
	set @execSQL =' Truncate table '+ @tablename
	exec (@execSQL)
end
drop table #table

--客户端ID和管理员ID
declare @UserID nvarchar(64)=NEWID()

/*后台-初始化数据*/
insert into M_Users(UserID,LoginName,LoginPWD,Name,IsAdmin,Status,CreateUserID) 
			values(@UserID,'admin','ADA9D527563353B415575BD5BAAE0469','云销科技',1,1,@UserID)


