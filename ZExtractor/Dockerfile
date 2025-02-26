# Use official .NET 6 SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy remaining files and build the application
COPY . ./
RUN dotnet publish -c Release -o out

# Debugging: Ensure the DLL exists after build
RUN ls -lah /app/out/

# Create runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
WORKDIR /app

# Install `ncompress` package to enable `compress` command
RUN apt-get update && apt-get install -y ncompress

# Verify `compress` exists in the right location
RUN ls -lah /usr/bin/compress && chmod +x /usr/bin/compress

# Create a directory for input files
RUN mkdir -p /app/data && chmod -R 777 /app/data

# Copy built application from the build environment
COPY --from=build-env /app/out /app/

# Debugging: Check if ZExtractor.dll exists in runtime image
RUN ls -lah /app/

# Create a sample test file with a header, large data, and footer
RUN echo "This is a test file for .Z extraction" > /app/data/sample.txt \
    && base64 /dev/urandom | head -c 10K >> /app/data/sample.txt \
    && echo "\nEnd of File" >> /app/data/sample.txt \
    && tar -cvf /app/data/sample.tar /app/data/sample.txt \
    && ls -lah /app/data/ \
    && echo "Attempting to compress file..." \
    && /usr/bin/compress -v /app/data/sample.txt || (echo "Compress failed!" && ls -lah /app/data/sample.txt.Z || true) \
    && /usr/bin/compress -v /app/data/sample.tar || (echo "Compress failed!" && ls -lah /app/data/sample.tar.Z || true)

# Verify if compressed files exist
RUN ls -lah /app/data/



# Set entrypoint to run the app
ENTRYPOINT ["dotnet", "/app/ZExtractor.dll"]
