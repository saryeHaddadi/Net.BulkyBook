
Docker 
- SQL Server
docker run `
	-e "ACCEPT_EULA=Y" `
	-e "SA_PASSWORD=111111" `
	-p 1433:1433 `
	-d --name sqlserver `
	mcr.microsoft.com/mssql/server:2019-CU14-ubuntu-20.04
