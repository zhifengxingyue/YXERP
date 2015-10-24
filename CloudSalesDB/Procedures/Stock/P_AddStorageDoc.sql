Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_AddStorageDoc')
BEGIN
	DROP  Procedure  P_AddStorageDoc
END

GO
/***********************************************************
过程名称： P_AddStorageDoc
功能描述： 创建单据
参数说明：	 
编写日期： 2015/9/18
程序作者： Allen
调试记录： exec P_AddStorageDoc 
************************************************************/
CREATE PROCEDURE [dbo].[P_AddStorageDoc]
@DocID nvarchar(64),
@DocCode nvarchar(20),
@DocType int,
@TotalMoney decimal(18,2)=0,
@CityCode nvarchar(10)='',
@Address nvarchar(500)='',
@Remark nvarchar(500)='',
@UserID nvarchar(64),
@OperateIP nvarchar(50),
@ClientID nvarchar(64)
AS

insert into StorageDoc(DocID,DocCode,DocType,Status,TotalMoney,CityCode,Address,Remark,CreateUserID,CreateTime,OperateIP,ClientID)
values(@DocID,@DocCode,@DocType,0,@TotalMoney,@CityCode,@Address,@Remark,@UserID,GETDATE(),@OperateIP,@ClientID)