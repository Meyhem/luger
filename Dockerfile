FROM mcr.microsoft.com/dotnet/sdk:5.0 as dotnetbuild

WORKDIR /build/backend
COPY Luger.Api .
RUN dotnet restore && dotnet publish --output ./publish --configuration Release

FROM node:16-alpine as reactbuild
WORKDIR /build/frontend
COPY Luger.React .
RUN yarn && yarn build

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine as runtime
WORKDIR /luger
COPY --from=dotnetbuild /build/backend/publish .
COPY --from=reactbuild /build/frontend/build wwwroot
EXPOSE 80
ENTRYPOINT [ "dotnet", "Luger.Api.dll" ]
