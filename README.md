# Windows Update Stopper
A tool for preventing Windows Updates Service.

The very beginning running, you will see a red notification line with content "Please install...".
Click "Install" button to install stopping service tool. After that, log screen will notify that installed successfully.

Click "Refresh log", and you will see a green check tick, indicates that now Windows Update Service is turned off on your machine.

![Alt text](https://github.com/vuongsg/ws-update-stopper/blob/master/screenshot.png?raw=true "Title")

## Functions
- Install: install stopping service
- Uninstall: remove stopping service
- Refresh log: refresh log screen
- Load log: load latest log
- Clear old logs: clear old log files
- About: introduction

## Use (for non-developers)
You can download Release.zip, then run "Windows Update Stopper Dashboard.exe".

## Tech stack
- C#
- .NET 6
- WPF
- Blazor Hybrid