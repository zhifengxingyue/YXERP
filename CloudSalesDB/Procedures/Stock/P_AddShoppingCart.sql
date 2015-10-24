Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_AddShoppingCart')
BEGIN
	DROP  Procedure  P_AddShoppingCart
END

GO
/***********************************************************
过程名称： P_AddShoppingCart
功能描述： 加入购物车
参数说明：	 
编写日期： 2015/9/15
程序作者： Allen
调试记录： exec P_AddShoppingCart 
************************************************************/
CREATE PROCEDURE [dbo].[P_AddShoppingCart]
@OrderType int,
@ProductDetailID nvarchar(64),
@ProductID nvarchar(64),
@Quantity int=1,
@UnitID nvarchar(64),
@IsBigUnit int=0,
@UserID nvarchar(64),
@Remark nvarchar(max),
@OperateIP nvarchar(50)
AS
begin tran

declare @Err int=0

if not exists(select AutoID from ShoppingCart where ProductDetailID=@ProductDetailID and UserID=@UserID and IsBigUnit=@IsBigUnit and OrderType=@OrderType)
begin
	insert into ShoppingCart(OrderType,ProductDetailID,ProductID,UnitID,IsBigUnit,Quantity,Remark,CreateTime,UserID,OperateIP)
	values(@OrderType,@ProductDetailID,@ProductID,@UnitID,@IsBigUnit,@Quantity,@Remark,GETDATE(),@UserID,@OperateIP)
end
else 
begin
	update ShoppingCart set Quantity=Quantity+@Quantity,Remark=@Remark where ProductDetailID=@ProductDetailID and UserID=@UserID and OrderType=@OrderType and IsBigUnit=@IsBigUnit
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