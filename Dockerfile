FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
COPY /Apps /App/Apps
ENV Docker=1
COPY /reset.sh /App/reset.sh
RUN chmod +x /App/reset.sh

# make sh open bash
RUN ln -sf /bin/bash /bin/sh
    
ENTRYPOINT ["dotnet", "Fenrus.dll", "--urls", "http://+:3000"]