# 3D Pinball Douyin Mini Game MVP

Unity/Tuanjie MVP for a vertical-screen 3D pinball game targeting Douyin mini game export.

## Quick Start

1. Open this folder with Unity or Tuanjie Editor.
2. Wait for scripts to compile.
3. Run `Douyin Pinball > Build MVP Scene` from the editor menu.
4. Open `Assets/Scenes/Main.unity`.
5. Press Play.

## Controls

- Start: click the `Start` button.
- Launch: hold and release `Space`, or hold and release the `Launch` UI button.
- Left flipper: `A`, left arrow, or touch the left half of the screen.
- Right flipper: `D`, right arrow, or touch the right half of the screen.
- Restart: click `Restart` after game over.

## MVP Scope

- Single 3D pinball table.
- Physics ball with speed clamp and table gravity.
- Left and right flippers.
- Hold-to-charge launcher.
- Score bumpers with combo multiplier.
- Game over and restart flow.
- Lightweight Douyin platform adapter placeholder.

## Douyin Export Notes

The current implementation keeps Douyin-specific features behind `Assets/Scripts/Platform/DouyinPlatform.cs`.
After the core gameplay is tuned, wire this adapter to the SDK version installed in your Tuanjie Editor for share, vibration, ranking, login, and ads.

See `Docs/DouyinMiniGameChecklist.md` for editor validation, tuning, and export readiness steps.
