# Cities Skylines 2 : Line Tool Lite

This mod enhances existing tools so you can precisely and quickly place objects such as trees, shrubs, and props in lines, curves, and circles with varying parameters. You can also place simple buildings and even vehicles (for amusement value only; [see the limitations](#limitations)).

## Features

- **Integrates directly with in-game tools**: No hotkey required.
- **Place objects in lines**: Create straight lines, curves, or circles.
- Works both *in-game* and *in the editor*.
- **Fence mode**: Automatically align and place objects end-to-end.
- **Accurate placement**: No worrying about imprecision.
- **Adjust spacing and rotation**: Use the in-game tool UI for more control, including random rotation for a more natural look.
- **Random position variation** (optional): Provides natural irregularity.
- **Live preview**: Provisionally place a line, adjust spacing and/or rotation, and see how it looks in real-time before making the final placement (or cancelling).
- Displays distances and angles for fine-tuning.
- Integrates with yenyang's excellent [Tree Controller](https://github.com/yenyang/Tree_Controller_BepInEx) mod.

### UI integration

Version 1.3 of this mod adds a direct User Interface (UI) integration with the game's tool system. This means that Line Tool Lite is *no longer activated by hotkey*. Line modes are activated directly from the new line modes selection at the bottom of the tool options panel:

![Selecton panel location](https://i.imgur.com/90Htbrp.png)

## Requirements

- BepInEx 5

## Installation

1. Install BepInEx 5.
1. Place the `LineToolLite` folder in the archive in your BepInEx `Plugins` folder.

## Start and stop Line Tool Lite

- To activate the tool, select the object that you'd like to place in a line (such as a tree), either normally or with the Dev UI. Then, select a line mode from the line modes options at the bottom of the tool options panel (below Tool Mode; see image above).
- To exit the tool, select 'single placement mode' to return to the normal placement mode for the selected object, or press **Escape**.

## Place a line

- **Click** where you want the line to begin, and click again at the desired endpoint to place the objects. **Note**: Curves require three clicks &mdash; start, guidepoint, and end.
- **Shift-click** at the end of a line starts a new line placement at the spot where the previous line ended.
- **Control-click** at the end of a line leaves it in preview mode; you can adjust the settings and see the results in real-time. You can also drag the highlighted control points (blue circles) to adjust the line positioning (**Control-click** to start dragging ensures that you don't accidentally trigger placement if you miss the point circles, but regular clicking also works). When finished, **click** to place or **right-click** to cancel.
- **Right-click** to cancel placement.

### Tool options

- Toggle **fence mode** to align objects with the line direction, and place them continuously end-to-end.
- Toggle between **straight line**, **curved**, and **circle** modes.
- Adjust **distances** using the arrow buttons &mdash; click for 1m (approximately 1 yard) increments, **Shift-click** for 10m, and **Control-click** for 0.1m. For circle mode, spacing is rounded *up* to the nearest distance that ensures an even placement around the circle.
- Select **fixed-length even spacing mode** to space out objects evenly over the entire length of the line, with spacing *as close as possible* to the spacing distance you set.

    For circle mode, this causes spacing to be rounded to the *nearest number* (up or down) that ensures an even placement around the circle (default circle rounding is always *up*).
- Select **random rotation** to have each object in the line have a different randomly-chosen rotation, or manually adjust the rotation for all items using the arrow buttons &mdash; click for 10-degree increments, **Shift-click** for 90 degrees, **Control-click** for 1 degree.
- Set **variable spacing** greater than zero to apply a random length offset to each item's spacing, up to the maximum distance specified &mdash; click for 1m increments, **Shift-click** for 10m, **Control-click** for 0.1m.
- Set **variable offset** greater than zero to apply a random sideways offset to each item, up to the maximum distance specified &mdash; click for 1m increments, **Shift-click** for 10m, **Control-click** for 0.1m.

    To remove variable spacing and/or offset, set the field(s) back to zero. **Shift-click** (10m increments) to make this faster.

## Limitations

Sub-objects aren't yet supported, so if you use this to place buildings they won't have any of their sub-components (such as props, embedded networks, or sub-buildings). The full version of this mod rectifies this.

Some specialized buildings can cause issues/and crashes, such as carparks.

Vehicles *can* be placed, but will get confused and likely do strange things, which does have its amusement value at least. The full version will allow you to place vehicles as 'props' (so they just sit there and look pretty, and not e.g. rise up nose-first into the sky in various geometric patterns).

## Support

It's usually easiest to contact me at the [Cities: Skylines modding Discord](https://discord.gg/7rTsfUdfTf), or I'm also contactable as u/algernon_A on the [Cities: Skylines modding Subreddit](https://www.reddit.com/r/CitiesSkylinesModding) (r/CitiesSkylinesModding). You could also raise an issue on the GitHub.

## Translations

This mod supports localization. Please help out translating this mod into different languages at the [CrowdIn project](https://crowdin.com/project/line-tool-cs2/)!

## Meta

### Why 'Lite'?

There is a more feature-complete version of this mod in the works, but that requires official modding support to be publicly released (because it uses functionality that isn't present in the current public release of the game). Since we're all getting sick and tired of waiting for that, this is a version for use with BepInEx (and that has all functionality stripped out that *isn't* available in the current public release of the game).

When official modding support (finally!) releases, this mod will be replaced by the full (non-lite) version on Paradox Mods.

### Credits

Special thanks to Captain of Coit for troubleshooting, guidance, and general advice with the UI coding!

### Source code

[Available on GitHub](https://github.com/algernon-A/LineToolLite) (algernon-A/LineToolLite).

As always, *never* trust a mod without publicly available source code!

>You should also be cautious about mods that *do* make their source available. However, mods with source available are off to a better start by already treating you &mdash; the user &mdash; with a modicum of respect.

### Modders (and aspiring modders!)

I'm available and happy to chat about what I've done and answer any questions, and also about how you can implement anything that I've done for your own mods. Come grab me on the Cities: Skylines Modding Discord!

## Disclaimers and legal

The only authorized distribution platforms for this mod, or mods based substantially on the code of this mod, are the GitHub repo linked above, this Thunderstore.io upload, and Paradox Mods (that's the *actual* Paradox Mods at mods.paradoxplaza.com, not the **scam and malware site paradoxmods net** which is explicitly NOT licensed to distribute any of my work, or works derived from my work). Any version or copy of this mod that you encounter elsewhere is most likely being used as a vector for malware and should be ignored.

Downloading, installation, and use of this mod is at your own risk.

>This mod is Copyright 2023-2024 algernon (github.com/algernon-A). All rights reserved. To eliminate any doubt, explicit permission is hereby granted to download this mod for personal use and for Thunderstore.io to distribute it (<- I hope all this isn't actually necessary, but you never know these days). Permission is explicitly NOT granted for further distribution or licensing (<- this bit so I can at least *try* to DMCA the malware-spreaders). If you think you've got a good use-case for an exception to any of this, contact me, let's talk!

>THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

>Have a virtual jellybean if you actually read this far!
