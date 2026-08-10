# Appium setup

Install and start Appium server where the device/emulator is connected.

Ex: 

```terminal
npm i
./node_modules/.bin/appium  -p 10001 --base-path /wd/hub --relaxed-security --allow-insecure chromedriver_autodownload
```

To allow UiPath Studio to connect to this Appium server, follow these steps on Mac:

```terminal
brew install cloudflared
cloudflared tunnel --url http://127.0.0.1:10001
```

You will see a message in the console with a URL about a tunnel has been created. Use this URL in UiPath Studio.

```
2026-08-10T04:36:53Z INF +--------------------------------------------------------------------------------------------+
2026-08-10T04:36:53Z INF |  Your quick Tunnel has been created! Visit it at (it may take some time to be reachable):  |
2026-08-10T04:36:53Z INF |  https://awareness-bailey-thereof-suddenly.trycloudflare.com                               |
2026-08-10T04:36:53Z INF +--------------------------------------------------------------------------------------------+
```