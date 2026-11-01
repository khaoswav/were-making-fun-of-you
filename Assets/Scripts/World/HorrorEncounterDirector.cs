using System.Collections;
using UnityEngine;

namespace WereMakingFunOfYou
{
    public sealed class HorrorEncounterDirector : MonoBehaviour
    {
        private enum LockerPhase
        {
            None,
            Waiting,
            HostApproach,
            DoorOpening,
            Peeking,
            DoorClosing,
            HostLeaving
        }

        private GameDirector game;
        private Transform player;
        private Transform host;
        private HostCharacterController hostMotion;
        private FirstPersonController playerMotion;
        private CharacterController playerCollider;
        private Transform lockerRoot;
        private Transform lockerDoor;
        private Transform prankDoor;
        private Transform safeRoomDoor;
        private Transform monsterRoot;
        private MonsterChaseController monster;
        private Vector3 hostStagePosition;
        private Vector3 safeRoomCenter;
        private float lockerDoorTarget;
        private float prankDoorTarget;
        private float safeRoomDoorTarget = 78f;
        private float phaseUntil;
        private float safeRoomEnteredAt = -1f;
        private float prankPhaseUntil;
        private int lockerPhase;
        private int prankPhase;
        private int wrongDoorAttempts;
        private bool hidden;
        private bool safeRoomDoorOpen = true;
        private bool safeRoomTriggered;
        private bool fakeScareUsed;

        public void Initialize(GameDirector gameDirector, Transform playerTransform, Transform hostTransform)
        {
            game = gameDirector;
            player = playerTransform;
            host = hostTransform;
            if (player == null || host == null) return;

            hostMotion = host.GetComponent<HostCharacterController>();
            playerMotion = player.GetComponent<FirstPersonController>();
            playerCollider = player.GetComponent<CharacterController>();
            hostStagePosition = host.position;
            BuildEncounterEnvironment();
            BuildMonster();
        }

        private void Update()
        {
            if (game == null || !game.IsStarted || player == null) return;

            AnimateDoor(lockerDoor, lockerDoorTarget);
            AnimateDoor(prankDoor, prankDoorTarget);
            AnimateDoor(safeRoomDoor, safeRoomDoorTarget);

            if (hidden) UpdateLockerSequence();
            if (prankPhase != 0) UpdatePrankDoor();
            UpdateSafeRoomTrigger();
        }

        public bool TryHandleInteraction(string content)
        {
            switch (content)
            {
                case "locker_hide":
                    EnterLocker();
                    return true;
                case "prank_door":
                    StartPrankDoor();
                    return true;
                case "fake_scare":
                    if (!fakeScareUsed)
                    {
                        fakeScareUsed = true;
                        StartCoroutine(FakeScareSequence());
                    }
                    else
                    {
                        game.ShowHostLine("host_fake_scare_repeat");
                    }
                    return true;
                case "safe_room_door":
                    if (!safeRoomTriggered)
                    {
                        safeRoomDoorOpen = !safeRoomDoorOpen;
                        safeRoomDoorTarget = safeRoomDoorOpen ? 78f : 0f;
                        game.ShowHostLine(safeRoomDoorOpen ? "host_safe_room_welcome" : "host_safe_room_door_closed");
                    }
                    else
                    {
                        game.ShowHostLine("host_safe_room_trap");
                    }
                    return true;
                default:
                    return false;
            }
        }

        public void RegisterWrongDoorAttempt()
        {
            wrongDoorAttempts++;
            string line = wrongDoorAttempts switch
            {
                1 => "door_locked",
                2 => "host_wrong_door",
                3 => "host_wrong_door_3",
                _ => "host_wrong_door_4"
            };
            game.ShowHostLine(line);
        }

