FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BillingAPI/BillingAPI.csproj", "BillingAPI/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Settings/Settings.csproj", "Settings/"]
COPY ["IoC.Container/IoC.Container.csproj", "IoC.Container/"]
COPY ["IoC.Init/IoC.Init.csproj", "IoC.Container/"]
COPY ["InternalServices/InternalServices.csproj", "InternalServices/"]
COPY ["Serialization/Serialization.csproj", "Serialization/"]
COPY ["FileHelper/FileHelper.csproj", "FileHelper/"]
COPY ["CommonExcel/CommonExcel.csproj", "CommonExcel/"]
COPY ["PubSubService/PubSubService.csproj", "PubSubService/"]

RUN dotnet restore "BillingAPI/BillingAPI.csproj"


COPY . .
WORKDIR /src
RUN dotnet build "BillingAPI/BillingAPI.csproj" -c Release -o /app



FROM build AS publish
RUN dotnet publish "BillingAPI/BillingAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BillingAPI.dll"]