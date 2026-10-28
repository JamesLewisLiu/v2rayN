#!/bin/bash

Arch="$1"
OutputPath="$2"
Version="$3"

FileName="sing-box-${Arch}.zip"
wget -nv -O $FileName "https://github.com/2dust/v2rayN-core-bin/raw/refs/heads/master/$FileName"
7z x $FileName
cp -rf sing-box-${Arch}/* $OutputPath

PackagePath="sing-box-Package-${Arch}"
mkdir -p "$PackagePath/sing-box.app/Contents/Resources"
cp -rf "$OutputPath" "$PackagePath/sing-box.app/Contents/MacOS"
cp -f "$PackagePath/sing-box.app/Contents/MacOS/sing-box.icns" "$PackagePath/sing-box.app/Contents/Resources/AppIcon.icns"
echo "When this file exists, app will not store configs under this folder" > "$PackagePath/sing-box.app/Contents/MacOS/NotStoreConfigHere.txt"
chmod +x "$PackagePath/sing-box.app/Contents/MacOS/sing-box"

cat >"$PackagePath/sing-box.app/Contents/Info.plist" <<-EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleDevelopmentRegion</key>
  <string>en</string>
  <key>CFBundleLocalizations</key>
  <array>
    <string>zh-Hans</string>
    <string>zh-Hant</string>
    <string>en</string>
    <string>fa</string>
    <string>fr</string>
    <string>ru</string>
    <string>hu</string>
  </array>
  <key>CFBundleDisplayName</key>
  <string>sing-box</string>
  <key>CFBundleExecutable</key>
  <string>sing-box</string>
  <key>CFBundleIconFile</key>
  <string>AppIcon</string>
  <key>CFBundleIconName</key>
  <string>AppIcon</string>
  <key>CFBundleIdentifier</key>
  <string>2dust.sing-box</string>
  <key>CFBundleName</key>
  <string>sing-box</string>
  <key>CFBundlePackageType</key>
  <string>APPL</string>
  <key>CFBundleShortVersionString</key>
  <string>${Version}</string>
  <key>CSResourcesFileMapped</key>
  <true/>
  <key>NSHighResolutionCapable</key>
  <true/>
  <key>LSMinimumSystemVersion</key>
  <string>13.6</string>
</dict>
</plist>
EOF

create-dmg \
    --volname "sing-box Installer" \
    --window-size 700 420 \
    --icon-size 100 \
    --icon "sing-box.app" 160 185 \
    --hide-extension "sing-box.app" \
    --app-drop-link 500 185 \
    "sing-box-${Arch}.dmg" \
    "$PackagePath/sing-box.app"
