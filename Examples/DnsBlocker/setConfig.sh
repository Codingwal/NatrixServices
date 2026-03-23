curl -i http://dnsblocker.ydns.eu:5000/api/dnsblocker/config \
    -X POST \
    -H "Content-Type: application/json" \
    -d @userConfig.json