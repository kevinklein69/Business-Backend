# syntax=docker/dockerfile:1

# ---- build stage (runs off-box: local Mac or CI, NOT the 2 GB server) ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore first, layer-cached on .csproj changes only.
COPY Business.sln ./
COPY src/Business.API/Business.API.csproj                 src/Business.API/
COPY src/Business.Application/Business.Application.csproj  src/Business.Application/
COPY src/Business.Domain/Business.Domain.csproj            src/Business.Domain/
COPY src/Business.Infrastructure/Business.Infrastructure.csproj src/Business.Infrastructure/
RUN dotnet restore src/Business.API/Business.API.csproj

COPY src/ src/
RUN dotnet publish src/Business.API/Business.API.csproj -c Release -o /app --no-restore

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# QuestPDF -> SkiaSharp needs fontconfig on Linux, otherwise PDF generation throws at runtime.
RUN apt-get update \
 && apt-get install -y --no-install-recommends libfontconfig1 \
 && rm -rf /var/lib/apt/lists/*

# 2 GB box: workstation GC uses far less RAM than the default server GC.
# ponytail: workstation GC is the cheap win; add DOTNET_GCHeapHardLimit only if it still OOMs.
ENV DOTNET_gcServer=0 \
    ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app ./

# Uploads land in /app/App_Data/uploads -> mount a persistent volume there in Coolify.
EXPOSE 8080
ENTRYPOINT ["dotnet", "Business.API.dll"]
