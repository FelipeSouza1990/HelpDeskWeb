# ========= build =========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copie csproj e restaure (cache melhor)
COPY HelpDeskWeb.csproj ./
RUN dotnet restore ./HelpDeskWeb.csproj

# copie o restante e publique
COPY . ./
RUN dotnet publish ./HelpDeskWeb.csproj -c Release -o /app/publish

# ========= runtime =========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# criar diretório de dados (para o SQLite no disco do Render)
RUN mkdir -p /var/data

# copiar app publicado
COPY --from=build /app/publish ./

# (opcional) saúde do container
# HEALTHCHECK --interval=30s --timeout=3s CMD wget -qO- http://127.0.0.1:$PORT/health || exit 1

# Render define a variável PORT; o Program.cs já usa ela
ENTRYPOINT ["dotnet", "HelpDeskWeb.dll"]
