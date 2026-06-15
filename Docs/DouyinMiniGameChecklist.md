# Douyin Mini Game MVP Checklist

## Editor Validation

- Open the project in Unity or Tuanjie Editor.
- Wait for C# compilation to finish with no console errors.
- Run `Douyin Pinball > Build MVP Scene`.
- Open `Assets/Scenes/Main.unity`.
- Switch Game view to a vertical phone aspect ratio, such as 9:16.
- Press Play and verify:
  - `Start` begins a new run.
  - Holding `Space` or the `Launch` button charges the launcher.
  - Releasing launches the ball up the right lane.
  - `A` or left touch activates the left flipper.
  - `D` or right touch activates the right flipper.
  - Bumpers add score and increase combo.
  - Ball entering the bottom drain shows `Game Over`.
  - `Restart` resets score and ball position.

## Physics Tuning

- Tune `BallController.maxSpeed` if the ball feels too fast.
- Tune `BallController.tableGravity` for table slope feel.
- Tune `LauncherController.minImpulse` and `maxImpulse` for launch strength.
- Tune `FlipperController.spring`, `damper`, and `pressedAngle` for stronger or softer flippers.
- Tune each `ScoreTrigger.scoreValue` to balance score pacing.

## Douyin SDK Integration

- Keep gameplay code independent from platform APIs.
- Wire short and long vibration in `DouyinPlatform`.
- Add share entry points to `DouyinPlatform.ShareGame`.
- Add score submission to the Douyin ranking API in `DouyinPlatform.SubmitScore`.
- Add login, privacy prompt, ads, and analytics only after the core loop is stable.

## Export Readiness

- Confirm the selected build target matches the installed Tuanjie Douyin mini game exporter.
- Use compressed textures and low-poly meshes for mobile performance.
- Profile on a real device before adding visual effects.
- Keep the first package small and avoid unused Unity packages when preparing a production export.
