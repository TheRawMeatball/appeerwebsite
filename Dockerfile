FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy everything and build
COPY . ./

RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ENV passwd="mypwd"
ENV DATABASE_ADDRESS=""
ENV db="Main"
ENV uid="root"
ENV data_path="/var/lib/appeer/"
RUN mkdir https/
COPY --from=build-env /app/Shared/person.svg+xml ${data_path}
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT dotnet csharpwebsite.Server.dll /ConnectionStrings:WebApiDatabase="Server=${DATABASE_ADDRESS};Database=${db};uid=${uid};Password=${passwd};CharSet=utf8mb4" /AppSettings:UploadPath="${data_path}uploads/" /AppSettings:DefaultAvatarPath="${data_path}person.svg+xml"