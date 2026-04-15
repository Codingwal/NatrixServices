export const genAppleConfig = (url: string) => `
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>PayloadContent</key>
    <array>
        <dict>
            <key>DNSSettings</key>
            <dict>
                <key>DNSProtocol</key>
                <string>HTTPS</string>
                <key>ServerURL</key>
                <string>${url}</string>
            </dict>
            <key>PayloadDescription</key>
            <string>Konfiguriert DNS over HTTPS</string>
            <key>PayloadDisplayName</key>
            <string>DNS over HTTPS (DoH)</string>
            <key>PayloadIdentifier</key>
            <string>com.apple.dns.configuration.doh</string>
            <key>PayloadType</key>
            <string>com.apple.dns.settings.managed</string>
            <key>PayloadUUID</key>
            <string>94425152-AII1-DEF2-GHI3-9934564310AB</string>
            <key>PayloadVersion</key>
            <integer>1</integer>
            <key>ServerAddresses</key>
            <array>
                <string>8.8.8.8</string>
            </array>
        </dict>
    </array>
    <key>PayloadDisplayName</key>
    <string>DoH Profil iPad</string>
    <key>PayloadIdentifier</key>
    <string>custom.dns.over.https</string>
    <key>PayloadRemovalDisallowed</key>
    <false/>
    <key>PayloadType</key>
    <string>Configuration</string>
    <key>PayloadUUID</key>
    <string>12HH4178-ABCD-1234-EFGH-33349788142B</string>
    <key>PayloadVersion</key>
    <integer>1</integer>
</dict>
</plist>
`;