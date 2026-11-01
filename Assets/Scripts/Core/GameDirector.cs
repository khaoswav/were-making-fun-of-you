using UnityEngine;
using UnityEngine.UI;

namespace WereMakingFunOfYou
{
    public sealed class GameDirector : MonoBehaviour
    {
        private Text subtitle;
        private Text objective;
        private Text prompt;
        private Canvas canvas;
        private bool started;
        private float messageUntil;

        public static GameDirector Instance { get; private set; }
        public GameLanguage Language { get; private set; } = GameLanguage.English;
        public bool IsStarted => started;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            BuildWorld();
            BuildInterface();
            ShowLanguageMenu();
        }

        private void Update()
        {
            if (subtitle != null && Time.time > messageUntil && subtitle.gameObject.activeSelf)
                subtitle.gameObject.SetActive(false);
        }

        private void BuildWorld()
        {
            RenderSettings.ambientLight = new Color(0.18f, 0.16f, 0.19f);
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Studio Floor";
            floor.transform.position = new Vector3(0, -0.25f, 5);
            floor.transform.localScale = new Vector3(22, 0.5f, 28);
            floor.GetComponent<Renderer>().material.color = new Color(0.12f, 0.11f, 0.14f);

            CreateWall(new Vector3(0, 3, 19), new Vector3(22, 6, 0.5f));
            CreateWall(new Vector3(-11, 3, 5), new Vector3(0.5f, 6, 28));
            CreateWall(new Vector3(11, 3, 5), new Vector3(0.5f, 6, 28));
            CreateWall(new Vector3(0, 3, -9), new Vector3(22, 6, 0.5f));

            var stage = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stage.name = "Host Stage";
            stage.transform.position = new Vector3(0, 0.2f, 13);
            stage.transform.localScale = new Vector3(9, 0.4f, 3);
            stage.GetComponent<Renderer>().material.color = new Color(0.32f, 0.035f, 0.07f);

            var lightObject = new GameObject("Main Studio Spotlight");
            lightObject.transform.position = new Vector3(0, 5.5f, 11);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Spot;
            light.range = 28;
            light.spotAngle = 72;
            light.intensity = 5;
            light.color = new Color(1f, 0.72f, 0.68f);

            var door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "Exit Door";
            door.transform.position = new Vector3(0, 1.5f, -7.8f);
            door.transform.localScale = new Vector3(2.2f, 3, 0.25f);
            door.GetComponent<Renderer>().material.color = new Color(0.42f, 0.035f, 0.06f);
            door.AddComponent<WorldInteractable>().SetContent("door");

            var player = new GameObject("Player");
            player.transform.position = new Vector3(0, 1, -2);
            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.32f;
            var cameraObject = new GameObject("Player Camera");
            cameraObject.transform.SetParent(player.transform);
            cameraObject.transform.localPosition = new Vector3(0, 0.65f, 0);
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            player.AddComponent<FirstPersonController>();
            player.AddComponent<InteractionRaycaster>();
        }

        private static void CreateWall(Vector3 position, Vector3 scale)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.position = position;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().material.color = new Color(0.075f, 0.065f, 0.085f);
        }

        private void BuildInterface()
        {
            var canvasObject = new GameObject("Game HUD");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.AddComponent<GraphicRaycaster>();
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            objective = MakeText("Objective", new Vector2(0, 1), new Vector2(24, -24), 24, TextAnchor.UpperLeft);
            prompt = MakeText("Interaction Prompt", new Vector2(0.5f, 0.5f), new Vector2(0, -62), 22, TextAnchor.MiddleCenter);
            subtitle = MakeText("Host Subtitle", new Vector2(0.5f, 0), new Vector2(0, 80), 26, TextAnchor.MiddleCenter);
            prompt.gameObject.SetActive(false);
            subtitle.gameObject.SetActive(false);
        }

        private Text MakeText(string name, Vector2 anchor, Vector2 position, int size, TextAnchor alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(canvas.transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(850, 90);
            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = alignment;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private void ShowLanguageMenu()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            var panel = new GameObject("Language Selection");
            panel.transform.SetParent(canvas.transform, false);
            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(560, 300);
            var image = panel.AddComponent<Image>();
            image.color = new Color(0.025f, 0.02f, 0.035f, 0.96f);
            var title = MakeText("Title", new Vector2(0.5f, 0.5f), new Vector2(0, 95), 30, TextAnchor.MiddleCenter);
            title.text = "WE'RE MAKING FUN OF YOU";
            title.transform.SetParent(panel.transform, false);
            title.rectTransform.anchoredPosition = new Vector2(0, 95);
            AddMenuButton(panel.transform, "ENGLISH", new Vector2(0, 25), () => SelectLanguage(GameLanguage.English, panel));
            AddMenuButton(panel.transform, "TÜRKÇE", new Vector2(0, -55), () => SelectLanguage(GameLanguage.Turkish, panel));
        }

        private void AddMenuButton(Transform parent, string label, Vector2 position, UnityEngine.Events.UnityAction action)
        {
            var go = new GameObject(label);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(260, 54);
            rect.anchoredPosition = position;
            var image = go.AddComponent<Image>();
            image.color = new Color(0.42f, 0.035f, 0.09f, 1);
            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);
            var labelText = MakeText(label + " Label", new Vector2(0.5f, 0.5f), Vector2.zero, 22, TextAnchor.MiddleCenter);
            labelText.transform.SetParent(go.transform, false);
            labelText.rectTransform.anchorMin = Vector2.zero;
            labelText.rectTransform.anchorMax = Vector2.one;
            labelText.rectTransform.sizeDelta = Vector2.zero;
            labelText.text = label;
        }

        private void SelectLanguage(GameLanguage language, GameObject panel)
        {
            Language = language;
            Destroy(panel);
            StartGame();
        }

        private void StartGame()
        {
            started = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            objective.text = LocalizedText.Get(Language, "objective_intro");
            ShowHostLine("host_intro", 7f);
        }

        public void ShowPrompt(bool visible, string key = "")
        {
            if (prompt == null) return;
            prompt.gameObject.SetActive(visible && started);
            if (visible) prompt.text = LocalizedText.Get(Language, key);
        }

        public void ShowHostLine(string key, float duration = 4f)
        {
            if (subtitle == null) return;
            subtitle.text = LocalizedText.Get(Language, key);
            subtitle.gameObject.SetActive(true);
            messageUntil = Time.time + duration;
        }

        public void Interact(string content)
        {
            if (content == "door") { ShowHostLine("door_locked"); return; }
            ShowHostLine("generic_interact");
        }
    }
}
