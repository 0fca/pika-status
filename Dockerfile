FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app/PikaStatus
COPY ./PikaStatus.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/PikaStatus/out ./
ENTRYPOINT ["dotnet", "PikaStatus.dll"]


