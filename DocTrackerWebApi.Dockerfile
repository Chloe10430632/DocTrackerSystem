FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "DocTrackerWebApi/DocTrackerWebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 如果上面的 ENV 沒效，才需要加這行安裝指令
RUN apt-get update && apt-get install -y tzdata
ENV TZ=Asia/Taipei

ENTRYPOINT ["dotnet", "DocTrackerWebApi.dll"]