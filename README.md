# Rockkeep

For more information, check the [Wiki](https://www.github.com/ctrl-shift-markus/Rockkeep/wiki).

![alt text](https://raw.githubusercontent.com/ctrl-shift-markus/markuscodes/refs/heads/main/assets/images/Rockkeep.ico "Rockkeep icon")

![Static Badge](https://img.shields.io/badge/maintaner-ctrl--shift--markus-blue)
![Static Badge](https://img.shields.io/badge/version-v1.0.0-green)
![Static Badge](https://img.shields.io/badge/language-c%23-yellow)
![Static Badge](https://img.shields.io/badge/language-.NET-red)

# About
Rockkeep is a secure, cross-platform, offline CLI password manager written in C# (.NET 9.0) that stores encrypted passwords on your own computer, meaning **you** have control over everything.

# Parameters
- `-h` - Show Rockkeep's help message
- `-v` - Show the current version
- `-s [service:string]` - Store a password
- `-d [service:string]` - Delete a password
- `-r [service:string]` - Retrieve a password
- `-p` - Purge Rockkeep's folder, wiping all passwords and any other files
- -`g [length:int] [includeSpecialCharacters:bool]` - Generate a random password. The length must be between 8 and 32, and includeSpecialCharacters must be true or false.

# Installation and Storage
## Windows
- Go to the [Releases](https://github.com/ctrl-shift-markus/Rockkeep/releases) page
- Download rockkeep.exe
- Move it to your desired installation path (I recommend `%ProgramFiles%/Rockkeep/rockkeep.exe`)
    - `mv rockkeep.exe %ProgramFiles%/Rockkeep/rockkeep.exe` (Run as Administrator)

## Linux
- Go to the [Releases](https://github.com/ctrl-shift-markus/Rockkeep/releases) page
- Download rockkeep
- Move it to your desired installation path (I recommend `/usr/bin/rockkeep`)
    - `sudo mv rockkeep /usr/bin/rockkeep`

## Manual
### What you need
- Git/GitHub CLI
- .NET 9.0

### How to build
- Clone the repository
    - `git clone https://www.github.com/ctrl-shift-markus/Rockkeep.git`
    - `gh repo clone ctrl-shift-markus/Rockkeep`
- Navigate to Rockkeep/RockkeepApp
    - `cd Rockkeep/RockkeepApp`
- Build the source code
    - `dotnet publish -c Release -r linux-x64` for Linux x64
    - `dotnet publish -c Release -r win-x64` for Windows x64
- Move it to your desired installation path

## Storage
Rockkeep stores your encrypted passwords at `~/.config/Rockkeep/passwords.json` for Linux and `%AppData%\Rockkeep\passwords`.json for Windows, and doesn't store any logs and temp files, just `passwords.json`.





