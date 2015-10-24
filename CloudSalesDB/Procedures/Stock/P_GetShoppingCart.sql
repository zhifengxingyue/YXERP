Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_GetShoppingCart')
BEGIN
	DROP  Procedure  P_GetShoppingCart
END

GO
/***********************************************************
过程名称： P_GetShoppingCart
功能描述： 获取购物车详情
参数说明：	 
编写日期： 2015/7/1
程序作者： Allen
调试记录： exec P_GetShoppingCart 1,'1104c2fd-e9b6-4ee5-b26d-aaa927cb15f6'
************************************************************/
CREATE PROCEDURE [dbo].[P_GetShoppingCart]
	@OrderType int,
	@UserID nvarchar(64)
AS

select s.AutoID,s.ProductDetailID,s.ProductID,s.Quantity,s.Remark Description,p.ProductName,u.UnitID,u.UnitName,case s.IsBigUnit when 0 then d.Price else d.BigPrice end Price,d.Imgs 
from ShoppingCart s 
join ProductDetail d on d.ProductDetailID=s.ProductDetailID
join Products p  on s.ProductID=p.ProductID
join ProductUnit u on s.UnitID=u.UnitID
where s.UserID=@UserID and s.OrderType=@OrderType


 

