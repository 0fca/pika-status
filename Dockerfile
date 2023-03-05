FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

RUN apt install git
RUN git clone https://github.com/0fca/Pika.Domain
WORKDIR /app/Pika.Domain
RUN dotnet restore
WORKDIR /app/PikaStatus
RUN ls -lah /app/
COPY ./PikaStatus.csproj .
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/PikaStatus/out ./
EXPOSE 12000
ENTRYPOINT ["dotnet", "PikaStatus.dll", "12000"]


