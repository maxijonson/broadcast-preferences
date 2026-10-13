@echo off
if exist "servers\oxide-production\oxide\plugins" copy /Y "lite\BroadcastPreferences.cs" "servers\oxide-production\oxide\plugins\BroadcastPreferences.cs"
if exist "servers\oxide-staging\oxide\plugins" copy /Y "lite\BroadcastPreferences.cs" "servers\oxide-staging\oxide\plugins\BroadcastPreferences.cs"
if exist "servers\carbon-production\carbon\plugins" copy /Y "lite\BroadcastPreferences.cs" "servers\carbon-production\carbon\plugins\BroadcastPreferences.cs"
if exist "servers\carbon-staging\carbon\plugins" copy /Y "lite\BroadcastPreferences.cs" "servers\carbon-staging\carbon\plugins\BroadcastPreferences.cs"
