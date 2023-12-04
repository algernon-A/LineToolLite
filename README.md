# Cities Skylines 2 : Line Tool Lite
- **Place objects in lines** - straight lines, curves, or circles.
- **Fence mode** - automatically align and place objects end-to-end.
- **Accurate placement** - no worrying about imprecision.
- Easily adjust **spacing** and **rotation** using the in-game tool UI (including random rotation for that natural look).
- Optional **random position variation** can provide some natural irregularity.
- Live **previewing** included, so you can provisionally place a line and adjust spacing and/or rotation to see how it looks in real-time before making the final placement (or cancelling).
- Displays **distances** and **angles** for fine-tuning.

Works on all types of objects - trees, shrubs, props.  You can even place simple buildings and even vehicles (for amusement value only - see limitations below).

## Instructions
### To activate the tool:
- Select the object that you'd like to place in a line (e.g. a tree) - either normally or with the Dev UI.
- Press **Control-L** to activate the line tool and bring up the tool UI (in the normal tool options place, towards the bottom-left of the screen).

### To place the line:
- **Left-click** where you want the line to begin, and **left-click again** at the desired endpoint to place the objects (curves require three clicks - start, guidepoint, and end).
- **Right-click** at any time to cancel placement.
- **Shift-click** at the end will start a new line placement at the exact same spot where the previous line ended.
- **Control-click** at the end will leave the line in preview mode; it's not fully placed yet so you can go and adjust the settings and see the results in real time.  When finished, **left-click** to place or **right-click** to cancel.

### Use the tool UI to:
- Toggle **fence mode** - objects will be automatically aligned with the line direction and placed continuously end-to-end.
- Toggle between **straight line**, **curved**, and **circle** modes.
- Adjust **distances** using the arrow buttons - plain click for 1m increments, **shift-click** for 10m, **control-click** for 0.1m.
- Select **random rotation** to have each object in the line have a different randomly-chosen rotation, or otherwise **manually adjust the rotation** for all items using the arrow buttons - plain click for 10-degree increments, **shift-click** for 90 degrees, **control-click** for 1 degree.
- Set **variable spacing** to more than zero to have a random (length-ways) offset applied to each item's spacing, up to the maximum distance specified - plain click for 1m increments, **shift-click** for 10m, **control-click** for 0.1m.
- Set **variable offset** to more than zero to have a random sideways offset applied to each item, up to the maximum distance specified - plain click for 1m increments, **shift-click** for 10m, **control-click** for 0.1m.

To remove variable spacing and/or offset, simply set the field(s) back to zero.  **Shift-clicking** (10m increments) can make this faster.

### To exit the tool:
- Press **Escape**, or
- Select another tool or object.

## Requirements
- BepInEx 5

## Installation
1. Make sure that BepInEx 5 is installed.
1. Place the `LineToolLite` folder in the archive in your BepInEx `Plugins` folder.

## Support
It's usually easiest to contact me at the [**Cities: Skylines modding Discord**](https://discord.gg/ZaH2zjtk), or I'm also contactable as u/algernon_A on the [Cities: Skylines modding Subreddit](https://www.reddit.com/r/CitiesSkylinesModding) (r/CitiesSkylinesModding).  You could also raise an issue on the GitHub.

## Limitations
Sub-objects aren't yet supported in this version, so if you use this to place buildings they won't have any of their sub-components (such as props, embedded networks, or sub-buildings).  The full version of this mod rectifies this.

Vehicles *can* be placed, but will get confused and likely do strange things, which does have its amusement value at least.  The full version will allow you to place vehicles as 'props' (so they just sit there and look pretty, and not e.g. rise up nose-first into the sky in various geometric patterns).

## Meta

### Why 'Lite'?
There is a more feature-complete version of this mod in the works, but that requires official modding support to be publicly released (because it uses functionality that isn't present in the current public release of the game).  Since we're all getting sick and tired of waiting for that, this is a version for use with BepInEx (and that has all functionality stripped out that *isn't* available in the current public release of the game).

When official modding support (finally!) releases, this mod will be replaced by the full (non-lite) version on Paradox Mods.

### Credits
Special thanks to Captain of Coit for troubleshooting, guidance, and general advice with the UI coding!

### Source code
[Available on GitHub](https://github.com/algernon-A/LineToolLite) (algernon-A/LineToolLite).

As always, *never* trust a mod without publicly available source code!

>And still be cautious about mods that *do* make their source available, but at least those mods are off to a better start by already treating you - the user - with at least a modicum of respect.

### Modders
Modders (and aspiring modders!), as always I'm available and happy to chat about what I've done and answer any questions, and also about how you can implement anything that I've done for your own mods.  Come grab me on the Cities: Skylines Modding Discord!

## Disclaimers and legal
The only authorized distribution platforms for this mod, or mods based substantially on the code of this mod, are the GitHub repo linked above, this Thunderstore.io upload, and Paradox Mods.  Any version or copy of this mod that you encounter elsewhere is most likely being used as a vector for malware and should be ignored.

Downloading, installation, and use of this mod is at your own risk.

>This mod is Copyright 2023 algernon (github.com/algernon-A).  All rights reserved.  To eliminate any doubt, explicit permission is hereby granted to download this mod for personal use and for Thunderstore.io to distribute it (<- I hope all this isn't actually necessary, but you never know these days).  Permission is explicitly NOT granted for further distribution or licensing (<- this bit so I can at least *try* to DMCA the malware-spreaders). If you think you've got a good use-case for an exception to any of this, contact me, let's talk!

>THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

>Have a virtual jellybean if you actually read this far!