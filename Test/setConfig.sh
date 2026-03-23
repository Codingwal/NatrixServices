curl -i localhost:5031/api/dnsblocker/config \
    -X POST \
    -H "Content-Type: application/json" \
    -d @userConfig.json