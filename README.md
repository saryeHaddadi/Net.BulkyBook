

kube-sarye.francecentral.cloudapp.azure.com:32000

k exec -it mssql-547594998d-2kwwc -- sh
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "MyC0m9l&xP@ssw0rd"


sqlcmd -S mssql-service -U sa -P "MyC0m9l&xP@ssw0rd"
mssql-service






Docker 
- SQL Server
docker run `
	-e "ACCEPT_EULA=Y" `
	-e "SA_PASSWORD=111111" `
	-p 1433:1433 `
	-d --name sqlserver `
	mcr.microsoft.com/mssql/server:2019-CU14-ubuntu-20.04

