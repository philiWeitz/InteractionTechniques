@echo off


cd ..

:: get all packages.config files and store them in a file
dir *packages.config*.* /s /b > tmpFile.txt

:: get the package files and install all defined nuget packages
for /f "tokens=*" %%a in (tmpFile.txt) do (
  echo -- install packages from: %%a
  "Tools/"Nuget.exe install "%%a" -OutputDir packages/
)

:: delete temporary file
del tmpFile.txt


pause