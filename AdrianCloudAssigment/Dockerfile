﻿FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
WORKDIR /app

COPY AdrianCloudAssigment/AdrianCloudAssigment.csproj AdrianCloudAssigment/
COPY DataAccess/DataAccess.csproj DataAccess/
COPY Common/Common.csproj Common/
RUN dotnet restore AdrianCloudAssigment/AdrianCloudAssigment.csproj

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "AdrianCloudAssigment.dll"]