        private void BuildEncounterEnvironment()
        {
            lockerRoot = new GameObject("Encounter Locker").transform;
            lockerRoot.position = new Vector3(-6.2f, 0f, 4.8f);
            BuildLocker();

            prankDoor = BuildHingedDoor(
                "The Waving Door",
                new Vector3(6.4f, 0f, 4.8f),
                1.55f,
                2.8f,
                "prank_door",
                new Color(0.24f, 0.055f, 0.075f),
                0f);

            safeRoomCenter = new Vector3(7.1f, 0f, -5.2f);
            BuildSafeRoom();

            CreateCube("Fake Scare Pedestal", new Vector3(0f, 0.55f, 6.3f), new Vector3(0.8f, 1.1f, 0.8f), new Color(0.12f, 0.1f, 0.13f));
            Transform scareButton = CreateCube("Definitely Safe Button", new Vector3(0f, 1.22f, 6.3f), new Vector3(0.42f, 0.22f, 0.42f), new Color(0.82f, 0.025f, 0.055f));
            scareButton.gameObject.AddComponent<WorldInteractable>().SetContent("fake_scare");
        }

        private void BuildLocker()
        {
            Color cabinet = new Color(0.16f, 0.17f, 0.2f);
            CreateCube("Locker Left Wall", lockerRoot.position + new Vector3(-0.72f, 1.4f, 0f), new Vector3(0.14f, 2.8f, 1.25f), cabinet);
            CreateCube("Locker Right Wall", lockerRoot.position + new Vector3(0.72f, 1.4f, 0f), new Vector3(0.14f, 2.8f, 1.25f), cabinet);
            CreateCube("Locker Back", lockerRoot.position + new Vector3(0f, 1.4f, -0.58f), new Vector3(1.3f, 2.8f, 0.14f), new Color(0.035f, 0.035f, 0.045f));
            CreateCube("Locker Top", lockerRoot.position + new Vector3(0f, 2.78f, 0f), new Vector3(1.55f, 0.14f, 1.25f), cabinet);
            CreateCube("Locker Base", lockerRoot.position + new Vector3(0f, 0.08f, 0f), new Vector3(1.55f, 0.16f, 1.25f), cabinet);
            lockerDoor = BuildHingedDoor(
                "Locker Door",
                lockerRoot.position + new Vector3(0f, 0f, 0.64f),
                1.42f,
                2.72f,
                "locker_hide",
                new Color(0.23f, 0.24f, 0.28f),
                0f);
            Transform handle = CreateCube(
                "Locker Handle",
                Vector3.zero,
                new Vector3(0.07f, 0.34f, 0.09f),
                new Color(0.72f, 0.7f, 0.62f),
                lockerDoor);
            handle.localPosition = new Vector3(1.24f, 1.35f, 0.11f);
        }

        private void BuildSafeRoom()
        {
            Color wall = new Color(0.22f, 0.24f, 0.25f);
            Vector3 center = safeRoomCenter;
            CreateCube("Safe Room Back Wall", center + new Vector3(0f, 1.65f, -2.05f), new Vector3(5f, 3.3f, 0.2f), wall);
            CreateCube("Safe Room Left Wall", center + new Vector3(-2.4f, 1.65f, 0f), new Vector3(0.2f, 3.3f, 4.2f), wall);
            CreateCube("Safe Room Right Wall", center + new Vector3(2.4f, 1.65f, 0f), new Vector3(0.2f, 3.3f, 4.2f), wall);
            CreateCube("Safe Room Ceiling", center + new Vector3(0f, 3.25f, 0f), new Vector3(5f, 0.2f, 4.2f), new Color(0.12f, 0.13f, 0.14f));
            safeRoomDoor = BuildHingedDoor(
                "Safe Room Door",
                center + new Vector3(0f, 0f, 2.02f),
                1.55f,
                2.75f,
                "safe_room_door",
                new Color(0.35f, 0.39f, 0.38f),
                78f);
        }

