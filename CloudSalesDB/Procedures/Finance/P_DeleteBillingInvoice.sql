﻿Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_DeleteBillingInvoice')
BEGIN
	DROP  Procedure  P_DeleteBillingInvoice
END

GO
/***********************************************************
过程名称： P_DeleteBillingInvoice
功能描述： 删除开票
参数说明：	 
编写日期： 2015/11/18
程序作者： Allen
调试记录： exec P_DeleteBillingInvoice 
************************************************************/
CREATE PROCEDURE [dbo].[P_DeleteBillingInvoice]
	@InvoiceID nvarchar(64),
	@BillingID nvarchar(64),
	@UserID nvarchar(64),
	@AgentID nvarchar(64),
	@ClientID nvarchar(64)
AS
begin tran

	declare @Err int=0

	declare @Status int
	select @Status=Status from BillingInvoice where InvoiceID=@InvoiceID

	if(@Status<>0)
	begin
		rollback tran
		return
	end

	Update BillingInvoice set Status=9,UpdateTime=getdate(),UpdateUserID=@UserID where InvoiceID=@InvoiceID

	update Billing set InvoiceStatus=0,InvoiceTime=null where  BillingID=@BillingID
	set @Err+=@@error

if(@Err>0)
begin
	rollback tran
end 
else
begin
	commit tran
end