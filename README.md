# Rust-Legacy-NoRecoil
Dont use this on modern rust lol, this is strictly for the 2013 build of rust legacy and all subversions, running on current rust will definitely have you VAC banned


PROBLEMS:
// This is really just a base and a simpe program I made in 20 minutes ~ 2 years ago, since it's very open source it should be detected on most legacy servers that have any kind of anticheat
// you cannot disable the no recoil until either you leave and restart the game or get manual banned or by the anticheat
// the program will not be able to find the offsets to set the recoil to 0 and give you no recoil UNTIL you shoot any type of weapon in the game.

This is an external command line that literally read and writes memory from mono.dll 
All the purpose is too manipulate the offsets set to the weapon recoil on all the weapons and set them four over or = 0
TLDR; No Recoil for all rust legacy weapons ingame

make sure to not mess with any of the Debug/ release otpions at the top of visual studio, build like normal in net framework 5.0, as a console app.
you will not be able to move the exe external from the folder it was built inside of, I don't really know why that is what it is, but it could be a new mini project for m to figure out. Seems pretty simple as a google search lol.
make sure too shoot any type of weapon inside of the game before opening and THEN open the norecoil exe. this is becuase the offsets have not been found or set yet because the player has not fired a weapon and cannot be determined until the user shoots the weapon.
