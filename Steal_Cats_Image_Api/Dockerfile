FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /src
COPY . .
RUN dotnet publish Steal_Cats_Image_Api/Steal_Cats_Image_Api.csproj -c Release -o /app
WORKDIR /app
EXPOSE 8080
ENTRYPOINT ["dotnet", "Steal_Cats_Image_Api.dll"]
