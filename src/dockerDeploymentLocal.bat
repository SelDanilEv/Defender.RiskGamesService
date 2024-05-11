docker rm -f LocalRiskGamesService
docker build . -t local-risk-games-service && ^
docker run -d --name LocalRiskGamesService -p 47060:80 ^
--env-file ./../../secrets/secrets.local.list ^
-e ASPNETCORE_ENVIRONMENT=DockerLocal ^
-it local-risk-games-service
echo finish local-risk-games-service
docker image prune -f
pause