        private void BuildMonster()
        {
            monsterRoot = new GameObject("The Studio Stumbler");
            monsterRoot.transform.position = new Vector3(-8.5f, 0f, 12.4f);
            var character = monsterRoot.AddComponent<CharacterController>();
            character.height = 2.65f;
            character.radius = 0.48f;
            character.center = new Vector3(0f, 1.32f, 0f);

            Transform visuals = new GameObject("Monster Visuals").transform;
            visuals.SetParent(monsterRoot.transform, false);
            CreateVisual(PrimitiveType.Capsule, "Monster Body", visuals, new Vector3(0f, 1.05f, 0f), new Vector3(0.9f, 1.1f, 0.6f), new Color(0.045f, 0.035f, 0.05f));
            CreateVisual(PrimitiveType.Sphere, "Monster Head", visuals, new Vector3(0f, 2.1f, 0.02f), new Vector3(0.82f, 0.72f, 0.68f), new Color(0.18f, 0.14f, 0.16f));
            CreateVisual(PrimitiveType.Sphere, "Monster Left Eye", visuals, new Vector3(-0.19f, 2.17f, 0.32f), new Vector3(0.13f, 0.16f, 0.08f), new Color(1f, 0.02f, 0.035f));
            CreateVisual(PrimitiveType.Sphere, "Monster Right Eye", visuals, new Vector3(0.19f, 2.17f, 0.32f), new Vector3(0.13f, 0.16f, 0.08f), new Color(1f, 0.02f, 0.035f));
            CreateVisual(PrimitiveType.Cube, "Monster Mouth", visuals, new Vector3(0f, 1.91f, 0.34f), new Vector3(0.38f, 0.12f, 0.08f), new Color(0.01f, 0f, 0.01f));
            CreateVisual(PrimitiveType.Capsule, "Monster Left Arm", visuals, new Vector3(-0.58f, 1.1f, 0f), new Vector3(0.23f, 0.75f, 0.24f), new Color(0.045f, 0.035f, 0.05f));
            CreateVisual(PrimitiveType.Capsule, "Monster Right Arm", visuals, new Vector3(0.58f, 1.1f, 0f), new Vector3(0.23f, 0.75f, 0.24f), new Color(0.045f, 0.035f, 0.05f));

            monster = monsterRoot.AddComponent<MonsterChaseController>();
            monsterRoot.SetActive(false);
        }

        private void EnterLocker()
        {
            if (hidden || lockerRoot == null) return;

            hidden = true;
            lockerPhase = (int)LockerPhase.Waiting;
            phaseUntil = Time.time + 2.1f;
            lockerDoorTarget = 0f;

            if (playerCollider != null) playerCollider.enabled = false;
            player.position = lockerRoot.position + new Vector3(0f, 1f, -0.08f);
            player.rotation = Quaternion.LookRotation(Vector3.forward);
            if (playerMotion != null) playerMotion.SetControlEnabled(false);

            game.ShowHostLine("host_locker_hide_prompt", 5f);
        }

        private void UpdateLockerSequence()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExitLocker();
                return;
            }

