# Note: Ensure that the 'replay-net' network exists and the PostgreSQL container is connected to it.
# You can create the network using:
# docker network create replay-net

# Stop the container if running
docker stop bar-replay

# Remove the container
docker rm bar-replay

# Build the Docker image
docker build -t bar-replay:latest .

# Run the container with log volume mapping
docker run -d -p 7036:7036 -v ${PWD}\Logs:/Logs --network replay-net -e ASPNETCORE_URLS="http://+:7036" --name bar-replay bar-replay:latest --restart unless-stopped

# Note: Adjust the volume mapping path as necessary for your environment.