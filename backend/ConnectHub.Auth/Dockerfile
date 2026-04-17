FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["ConnectHub.Auth.csproj", "."]
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .

# Force cache bust
ARG CACHE_BUST=1
RUN echo "Cache bust: ${CACHE_BUST}"

ENV ASPNETCORE_URLS=http://+:5046
EXPOSE 5046
ENTRYPOINT ["dotnet", "ConnectHub.Auth.dll"]