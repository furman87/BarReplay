# build the Docker image
docker build -t bar-replay:latest .

# run the Docker container
# docker run -d -p 7036:7036 -e ASPNETCORE_URLS="http://+:7036" --name bar-replay --restart unless-stopped bar-replay:latest
docker run -d -p 7036:7036 -v D:/Dev/BarReplay/Logs:/Logs -e ASPNETCORE_URLS="http://+:7036" --name bar-replay bar-replay:latest --restart unless-stopped

# run a PostgreSQL container
docker run --hostname=af1a45b3d347 --mac-address=ea:18:26:a3:31:9d --env=POSTGRES_PASSWORD=litlbit --env=PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin:/usr/lib/postgresql/16/bin --env=GOSU_VERSION=1.16 --env=LANG=en_US.utf8 --env=PG_MAJOR=16 --env=PG_VERSION=16.1-1.pgdg120+1 --env=PGDATA=/var/lib/postgresql/data --volume=/var/lib/postgresql/data --network=bridge -p 5432:5432 --restart=always --runtime=runc -d postgres