Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_CreateBillingPay')
BEGIN
	DROP  Procedure  P_CreateBillingPay
END

GO
/***********************************************************
过程名称： P_CreateBillingPay
功能描述： 添加收款
参数说明：	 
编写日期： 2015/11/19
程序作者： Allen
调试记录： exec P_CreateBillingPay 
************************************************************/
CREATE PROCEDURE [dbo].[P_CreateBillingPay]
	@BillingID nvarchar(64),
	@Type int,
	@PayType int,
	@PayMoney decimal(18,4),
	@PayTime datetime,
	@Remark nvarchar(4000),
	@UserID nvarchar(64),
	@AgentID nvarchar(64),
	@ClientID nvarchar(64)
AS
begin tran

	declare @Err int=0

	declare @TotalMoney decimal(18,4),@TotalPayMoney decimal(18,4),@OrderID nvarchar(64),@BillingCode nvarchar(50)

	insert into BillingPay(BillingID,Type,Status,PayType,PayTime,PayMoney,Remark,CreateTime,CreateUserID,AgentID,ClientID)
			values(@BillingID,@Type,1,@PayType,@PayTime,@PayMoney,@Remark,getdate(),@UserID,@AgentID,@ClientID)
	set @Err+=@@error

	select @TotalMoney=TotalMoney,@TotalPayMoney=PayMoney,@OrderID=OrderID,@BillingCode=BillingCode from Billing where BillingID=@BillingID

	if(@TotalPayMoney+@PayMoney>=@TotalMoney)
	begin
		update Billing set PayMoney=PayMoney+@PayMoney,PayStatus=2,PayTime=getdate() where  BillingID=@BillingID
	end
	else if(@TotalPayMoney+@PayMoney>0)
	begin
		update Billing set PayMoney=PayMoney+@PayMoney,PayStatus=1,PayTime=getdate() where  BillingID=@BillingID
	end
	else
	begin
		update Billing set PayMoney=PayMoney+@PayMoney,PayStatus=0,PayTime=getdate() where  BillingID=@BillingID
	end
	set @Err+=@@error



	--代理商账户处理
	declare @levelMoney decimal(18,4),@FreezeMoney  decimal(18,4),@DefaultAgentID nvarchar(64)
	select @levelMoney=TotalIn-TotalOut-FreezeMoney,@FreezeMoney=FreezeMoney,@DefaultAgentID=AgentID from Clients where ClientID=@ClientID

	if(@DefaultAgentID=@AgentID)
	begin
		insert into ClientAccounts(AgentID,HappenMoney,EndMoney,Mark,SubjectID,Remark,CreateUserID,ClientID)
		values(@AgentID,@PayMoney,@levelMoney+@FreezeMoney+@PayMoney,0,1,'销售账单收款，账单编号：'+@BillingCode,@UserID,@ClientID)

		set @Err+=@@error
		update Clients set TotalIn=TotalIn+@PayMoney where ClientID=@ClientID

		set @Err+=@@error
	end
if(@Err>0)
begin
	rollback tran
end 
else
begin
	commit tran
end