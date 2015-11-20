Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_CreateStorageBillingPay')
BEGIN
	DROP  Procedure  P_CreateStorageBillingPay
END

GO
/***********************************************************
过程名称： P_CreateStorageBillingPay
功能描述： 添加付款
参数说明：	 
编写日期： 2015/11/18
程序作者： Allen
调试记录： exec P_CreateStorageBillingPay 
************************************************************/
CREATE PROCEDURE [dbo].[P_CreateStorageBillingPay]
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

	declare @TotalMoney decimal(18,4),@TotalPayMoney decimal(18,4),@BillingCode nvarchar(50)

	insert into StorageBillingPay(BillingID,Type,Status,PayType,PayTime,PayMoney,Remark,CreateTime,CreateUserID,AgentID,ClientID)
			values(@BillingID,@Type,1,@PayType,@PayTime,@PayMoney,@Remark,getdate(),@UserID,@AgentID,@ClientID)
	set @Err+=@@error

	select @TotalMoney=TotalMoney,@TotalPayMoney=PayMoney,@BillingCode=BillingCode from StorageBilling where BillingID=@BillingID

	if(@TotalPayMoney+@PayMoney>=@TotalMoney)
	begin
		update StorageBilling set PayMoney=PayMoney+@PayMoney,PayStatus=2,PayTime=getdate() where  BillingID=@BillingID
	end
	else if(@TotalPayMoney+@PayMoney>0)
	begin
		update StorageBilling set PayMoney=PayMoney+@PayMoney,PayStatus=1,PayTime=getdate() where  BillingID=@BillingID
	end
	else
	begin
		update StorageBilling set PayMoney=PayMoney+@PayMoney,PayStatus=0,PayTime=getdate() where  BillingID=@BillingID
	end
	set @Err+=@@error

	--代理商账户处理
	declare @levelMoney decimal(18,4),@FreezeMoney  decimal(18,4)
	select @levelMoney=TotalIn-TotalOut-FreezeMoney,@FreezeMoney=FreezeMoney from Clients where ClientID=@ClientID

	insert into ClientAccounts(AgentID,HappenMoney,EndMoney,Mark,SubjectID,Remark,CreateUserID,ClientID)
	values(@AgentID,@PayMoney,@levelMoney+@FreezeMoney-@PayMoney,1,1,'采购账单支出，账单编号：'+@BillingCode,@UserID,@ClientID)

	set @Err+=@@error
	update Clients set TotalOut=TotalOut-@PayMoney where ClientID=@ClientID

	set @Err+=@@error

if(@Err>0)
begin
	rollback tran
end 
else
begin
	commit tran
end