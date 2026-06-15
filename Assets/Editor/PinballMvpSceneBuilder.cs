using DouyinPinball.Core;
using DouyinPinball.Pinball;
using DouyinPinball.Platform;
using DouyinPinball.UI;
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DouyinPinball.EditorTools
{
    public static class PinballMvpSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Main.unity";
        private const string PrefabFolder = "Assets/Prefabs";
        private const string MaterialFolder = "Assets/Materials";

        [MenuItem("Douyin Pinball/Build MVP Scene")]
        public static void BuildScene()
        {
            EnsureFolders();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            Material tableMaterial = CreateMaterial("TableBlue", new Color(0.05f, 0.12f, 0.25f));
            Material wallMaterial = CreateMaterial("WallMetal", new Color(0.55f, 0.62f, 0.7f));
            Material bumperMaterial = CreateMaterial("BumperOrange", new Color(1f, 0.45f, 0.08f));
            Material ballMaterial = CreateMaterial("BallChrome", new Color(0.9f, 0.92f, 0.95f));
            Material flipperMaterial = CreateMaterial("FlipperRed", new Color(0.9f, 0.12f, 0.16f));
            Material lidMaterial = CreateTransparentMaterial("TransparentLid", new Color(0.6f, 0.85f, 1f, 0.22f));

            GameObject managers = new GameObject("Managers");
            ScoreManager scoreManager = managers.AddComponent<ScoreManager>();
            DouyinPlatform douyinPlatform = managers.AddComponent<DouyinPlatform>();

            GameObject spawnPoint = new GameObject("BallSpawnPoint");
            spawnPoint.transform.position = new Vector3(2.3f, 0.45f, -4.1f);

            CreateCube("Table", Vector3.zero, new Vector3(5.8f, 0.18f, 10.5f), tableMaterial);

            CreateCube("LeftWall", new Vector3(-3f, 0.55f, 0f), new Vector3(0.25f, 0.8f, 10.8f), wallMaterial);
            CreateCube("RightWall", new Vector3(3f, 0.55f, 0f), new Vector3(0.25f, 0.8f, 10.8f), wallMaterial);
            CreateCube("TopWall", new Vector3(0f, 0.55f, 5.3f), new Vector3(6.1f, 0.8f, 0.25f), wallMaterial);
            CreateCube("LauncherLaneWall", new Vector3(1.7f, 0.55f, -0.3f), new Vector3(0.18f, 0.7f, 7.8f), wallMaterial);
            CreateCube("BottomLeftDrainGuide", new Vector3(-1.9f, 0.55f, -4.5f), new Vector3(2.1f, 0.7f, 0.2f), wallMaterial).transform.rotation = Quaternion.Euler(0f, -18f, 0f);
            CreateCube("BottomRightDrainGuide", new Vector3(1.9f, 0.55f, -4.5f), new Vector3(2.1f, 0.7f, 0.2f), wallMaterial).transform.rotation = Quaternion.Euler(0f, 18f, 0f);
            CreateCube("TransparentLid", new Vector3(0f, 1.05f, 0f), new Vector3(5.7f, 0.08f, 10.25f), lidMaterial);

            Rigidbody ballBody = CreateBall(ballMaterial, spawnPoint.transform);
            BallController ball = ballBody.GetComponent<BallController>();

            Transform launchDirection = new GameObject("LaunchDirection").transform;
            launchDirection.position = spawnPoint.transform.position;
            launchDirection.rotation = Quaternion.identity;

            LauncherController launcher = managers.AddComponent<LauncherController>();
            SetObject(launcher, "ball", ballBody);
            SetObject(launcher, "launchDirection", launchDirection);

            FlipperController leftFlipper = CreateFlipper("FlipperLeft", true, KeyCode.A, new Vector3(-1.05f, 0.42f, -3.65f), -18f, flipperMaterial);
            FlipperController rightFlipper = CreateFlipper("FlipperRight", false, KeyCode.D, new Vector3(1.05f, 0.42f, -3.65f), 18f, flipperMaterial);

            CreateBumper("BumperLow", new Vector3(0f, 0.42f, -0.7f), 100, bumperMaterial);
            CreateBumper("BumperLeft", new Vector3(-1.25f, 0.42f, 1.35f), 150, bumperMaterial);
            CreateBumper("BumperRight", new Vector3(1.25f, 0.42f, 2.1f), 150, bumperMaterial);
            CreateBumper("BumperTop", new Vector3(0f, 0.42f, 3.4f), 250, bumperMaterial);

            GameObject failZone = CreateCube("FailZone", new Vector3(0f, 0.7f, -5.25f), new Vector3(5.4f, 1.2f, 0.25f), null);
            failZone.GetComponent<Renderer>().enabled = false;
            failZone.AddComponent<FailZone>();

            HudController hud = CreateHud(launcher);

            GameManager gameManager = managers.AddComponent<GameManager>();
            SetObject(gameManager, "ball", ball);
            SetObject(gameManager, "launcher", launcher);
            SetObject(gameManager, "scoreManager", scoreManager);
            SetObject(gameManager, "hud", hud);
            SetObject(gameManager, "douyinPlatform", douyinPlatform);
            SetObject(launcher, "gameManager", gameManager);
            SetObject(ball, "spawnPoint", spawnPoint.transform);
            SetObject(ball, "gameManager", gameManager);
            SetObject(leftFlipper, "gameManager", gameManager);
            SetObject(rightFlipper, "gameManager", gameManager);

            WireButtons(hud, gameManager);
            CreateCameraAndLights();
            SavePrefabs(ballBody.gameObject, leftFlipper.gameObject, rightFlipper.gameObject);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Douyin Pinball", "MVP scene and prefabs generated.", "OK");
        }

        private static Rigidbody CreateBall(Material material, Transform spawnPoint)
        {
            GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.name = "Ball";
            ball.transform.position = spawnPoint.position;
            ball.transform.localScale = Vector3.one * 0.38f;
            SetMaterial(ball, material);

            Rigidbody body = ball.AddComponent<Rigidbody>();
            body.mass = 1f;
            body.drag = 0.02f;
            body.angularDrag = 0.03f;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            ball.AddComponent<BallController>();
            return body;
        }

        private static FlipperController CreateFlipper(string name, bool isLeft, KeyCode key, Vector3 position, float yAngle, Material material)
        {
            GameObject flipper = CreateCube(name, position, new Vector3(1.45f, 0.22f, 0.34f), material);
            flipper.transform.rotation = Quaternion.Euler(0f, yAngle, 0f);

            Rigidbody body = flipper.AddComponent<Rigidbody>();
            body.mass = 3f;
            body.drag = 0.1f;
            body.angularDrag = 0.1f;

            HingeJoint hinge = flipper.AddComponent<HingeJoint>();
            hinge.axis = Vector3.up;
            hinge.anchor = new Vector3(isLeft ? -0.55f : 0.55f, 0f, 0f);
            hinge.useLimits = true;
            hinge.limits = new JointLimits { min = isLeft ? -5f : -45f, max = isLeft ? 45f : 5f };

            FlipperController controller = flipper.AddComponent<FlipperController>();
            SetBool(controller, "leftFlipper", isLeft);
            SetEnum(controller, "keyboardKey", key);
            return controller;
        }

        private static void CreateBumper(string name, Vector3 position, int score, Material material)
        {
            GameObject bumper = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bumper.name = name;
            bumper.transform.position = position;
            bumper.transform.localScale = new Vector3(0.75f, 0.22f, 0.75f);
            SetMaterial(bumper, material);

            ScoreTrigger trigger = bumper.AddComponent<ScoreTrigger>();
            SetInt(trigger, "scoreValue", score);
        }

        private static HudController CreateHud(LauncherController launcher)
        {
            Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            canvas.gameObject.AddComponent<GraphicRaycaster>();

            if (UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            Text scoreText = CreateText(canvas.transform, "ScoreText", "Score: 0", new Vector2(0f, 860f), 42, TextAnchor.MiddleLeft);
            Text comboText = CreateText(canvas.transform, "ComboText", "Combo x1", new Vector2(0f, 805f), 34, TextAnchor.MiddleLeft);
            Text messageText = CreateText(canvas.transform, "MessageText", "3D Pinball", new Vector2(0f, 260f), 72, TextAnchor.MiddleCenter);
            Button startButton = CreateButton(canvas.transform, "StartButton", "START", new Vector2(0f, -360f));
            Button restartButton = CreateButton(canvas.transform, "RestartButton", "RESTART", new Vector2(0f, -360f));
            Button launchButton = CreateButton(canvas.transform, "LaunchButton", "Launch", new Vector2(320f, -780f));

            HudController hud = canvas.gameObject.AddComponent<HudController>();
            SetObject(hud, "scoreText", scoreText);
            SetObject(hud, "comboText", comboText);
            SetObject(hud, "messageText", messageText);
            SetObject(hud, "startButton", startButton);
            SetObject(hud, "restartButton", restartButton);
            SetObject(hud, "launchButton", launchButton);
            SetObject(hud, "launcher", launcher);
            return hud;
        }

        private static Text CreateText(Transform parent, string name, string content, Vector2 anchoredPosition, int fontSize, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(900f, 120f);
            rect.anchoredPosition = anchoredPosition;

            Text text = textObject.AddComponent<Text>();
            text.text = content;
            text.font = LoadUiFont();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            return text;
        }

        private static Font LoadUiFont()
        {
            try
            {
                return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            catch
            {
                return Font.CreateDynamicFontFromOSFont("Arial", 16);
            }
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition)
        {
            GameObject buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);
            RectTransform rect = buttonObject.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(260f, 92f);
            rect.anchoredPosition = anchoredPosition;

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(1f, 0.72f, 0.18f, 0.92f);
            Button button = buttonObject.AddComponent<Button>();

            Text text = CreateText(buttonObject.transform, "Label", label, Vector2.zero, 36, TextAnchor.MiddleCenter);
            text.rectTransform.sizeDelta = rect.sizeDelta;
            text.color = Color.black;
            return button;
        }

        private static void WireButtons(HudController hud, GameManager gameManager)
        {
            SerializedObject serializedHud = new SerializedObject(hud);
            Button startButton = serializedHud.FindProperty("startButton").objectReferenceValue as Button;
            Button restartButton = serializedHud.FindProperty("restartButton").objectReferenceValue as Button;

            UnityEventTools.AddPersistentListener(startButton.onClick, gameManager.StartGame);
            UnityEventTools.AddPersistentListener(restartButton.onClick, gameManager.RestartGame);
        }

        private static void CreateCameraAndLights()
        {
            Camera camera = new GameObject("Main Camera").AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(0f, 8.5f, -8.2f);
            camera.transform.rotation = Quaternion.Euler(58f, 0f, 0f);
            camera.fieldOfView = 52f;

            Light keyLight = new GameObject("Key Light").AddComponent<Light>();
            keyLight.type = LightType.Directional;
            keyLight.intensity = 1.2f;
            keyLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            Light fillLight = new GameObject("Fill Light").AddComponent<Light>();
            fillLight.type = LightType.Point;
            fillLight.intensity = 1.5f;
            fillLight.range = 12f;
            fillLight.transform.position = new Vector3(0f, 5f, -2f);
        }

        private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.position = position;
            cube.transform.localScale = scale;
            SetMaterial(cube, material);
            return cube;
        }

        private static void SavePrefabs(GameObject ball, GameObject leftFlipper, GameObject rightFlipper)
        {
            PrefabUtility.SaveAsPrefabAsset(ball, $"{PrefabFolder}/Ball.prefab");
            PrefabUtility.SaveAsPrefabAsset(leftFlipper, $"{PrefabFolder}/FlipperLeft.prefab");
            PrefabUtility.SaveAsPrefabAsset(rightFlipper, $"{PrefabFolder}/FlipperRight.prefab");

            GameObject bumper = GameObject.Find("BumperLow");
            if (bumper != null)
            {
                PrefabUtility.SaveAsPrefabAsset(bumper, $"{PrefabFolder}/Bumper.prefab");
            }
        }

        private static Material CreateMaterial(string name, Color color)
        {
            string path = $"{MaterialFolder}/{name}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
            {
                return material;
            }

            material = new Material(Shader.Find("Standard"));
            material.color = color;
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static Material CreateTransparentMaterial(string name, Color color)
        {
            Material material = CreateMaterial(name, color);
            material.SetFloat("_Mode", 3f);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            material.color = color;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void SetMaterial(GameObject gameObject, Material material)
        {
            if (material == null)
            {
                return;
            }

            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        private static void EnsureFolders()
        {
            Directory.CreateDirectory("Assets/Scenes");
            Directory.CreateDirectory(PrefabFolder);
            Directory.CreateDirectory(MaterialFolder);
        }

        private static void SetObject(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetBool(UnityEngine.Object target, string propertyName, bool value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetInt(UnityEngine.Object target, string propertyName, int value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetEnum(UnityEngine.Object target, string propertyName, KeyCode value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).intValue = (int)value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
