*Please note that while I work for Blizzard Entertainment, this project was developed 
on my personal time before I worked for Blizzard. This project does not constitute an 
official documentation of the MPQ or CASC storage container formats, nor is it an 
official mechanism by which to do data-mining. --@robpaveza, 1 Feb. 2016*

StormLibSharp and CascLibSharp
=============

C# wrappers for Ladislav Zezula's [StormLib](https://github.com/ladislav-zezula/StormLib) and [CascLib](https://github.com/ladislav-zezula/CascLib).

*Note: Because MPQ is no longer being used, StormLibSharp is no longer being maintained.  I will accept pull requests if there is interest or demonstrable fixes.  However, my primary focus will be on supporting CascLibSharp and bringing changes online as CascLib evolves.* 

## What do StormLib and CascLib do? ##
If you don't know, why are you even using this?  StormLib is a utility library for reading and writing MPQ files, the archive file format that were used in Blizzard games until World of Warcraft: Warlords of Draenor.  In Warlords, Blizzard switched their file storage format to CASC ([Content Addressable Storage Container](http://wow.gamepedia.com/Content_Addressable_Storage_Container)), which has a number of benefits.

StormLib, and its successor CascLib, are likely most interesting to people who want to data-mine the game files (such as what WoWHead does).

Both StormLib and CascLib have APIs that are very similar to, if not inspired by, the Win32 API for reading and manipulating files.

## Great, so what do the Sharp versions do? ##
StormLibSharp was an effort to expose StormLib's functionality via typical .NET objects.  For example, in the Base Class Library, files are exposed primarily via [FileStream](https://msdn.microsoft.com/en-us/library/system.io.filestream(v=vs.110).aspx).  In addition to providing a baseline set of functionality, it provided utility methods like "Extract File" to support common use cases.

CascLibSharp follows in StormLibSharp's tradition, by wrapping the Win32-like CascLib API into a .NET BCL-like API.  To open a game's data, you create a `CascStorageContext`, supplying the path to the game's data directory (such as `c:\Program Files (x86)\Heroes of the Storm\HeroesData`).  Then, you can enumerate the files, such as via SearchFiles, or if you know a file you want to view, you can just ask to open that file; it will return a `Stream`.

## Usage Notes ##
Be absolutely certain to dispose of all of the objects that you create.  Remember that you're working with native code under the covers; you have to dispose of the objects in order to free them.

Previous versions of WoW maintained a special file called `(listfile)` in each MPQ.  The listfile contained a list of every file in the MPQ.  Warlords of Draenor does not; in order to search CASC, you must provide a path to the listfile.  (You can't provide the text as a string; this is a limitation of CascLib).  Ladik maintains a Warlords listfile within CascLib [here](https://github.com/ladislav-zezula/CascLib/tree/master/listfile).  This limitation does not appear to exist in Starcraft, Heroes, or Overwatch.  (By the way, I am not in the Overwatch beta.  If anyone wants to fix that for me, I'd be very appreciative).

## Example usage ##
For an example, check out the TestConsole project.
