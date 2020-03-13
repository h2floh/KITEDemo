FROM sbidprod.azurecr.io/quinault as build

# Update the list of products
RUN apt-get update

# Install wget
RUN apt-get -y install wget

# Install .NET Core SDK
RUN wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt-get update
RUN apt-get -y install dotnet-sdk-3.1

# Last Update & upgrade
RUN apt-get -y update
RUN apt-get -y upgrade

# Compile
COPY . /api
WORKDIR /api
RUN dotnet restore
RUN dotnet build

# Test
RUN dotnet test

# Package
RUN dotnet publish -c release -r linux-x64 --self-contained

# Create Standard SSL
RUN dotnet dev-certs https -ep /api/aspnetapp.pfx -p password

# Create Run Image 
#FROM mcr.microsoft.com/dotnet/core/runtime:3.1
FROM sbidprod.azurecr.io/quinault
#FROM ubuntu

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
# ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS="https://+"
# ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/aspnetapp.pfx
# ENV ASPNETCORE_Kestrel__Certificates__Default__Password=password

WORKDIR /

# COPY --from=build /api/aspnetapp.pfx .
COPY --from=build /api/bin/release/netcoreapp3.1/linux-x64/publish .

ENTRYPOINT [ "/PlatformAPI" ]
