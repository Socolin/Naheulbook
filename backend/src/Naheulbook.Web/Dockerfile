FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY . /app
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://localhost:5000
EXPOSE 5000
ENTRYPOINT "./Naheulbook.Web"