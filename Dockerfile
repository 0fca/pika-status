FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app/PikaStatus
RUN apt install git
RUN git clone https://github.com/0fca/Pika.Domain
COPY ./PikaStatus.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/PikaStatus/out ./
ENTRYPOINT ["dotnet", "PikaStatus.dll"]


