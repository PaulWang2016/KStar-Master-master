@echo off 
pushd %~dp0

REM Server name/ip
SET Server=127.0.0.1    		
REM SQL user name
SET Username=SA		  	
REM SQL user password
SET Password=K2pass!	

REM Checking & Creating databases + tables

@echo Executing 00_Databases.sql file ...
sqlcmd  -S %Server% -U %Username% -P %Password%  -i "00_Databases.sql"

@echo Executing 01_aZaaS.Framework file ...
sqlcmd  -S %Server% -U %Username% -P %Password%  -i "01_aZaaS.Framework.sql"

@echo Executing 02_aZaaS.KStar.sql file ...
sqlcmd  -S %Server% -U %Username% -P %Password% -i "02_aZaaS.KStar.sql"

@echo Executing 03_aZaaS.Framework.Quartznet.sql file ...
sqlcmd  -S %Server% -U %Username% -P %Password%  -i "03_aZaaS.Framework.Quartznet.sql" 

@echo Executing 04_KSTARService.sql file ...
sqlcmd  -S %Server% -U %Username% -P %Password% -i "04_KSTARService.sql"

REM Importing system & sample data

@echo Executing 05_aZaaS.Framework_SampleData.sql file ...
sqlcmd  -S %Server% -U %Username% -P %Password% -i "05_aZaaS.Framework_SampleData.sql"

@echo Executing 06_aZaaS.KStar_SampleData.sql ...
sqlcmd  -S %Server% -U %Username% -P %Password% -i "06_aZaaS.KStar_SampleData.sql"

@echo.
@echo All script files executed successfully!
pause