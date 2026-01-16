@echo off
cls

SET root=%cd%
SET server=%root%\server

echo Starting server...
cd "%server%"
RustDedicated.exe -batchmode -nographics ^
+rcon.ip 0.0.0.0 ^
+rcon.port 28016 ^
+rcon.password "dk2lksk3" ^
+server.ip 0.0.0.0 ^
+server.port 28015 ^
+server.maxplayers 3 ^
+server.hostname "My Oxide Server" ^
+server.identity "my_server_identity" ^
+server.level "Procedural Map" ^
+server.seed 131302294 ^
+server.worldsize 1000 ^
+server.saveinterval 300 ^
+server.globalchat true ^
+server.description "Powered by Oxide" ^
+server.headerimage "http://i.imgur.com/xNyLhMt.jpg" ^
+server.url "https://oxidemod.org" ^
-logfile "%server%\logs\server_log.txt"