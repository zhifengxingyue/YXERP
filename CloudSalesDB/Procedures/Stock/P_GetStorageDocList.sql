Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_GetStorageDocList')
BEGIN
	DROP  Procedure  P_GetStorageDocList
END

GO
/***********************************************************
过程名称： P_GetStorageDocList
功能描述： 获取单据列表
参数说明：	 
编写日期： 2015/6/29
程序作者： Allen
调试记录： exec P_GetStorageDocList 
			@UserID='2606068a-681c-47df-8338-ddb8fb0e1895',
			@KeyWords='',
			@DocType=1,
			@Status=-1,
			@PageSize=20,
			@PageIndex=1,
			@ClientID='f24d8a95-5fa4-41ef-b5ad-390b834618c3'
			
************************************************************/
CREATE PROCEDURE [dbo].[P_GetStorageDocList]
	@UserID nvarchar(64)='',
	@KeyWords nvarchar(4000),
	@DocType int,
	@Status int=-1,
	@PageSize int,
	@PageIndex int,
	@TotalCount int=0 output ,
	@PageCount int=0 output,
	@ClientID nvarchar(64)
AS
	declare @tableName nvarchar(4000),
	@columns nvarchar(4000),
	@condition nvarchar(4000),
	@key nvarchar(100),
	@orderColumn nvarchar(4000),
	@isAsc int

	select @tableName='StorageDoc',@columns='* ',@key='AutoID',@orderColumn='CreateTime desc',@isAsc=0
	set @condition=' ClientID='''+@ClientID+''' and Status<>9 '
	--关键词
	if(@keyWords <> '')
	begin
		set @condition +=' and (DocCode like ''%'+@KeyWords+'%'' or  PersonName like ''%'+@KeyWords+'%'' or  MobileTele like ''%'+@KeyWords+'%'') '
	end
	--创建人
	if(@UserID<>'')
	begin
		set @condition += ' and CreateUserID='''+@UserID+''''
	end
	--单据类型
	if(@DocType<>-1)
	begin
		set @condition += ' and DocType='+str(@DocType)
	end
	--状态
	if(@Status<>-1)
	begin
		set @condition += ' and Status='+str(@Status)
	end

	declare @total int,@page int
	exec P_GetPagerData @tableName,@columns,@condition,@key,@orderColumn,@PageSize,@PageIndex,@total out,@page out,@isAsc 
	select @TotalCount=@total,@PageCount =@page
 

