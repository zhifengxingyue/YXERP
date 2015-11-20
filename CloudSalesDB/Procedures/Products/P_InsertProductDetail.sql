﻿Use [CloudSales1.0]
GO
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'P_InsertProductDetail')
BEGIN
	DROP  Procedure  P_InsertProductDetail
END

GO
/***********************************************************
过程名称： P_InsertProductDetail
功能描述： 添加子产品
参数说明：	 
编写日期： 2015/8/15
程序作者： Allen
调试记录： exec P_InsertProductDetail 
************************************************************/
CREATE PROCEDURE [dbo].[P_InsertProductDetail]
@ProductID nvarchar(64),
@ProductCode nvarchar(200),
@BigPrice decimal(18,2),
@AttrList nvarchar(max),
@ValueList nvarchar(max),
@AttrValueList nvarchar(max),
@Price decimal(18,2),
@Weight decimal(18,2),
@ProductImg nvarchar(4000),
@Description text,
@ShapeCode nvarchar(50),
@CreateUserID nvarchar(64),
@ClientID nvarchar(64),
@DetailID nvarchar(64) output,
@Result int output--1：成功；0失败
AS

begin tran

declare @Err int
set @Err=0
set @Result=0
set @DetailID=NEWID()


if exists(select AutoID from ProductDetail where ProductID=@ProductID  and [AttrValue]=@ValueList)
begin
	set @DetailID=''
	rollback tran
	return
end

INSERT INTO ProductDetail(ProductDetailID,[ProductID],DetailsCode,BigPrice ,[SaleAttr],[AttrValue],[SaleAttrValue],[Price],[Status],
					Weight,ImgS,[ShapeCode] ,[Description],[CreateUserID],[CreateTime] ,[UpdateTime],[OperateIP] ,[ClientID])
				VALUES(@DetailID,@ProductID,@ProductCode,@BigPrice,@AttrList,@ValueList,@AttrValueList,@Price,1,
					@Weight,@ProductImg,@ShapeCode,@Description,@CreateUserID,getdate(),getdate(),'',@ClientID);
set @Result=1;

set @Err+=@@Error

if(@Err>0)
begin
	set @DetailID=''
	rollback tran
end 
else
begin
	commit tran
end