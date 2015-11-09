Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'M_AddClientOrder')
BEGIN
	DROP  Procedure  M_AddClientOrder
END

GO
/***********************************************************
过程名称： M_AddClientOrder
功能描述： 添加客户端订单
参数说明：	 
编写日期： 2015/11/9
程序作者： MU
调试记录： exec M_AddClientOrder 
************************************************************/
CREATE PROCEDURE [dbo].M_AddClientOrder
@OrderID nvarchar(64),
@UserQuantity int,
@Years int,
@Amount decimal(18,4),
@AgentID nvarchar(64),
@ClientiD nvarchar(64),
@CreateUserID nvarchar(64),

@ProductIDs nvarchar(2000),
@Prices nvarchar(1000),
@Quantitys nvarchar(200)
AS

--添加客户端订单
insert into ClientOrder(OrderID,UserQuantity,Years,Amount,AgentID,ClientiD,CreateUserID)
values(@OrderID,@UserQuantity,@Years,@Amount,@AgentID,@ClientiD,@CreateUserID)

--添加客户订单详情
insert into ClientOrderDetail(OrderID,ProductID,Price,Quantity,CreateUserID)
values(@OrderID,@ProductID,@Price,@Quantity,@CreateUserID)




