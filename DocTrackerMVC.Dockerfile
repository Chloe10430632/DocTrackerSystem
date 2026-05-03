# 階段 1：編譯環境 (SDK) - 針對 Mac M1 強制使用 arm64
FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製整個方案以利依賴庫 (Library) 讀取
COPY . .
RUN dotnet publish "DocTrackerSystem/DocTrackerSystem.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DocTrackerSystem.dll"]