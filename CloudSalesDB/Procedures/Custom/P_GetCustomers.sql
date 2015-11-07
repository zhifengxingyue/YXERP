﻿Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_GetCustomers')
BEGIN
	DROP  Procedure  P_GetCustomers
END

GO
/***********************************************************
过程名称： P_GetCustomers
功能描述： 获取客户列表
参数说明：	 
编写日期： 2015/11/5
程序作者： Allen
调试记录： exec P_GetCustomers 
************************************************************/
CREATE PROCEDURE [dbo].[P_GetCustomers]
	@SearchType int,
	@SourceID nvarchar(64)='',
	@StageID nvarchar(64)='',
	@Status int=-1,
	@Mark int=-1,
	@SearchUserID nvarchar(64)='',
	@SearchTeamID nvarchar(64)='',
	@SearchAgentID nvarchar(64)='',
	@Keywords nvarchar(4000),
	@BeginTime nvarchar(50)='',
	@EndTime nvarchar(50)='',
	@pageSize int,
	@pageIndex int,
	@totalCount int output ,
	@pageCount int output,
	@UserID nvarchar(64)='',
	@AgentID nvarchar(64),
	@ClientID nvarchar(64)
AS
	declare @tableName nvarchar(4000),
	@columns nvarchar(4000),
	@condition nvarchar(4000),
	@key nvarchar(100),
	@orderColumn nvarchar(4000),
	@isAsc int

	select @tableName='Customer cus left join Contact con on cus.CustomerID=con.CustomerID and con.Status<>9 and con.Type=1 and cus.Type=1 ',
	@columns='cus.CustomerID,cus.Name,cus.Type,cus.SourceID,cus.StageID,con.Name ContactName,isnull(con.MobilePhone,cus.MobilePhone) MobilePhone,isnull(con.Jobs,cus.Jobs) Jobs,cus.OwnerID,cus.CreateTime,cus.Mark,con.ContactID,cus.AgentID,cus.ClientID ',
	@key='cus.AutoID',
	@orderColumn='cus.CreateTime desc',
	@isAsc=0

	set @condition='cus.ClientID='''+@ClientID+''' and cus.Status<>9 '

	if(@SearchAgentID<>'')
	begin
		set @condition +=' and cus.AgentID = '''+@SearchAgentID+''''
	end

	if(@SearchUserID<>'')
	begin
		set @condition +=' and cus.OwnerID = '''+@SearchUserID+''''
	end

	if(@SourceID<>'')
	begin
		set @condition +=' and cus.SourceID = '''+@SourceID+''''
	end

	if(@StageID<>'')
	begin
		set @condition +=' and cus.StageID = '''+@StageID+''''
	end
	
	if(@Status<>-1)
	begin
		set @condition +=' and cus.Status = '+convert(nvarchar(2), @Status)
	end

	if(@Mark<>-1)
	begin
		set @condition +=' and cus.Mark = '+convert(nvarchar(2), @Mark)
	end

	if(@BeginTime<>'')
		set @condition +=' and cus.CreateTime >= '''+@BeginTime+' 0:00:00'''

	if(@EndTime<>'')
		set @condition +=' and cus.CreateTime <=  '''+@EndTime+' 23:59:59'''

	if(@keyWords <> '')
	begin
		set @condition +=' and (cus.Name like ''%'+@keyWords+'%'' or cus.MobilePhone like ''%'+@keyWords+'%'' or cus.Jobs like ''%'+@keyWords+'%'' or con.Name like ''%'+@keyWords+'%'' or con.MobilePhone like ''%'+@keyWords+'%'' or con.Jobs like ''%'+@keyWords+'%'')'
	end

	declare @total int,@page int
	exec P_GetPagerData @tableName,@columns,@condition,@key,@orderColumn,@pageSize,@pageIndex,@total out,@page out,@isAsc 
	select @totalCount=@total,@pageCount =@page
 

