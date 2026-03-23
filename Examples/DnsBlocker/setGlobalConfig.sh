curl -i localhost:5031/api/dnsblocker/config/global \
    -X POST \
    -H "Content-Type: application/json" \
    -H "password: admin" \
    -d @globalConfig.json