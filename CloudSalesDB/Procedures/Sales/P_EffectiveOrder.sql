Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_EffectiveOrder')
BEGIN
	DROP  Procedure  P_EffectiveOrder
END

GO
/***********************************************************
过程名称： P_EffectiveOrder
功能描述： 生效订单
参数说明：	 
编写日期： 2015/11/15
程序作者： Allen
调试记录： exec P_EffectiveOrder 
************************************************************/
CREATE PROCEDURE [dbo].[P_EffectiveOrder]
	@OrderID nvarchar(64),
	@BillingCode nvarchar(50),
	@OperateID nvarchar(64)='',
	@AgentID nvarchar(64)='',
	@ClientID nvarchar(64)='',
	@Result int output
AS
	
begin tran

set @Result=0

--订单信息
declare @Err int=0,@Status int,@OrderAgentID nvarchar(64),@OwnerID nvarchar(64),@OrderCode nvarchar(50)
select @Status=Status,@OrderAgentID=AgentID,@OwnerID=OwnerID,@OrderCode=OrderCode from Orders where OrderID=@OrderID  and ClientID=@ClientID

if(@Status<>1)
begin
	rollback tran
	return
end

--代理商信息
declare @IsDefault int,@TotalIn decimal(18,4),@TotalOut decimal(18,4),@FreezeMoney decimal(18,4)
select @IsDefault=IsDefault,@TotalIn=TotalIn,@TotalOut=TotalOut,@FreezeMoney=FreezeMoney from Agents where AgentID=@OrderAgentID


--代理商订单信息
declare @TotalMoney decimal(18,4)=0




Update Orders set Status=2,AuditTime=getdate() where OrderID=@OrderID
set @Err+=@@error

insert into OrderUser(OrderID,UserID,Status,CreateTime,CreateUserID,AgentID,ClientID)
		 values(@OrderID,@OwnerID,1,getdate(),@OperateID,@OrderAgentID,@ClientID)
set @Err+=@@error

--生成账单
insert into Billing(BillingID,BillingCode,OrderID,OrderCode,TotalMoney,Status,PayStatus,InvoiceStatus,CreateUserID,AgentID,ClientID)
						values(NEWID(),@BillingCode,@OrderID,@OrderCode,@TotalMoney,1,0,0,@OwnerID,@OrderAgentID,@ClientID)

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

 


 

