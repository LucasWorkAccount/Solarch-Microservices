﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ["Medical-Record-System/Medical-Record-System.csproj", "Medical-Record-System/"]
RUN dotnet restore "Medical-Record-System/Medical-Record-System.csproj"
COPY . .
WORKDIR "/src/Medical-Record-System"
RUN dotnet build "Medical-Record-System.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Development
RUN dotnet publish "Medical-Record-System.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "Medical-Record-System.dll"]