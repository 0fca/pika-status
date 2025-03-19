FROM 192.168.1.252:5030/pika.cloud/pika.domain AS  build-env

COPY *.csproj .

RUN cd Pika.Domain && dotnet restore

WORKDIR /app
RUN ls -lah /app/
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build-env /app/out ./
EXPOSE 12000
ENTRYPOINT ["dotnet", "PikaStatus.dll", "12000"]