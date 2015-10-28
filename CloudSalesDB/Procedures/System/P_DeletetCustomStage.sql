Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_DeletetCustomStage')
BEGIN
	DROP  Procedure  P_DeletetCustomStage
END

GO
/***********************************************************
过程名称： P_DeletetCustomStage
功能描述： 删除客户阶段
参数说明：	 
编写日期： 2015/10/29
程序作者： Allen
调试记录： exec P_DeletetCustomStage 
************************************************************/
CREATE PROCEDURE [dbo].[P_DeletetCustomStage]
@StageID nvarchar(64),
@ClientID nvarchar(64)=''
AS

begin tran


declare @Err int=0,@Sort int=0,@Mark int=0,@Status int=1

select @Sort=Sort,@Mark=Mark,@Status=Status from CustomStage where StageID=@StageID and ClientID=@ClientID
if(@Mark=0 and @Status=1)
begin

update  CustomStage set Status=9 where StageID=@StageID and ClientID=@ClientID 

update  CustomStage set Sort=Sort-1 where  ClientID=@ClientID and Sort>@Sort
end
set @Err+=@@error

if(@Err>0)
begin
	rollback tran
end 
else
begin
	commit tran
end