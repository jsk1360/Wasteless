
-- =============================================================================================================================
-- Author:		
-- Create date: 
-- Description:	A table-valued function that splits a string into rows of substrings, based on a specified separator character.
-- History:
-- =============================================================================================================================
CREATE FUNCTION [dbo].[Split](@data NVARCHAR(MAX), @delim CHAR(1),  @trim int = 0)        
RETURNS @table TABLE ([Value] NVARCHAR(MAX), OrderNbr INT)        
AS        
BEGIN        
DECLARE @idx INT = 1
DECLARE @counter INT = 0      
DECLARE @slice NVARCHAR(MAX)         
      
IF len(@data)<2 OR @data IS NULL  RETURN        
while @idx!= 0        
BEGIN        
	SET @idx = charindex(@delim,@data)        
		IF @idx!=0        
			SET @slice = LEFT(@data,@idx - 1)        
		ELSE        
			SET @slice = @data        
		IF(len(@slice)>0) 
			BEGIN  
				set @counter = @counter + 1
				IF @trim = 0
				BEGIN
				INSERT INTO @table([Value], OrderNbr) VALUES(@slice,@counter)        
				END
				ELSE
				BEGIN 
				INSERT INTO @table([Value], OrderNbr) VALUES(rtrim(ltrim(@slice)),@counter)        
				END
			END
	SET @data = RIGHT(@data,len(@data) - @idx)        
	IF len(@data) = 0 break        
END    
RETURN        
END