            LockerPhase phase = (LockerPhase)lockerPhase;
            if (phase == LockerPhase.Waiting && Time.time >= phaseUntil)
            {
                hostMotion?.WalkTo(lockerRoot.position + new Vector3(0f, 0.55f, 2.35f));
                lockerPhase = (int)LockerPhase.HostApproach;
                phaseUntil = Time.time + 3f;
            }
            else if (phase == LockerPhase.HostApproach && Time.time >= phaseUntil)
            {
                lockerDoorTarget = 102f;
                lockerPhase = (int)LockerPhase.DoorOpening;
                phaseUntil = Time.time + 1.1f;
            }
            else if (phase == LockerPhase.DoorOpening && Time.time >= phaseUntil)
            {
                lockerPhase = (int)LockerPhase.Peeking;
                phaseUntil = Time.time + 1.9f;
                game.ShowHostLine("host_locker_peek", 3.5f);
            }
            else if (phase == LockerPhase.Peeking && Time.time >= phaseUntil)
            {
                lockerDoorTarget = 0f;
                lockerPhase = (int)LockerPhase.DoorClosing;
                phaseUntil = Time.time + 1.1f;
            }
            else if (phase == LockerPhase.DoorClosing && Time.time >= phaseUntil)
            {
                hostMotion?.WalkTo(hostStagePosition);
                lockerPhase = (int)LockerPhase.HostLeaving;
                phaseUntil = Time.time + 3.2f;
            }
            else if (phase == LockerPhase.HostLeaving && Time.time >= phaseUntil)
            {
                lockerPhase = (int)LockerPhase.None;
            }
        }

        private void ExitLocker()
        {
            hidden = false;
            lockerPhase = (int)LockerPhase.None;
            lockerDoorTarget = 0f;

            if (playerCollider != null) playerCollider.enabled = false;
            player.SetPositionAndRotation(
                lockerRoot.position + new Vector3(0f, 1f, 1.8f),
                Quaternion.LookRotation(Vector3.forward));
            if (playerCollider != null) playerCollider.enabled = true;
            if (playerMotion != null) playerMotion.SetControlEnabled(true);

            game.ShowHostLine("host_locker_exit");
        }

        private void StartPrankDoor()
        {
            if (prankPhase != 0) return;
            prankPhase = 1;
            prankDoorTarget = -94f;
            prankPhaseUntil = Time.time + 1.15f;
        }

        private void UpdatePrankDoor()
        {
            if (prankPhase == 1 && Time.time >= prankPhaseUntil)
            {
                prankDoorTarget = 0f;
                prankPhase = 2;
                prankPhaseUntil = Time.time + 1.15f;
            }
            else if (prankPhase == 2 && Time.time >= prankPhaseUntil)
            {
                prankPhase = 0;
                hostMotion?.TriggerWave(2.4f);
                game.ShowHostLine("host_door_wave");
            }
        }

        private void UpdateSafeRoomTrigger()
        {
            if (safeRoomTriggered || hidden) return;

            Vector2 playerPosition = new Vector2(player.position.x, player.position.z);
            Vector2 roomPosition = new Vector2(safeRoomCenter.x, safeRoomCenter.z);
            bool inside = Mathf.Abs(playerPosition.x - roomPosition.x) < 2.05f &&
                          Mathf.Abs(playerPosition.y - roomPosition.y) < 1.9f;

            if (!inside)
            {
                safeRoomEnteredAt = -1f;
                return;
            }

            if (safeRoomEnteredAt < 0f)
            {
                safeRoomEnteredAt = Time.time;
                game.ShowHostLine("host_safe_room_welcome");
            }

            if (Time.time - safeRoomEnteredAt >= 2.7f)
                TriggerSafeRoomTrap();
        }

        private void TriggerSafeRoomTrap()
        {
            if (safeRoomTriggered) return;
            safeRoomTriggered = true;
            safeRoomDoorOpen = false;
            safeRoomDoorTarget = 0f;
            game.ShowHostLine("host_safe_room_trap", 5f);
            StartCoroutine(RealScareSequence());
        }

        private IEnumerator FakeScareSequence()
        {
            game.ShowHostLine("host_fake_jumpscare", 2f);
            yield return StartCoroutine(ShowScareFace(0.48f));
            game.ShowHostLine("host_fake_jumpscare_punchline", 4f);
        }

        private IEnumerator RealScareSequence()
        {
            yield return new WaitForSeconds(0.75f);
            yield return StartCoroutine(ShowScareFace(0.72f));

            if (monsterRoot == null || monster == null || player == null) yield break;
            monsterRoot.transform.position = hostStagePosition + new Vector3(-1.5f, -0.55f, -2.5f);
            safeRoomDoorTarget = 102f;
            monsterRoot.SetActive(true);
            monster.BeginChase(player, OnMonsterCaught, OnMonsterEnraged);
        }

        private IEnumerator ShowScareFace(float duration)
        {
            Camera view = player != null ? player.GetComponentInChildren<Camera>() : null;
            if (view == null) yield break;

            Transform face = new GameObject("One-Frame Nightmare").transform;
            face.SetParent(view.transform, false);
            face.localPosition = new Vector3(0f, 0f, 0.82f);
            CreateVisual(PrimitiveType.Sphere, "Scare Face", face, Vector3.zero, new Vector3(1.15f, 1.35f, 0.36f), new Color(0.055f, 0.018f, 0.03f));
            CreateVisual(PrimitiveType.Sphere, "Scare Left Eye", face, new Vector3(-0.25f, 0.2f, 0.31f), new Vector3(0.19f, 0.23f, 0.1f), Color.red);
            CreateVisual(PrimitiveType.Sphere, "Scare Right Eye", face, new Vector3(0.25f, 0.2f, 0.31f), new Vector3(0.19f, 0.23f, 0.1f), Color.red);
            CreateVisual(PrimitiveType.Cube, "Scare Mouth", face, new Vector3(0f, -0.32f, 0.31f), new Vector3(0.46f, 0.22f, 0.09f), Color.black);
            yield return new WaitForSeconds(duration);
            if (face != null) Destroy(face.gameObject);
        }

        private void OnMonsterEnraged()
        {
            game.ShowHostLine("host_monster_enraged", 3.5f);
        }

        private void OnMonsterCaught()
        {
            StartCoroutine(MonsterCaughtSequence());
        }

        private IEnumerator MonsterCaughtSequence()
        {
            game.ShowHostLine("host_real_jumpscare", 3f);
            yield return StartCoroutine(ShowScareFace(0.9f));
            if (monsterRoot != null) monsterRoot.SetActive(false);
            if (player != null)
            {
                if (playerCollider != null) playerCollider.enabled = false;
                player.position = new Vector3(0f, 1f, -2f);
                player.rotation = Quaternion.LookRotation(Vector3.forward);
                if (playerCollider != null) playerCollider.enabled = true;
            }
            if (playerMotion != null) playerMotion.SetControlEnabled(true);
        }

        private static Transform BuildHingedDoor(
            string objectName,
            Vector3 center,
            float width,
            float height,
            string contentKey,
            Color color,
            float initialAngle)
        {
            Transform pivot = new GameObject(objectName + " Pivot").transform;
            pivot.position = center + new Vector3(-width * 0.5f, 0f, 0f);
            Transform panel = CreateCube(objectName, Vector3.zero, new Vector3(width, height, 0.14f), color, pivot);
            panel.localPosition = new Vector3(width * 0.5f, height * 0.5f, 0f);
            panel.gameObject.AddComponent<WorldInteractable>().SetContent(contentKey);
            pivot.localRotation = Quaternion.Euler(0f, initialAngle, 0f);
            return pivot;
        }

        private static Transform CreateCube(string name, Vector3 position, Vector3 scale, Color color, Transform parent = null)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            if (parent != null)
            {
                cube.transform.SetParent(parent, false);
                cube.transform.localPosition = position;
                cube.transform.localScale = scale;
            }
            else
            {
                cube.transform.position = position;
                cube.transform.localScale = scale;
            }
            cube.GetComponent<Renderer>().material.color = color;
            return cube.transform;
        }

        private static void CreateVisual(PrimitiveType shape, string name, Transform parent, Vector3 localPosition, Vector3 localScale, Color color)
        {
            GameObject part = GameObject.CreatePrimitive(shape);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            part.GetComponent<Renderer>().material.color = color;
            Collider partCollider = part.GetComponent<Collider>();
            if (partCollider != null) Destroy(partCollider);
        }

        private static void AnimateDoor(Transform door, float targetAngle)
        {
            if (door == null) return;
            Quaternion target = Quaternion.Euler(0f, targetAngle, 0f);
            door.localRotation = Quaternion.Slerp(door.localRotation, target, Time.deltaTime * 6.5f);
        }
    }
}