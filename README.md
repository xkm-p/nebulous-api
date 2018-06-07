# nebulous-api
**Unofficial** C# library for the account API of the video game Nebulous ([iOS](https://itunes.apple.com/de/app/nebulous-game/id1069691018) / [Android](https://play.google.com/store/apps/details?id=software.simplicial.nebulous)).

## What this library can do and can't do
This library allows you to access your **own** account, including mails and your friends list.
It does **not** enable hacks, cheats or similar techniques, and neither will I work on a library that allows this.

The library is not complete. Many features will be missing, such as clan functionality, and there's a 99% chance that I will no longer update the library, hence open source. 

* Do whatever you want with the library, just make sure to give credits
* I'm not responsible for any abuse of this library
* I don't care if your account gets banned for using this library

## How to use
1. Add a reference to `Nebulous API.dll` in your project
2. Make sure your project has a reference to Newtonsoft Json.NET (download via NuGet)
2. Call the function `Nebulous.AccountAPI.SetLoginTicket("[Your login ticket]")` before calling any other functions

The rest should be self-explanatory.

The libraries are located in [precompiled](precompiled/). Use these if you don't want to/can't compile the project in the [project](project/) folder.

## How to obtain your login ticket
Unfortunately, as of now the only way to log into your Nebulous account in nebulous-api is by using your login ticket. To obtain this ticket, you will need an Android device with root access.

1. Navigate to `/data/data/software.simplicial.nebulous/shared_prefs`
2. Open the file `application.MainActivity.xml`
3. Locate the `loginTicket` attribute and extract the value


## Credits
* **Newtonsoft Json.NET** (https://www.newtonsoft.com/json)
* **ThePirateBay** for providing me with a free & legal copy of Windows 10 (was really an unexpected gesture)