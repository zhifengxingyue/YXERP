Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_AddStorageDetail')
BEGIN
	DROP  Procedure  P_AddStorageDetail
END

GO
/***********************************************************
过程名称： P_AddStorageDetail
功能描述： 添加单据明细
参数说明：	 
编写日期： 2015/9/18
程序作者： Allen
调试记录： exec P_AddStorageDetail 
************************************************************/
CREATE PROCEDURE [dbo].[P_AddStorageDetail]
@DocID nvarchar(64),
@AutoID int,
@ProductDetailID nvarchar(64),
@Quantity int,
@Price decimal(18,2),
@TotalMoney decimal(18,2)=0,
@BatchCode nvarchar(20)='',
@ClientID nvarchar(64)
AS


declare @WareID nvarchar(64),@DepotID nvarchar(64)

--绑定默认仓库
if exists(select AutoID from ProductQuantity where ProductDetailID=@ProductDetailID)
begin
	select top 1 @WareID=WareID,@DepotID=DepotID from ProductQuantity where ProductDetailID=@ProductDetailID order by BatchCode desc
end
else
begin
	select top 1 @WareID = WareID from WareHouse where ClientID=@ClientID and Status=1
	select top 1 @DepotID = DepotID from DepotSeat where WareID=@WareID and Status=1
end

insert into StorageDetail(DocID,ProductDetailID,ProductID,UnitID,IsBigUnit,Quantity,Price,TotalMoney,WareID,DepotID,BatchCode,Status,Remark,ClientID)
select @DocID,ProductDetailID,ProductID,UnitID,IsBigUnit,@Quantity,@Price,@TotalMoney,@WareID,@DepotID,@BatchCode,0,Remark,@ClientID  from ShoppingCart  where AutoID=@AutoID

--删除购物车信息
delete from ShoppingCart  where AutoID=@AutoID

--处理总金额
declare @Amount decimal(18,2)
select @Amount =sum(TotalMoney) from StorageDetail where DocID=@DocID
update StorageDoc set TotalMoney=@Amount where DocID=@DocID