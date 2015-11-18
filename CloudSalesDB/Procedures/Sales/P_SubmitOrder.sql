Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_SubmitOrder')
BEGIN
	DROP  Procedure  P_SubmitOrder
END

GO
/***********************************************************
过程名称： P_SubmitOrder
功能描述： 提交订单
参数说明：	 
编写日期： 2015/11/14
程序作者： Allen
调试记录： exec P_SubmitOrder 
************************************************************/
CREATE PROCEDURE [dbo].[P_SubmitOrder]
	@OrderID nvarchar(64)='',
	@PersonName nvarchar(50)='',
	@MobileTele nvarchar(50)='',
	@CityCode nvarchar(50)='',
	@Address nvarchar(50)='',
	@PostalCode nvarchar(20)='',
	@TypeID nvarchar(64)='',
	@ExpressType int=0,
	@Remark nvarchar(500)='',
	@UserID nvarchar(64)='',
	@AgentID nvarchar(64)='',
	@ClientID nvarchar(64)='',
	@Result int output
AS

set @Result=0	
begin tran

declare @Err int=0, @Status int=0,@TotalMoney decimal(18,2)=0

if not exists(select AutoID from Orders  where OrderID=@OrderID and ClientID=@ClientID and Status=0)
begin
	set @Result=2
	rollback tran
	return
end 

insert into OrderDetail(OrderID,ProductDetailID,ProductID,UnitID,IsBigUnit,Quantity,Price,TotalMoney,Remark,ClientID)
select @OrderID,s.ProductDetailID,s.ProductID, s.UnitID,s.IsBigUnit,s.Quantity,case s.IsBigUnit when 0 then d.Price else d.BigPrice end,0,s.Remark,@ClientID
	from ShoppingCart s join ProductDetail d on d.ProductDetailID=s.ProductDetailID
	where s.[GUID]=@OrderID and s.OrderType=11 

set @Err+=@@error

update OrderDetail set TotalMoney=Price*Quantity where  OrderID=@OrderID and ClientID=@ClientID
set @Err+=@@error

--清空购物车
delete from ShoppingCart where [GUID]=@OrderID and OrderType=11
set @Err+=@@error

select @TotalMoney=sum(TotalMoney) from OrderDetail where OrderID=@OrderID and ClientID=@ClientID

update Orders set Status=1,PersonName=@PersonName,MobileTele=@MobileTele,CityCode=@CityCode,Address=@Address,PostalCode=@PostalCode,TypeID=@TypeID,ExpressType=@ExpressType,Remark=@Remark,TotalMoney=@TotalMoney 
			  where OrderID=@OrderID and ClientID=@ClientID and Status=0

set @Err+=@@error

if(@Err>0)
begin
	rollback tran
end 
else
begin
	set @Result=1
	commit tran
end

 


 

