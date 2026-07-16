# YYGQ

A joke [Muse Dash](https://store.steampowered.com/app/774171/Muse_Dash/) mod that reskins the
in-battle judgment into the **YYGQ** set — `NB!` / `SB?` / `TQL` — a [MelonLoader](https://melonwiki.xyz/)
mod that ships as a single, self-contained DLL.

## What it does

- **Judgment words** — `PERFECT` → `NB!`, `GREAT` → `SB?`, `PASS` → its icon, including the fever/gold
  variants (foreground **and** background layer).
- **Note-pickup score** — the `+200 / +600` number becomes `TQL`, and the overlay fades in and out
  with the original animation instead of cutting.
- **DJMax-aware** — when the battle runs in the DJMax score style, the dedicated DJMax art is used;
  every other style falls back to the default set.

All art is embedded in the DLL, so there is nothing to configure — drop it in and play.

## Install

1. Install [MelonLoader](https://melonwiki.xyz/) **0.7.3** (Il2Cpp) into Muse Dash.
2. Drop `YYGQ.dll` into `Muse Dash/Mods/`.
3. Launch the game.

## Build from source

Requires the .NET 6 SDK and a local Muse Dash install with MelonLoader 0.7.3 (its generated
`Il2CppAssemblies` are referenced at build time).

```sh
# point this at your Muse Dash folder (the one containing the MelonLoader directory)
export MD_DIRECTORY="C:/Program Files (x86)/Steam/steamapps/common/Muse Dash"

dotnet build -c Release
# -> bin/Release/YYGQ.dll
```

## How it works

Every in-battle score visual spawns through `Effect.CreateInstance`, so one Harmony postfix sees them
all. Judgment words are `SpriteRenderer`s whose sprite name maps to a theme-agnostic identity
(`Score`/`Gold` + kind + optional `Bg`); the matching embedded art for the active score style is
swapped in over every layer. The pickup score is a `TextMeshPro` number, so its mesh is hidden and an
overlay sprite takes its place, tracking the number's animated alpha each frame.

## Credits

- **Authors** — KARPED1EM & Doushabo
- **Art** — art assets from Muse Dash Custom Play

## License

[GPL-3.0](LICENSE) © KARPED1EM & Doushabo
