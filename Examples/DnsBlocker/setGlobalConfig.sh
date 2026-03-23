curl -i http://dnsblocker.ydns.eu:5000/api/dnsblocker/config/global \
    -X POST \
    -H "Content-Type: application/json" \
    -H "password: admin" \
    -d @globalConfig.json