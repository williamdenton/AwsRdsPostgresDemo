FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as builder
ARG APP_VERSION=1.0.0

# copy in just the csproj, as this file seldom changes and it contains all 
# the nuget packages, we can do a nuget restore and docker will cache this 
# layer for the next build, drastically speeding up docker build time.
ADD ./AwsRdsPostgresDemo.csproj /src/
WORKDIR /src
RUN dotnet restore

#copy the rest of the source files into the builder and compile.
ADD ./ /src/ 
RUN dotnet publish -c Release -p:Version=${APP_VERSION} -o bin/docker/publish

# build is finished, now copy the binaries forward into the runtime container
FROM mcr.microsoft.com/dotnet/core/runtime:3.0

# least privilege dictates we dont run in the container as root
RUN addgroup --system --gid 1000 dotnet && \
    adduser --system --uid 1000 --ingroup dotnet dotnet

COPY --from=builder --chown=dotnet:dotnet  /src/bin/docker/publish /app/
WORKDIR /app
USER dotnet
ENTRYPOINT ["dotnet", "AwsRdsPostgresDemo.dll"]
