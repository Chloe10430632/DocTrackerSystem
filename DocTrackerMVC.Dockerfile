# 階段 1：編譯環境 (SDK) - 針對 Mac M1 強制使用 arm64
FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製整個方案以利依賴庫 (Library) 讀取
COPY . .
RUN dotnet publish "DocTrackerSystem/DocTrackerSystem.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 如果上面的 ENV 沒效，才需要加這行安裝指令
RUN apt-get update && apt-get install -y tzdata
ENV TZ=Asia/Taipei

ENTRYPOINT ["dotnet", "DocTrackerSystem.dll"]