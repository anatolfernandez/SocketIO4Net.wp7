@echo off
set host=127.0.0.1
set port=3000
@echo on
start "node cmd" /B node appEvents.js --host %host% --port %port%
start "chrome" chrome.exe http://%host%:%port%/

