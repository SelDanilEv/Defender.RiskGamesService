docker rm -f DevRiskGamesService
docker build . -t dev-risk-games-service && ^
docker run -d --name DevRiskGamesService -p 49060:80 ^
--env-file ./../../secrets/secrets.dev.list ^
-e ASPNETCORE_ENVIRONMENT=DockerDev ^
-it dev-risk-games-service
echo finish dev-risk-games-service
docker image prune -f
pause
