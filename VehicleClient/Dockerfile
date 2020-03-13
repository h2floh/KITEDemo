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
COPY . /app
WORKDIR /app
RUN dotnet restore
RUN dotnet build

# Test
RUN dotnet test

# Package
RUN dotnet publish -c release -r linux-x64 --self-contained

# Create Run Image 
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
#FROM sbidprod.azurecr.io/quinault
#FROM ubuntu

RUN apt-get update
RUN apt-get install libc6-dev --yes
RUN apt-get install libgdiplus --yes

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

WORKDIR /

COPY --from=build /app/bin/release/netcoreapp3.1/linux-x64/publish .

ENTRYPOINT [ "/VehicleClient" ]
