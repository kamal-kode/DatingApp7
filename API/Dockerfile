FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-dev
WORKDIR /app
EXPOSE 8080

#copy csproj and restore distinct layers
COPY *.csproj ./
RUN dotnet restore

#copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

#build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-dev /app/out .
ENTRYPOINT [ "dotnet", "API.dll" ]
