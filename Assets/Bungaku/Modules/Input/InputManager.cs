using Bungaku.Utility;
using UnityEngine;
#if USING_LEGACY_INPUT_SYSTEM
using UnityEngine.EventSystems;
#endif

namespace Bungaku.InputSystem
{
    [AddComponentMenu("Bungaku/Managers/Input Manager")]
    public class InputManager : MonoBehaviourSingleton<InputManager>
    {
        // Restrict to be only processed if using Old Input System.
        // If input didn' work, Make sure that you have correctly defined the
        // USING_LEGACY_INPUT_SYSTEM condition in your project settings.
        // To define this condition, go to Edit > Project Settings > Player,
        // and scroll down to the Other Settings section.
        // In the Scripting Define Symbols field, add USING_LEGACY_INPUT_SYSTEM.
#if USING_LEGACY_INPUT_SYSTEM
        // Does player holding down a key?
        public bool m_IsHeldDown { get; private set; }

        // Declare a timer to track the elapsed time.
        float timer;

        // Declare a threshold value to determine how long the mouse button must be held down.
        float threshold = 0.5f;

        public static ArrowKeys ArrowKeys { get; } = new ArrowKeys();
        public static MouseScroll MouseScroll { get; } = new MouseScroll();
        public static MouseClick MouseClick { get; } = new MouseClick();

        #region Return & Backspace
        public bool Return()
        {
            return Input.GetKeyDown(KeyCode.Return);
        }

        public bool Backspace()
        {
            return Input.GetKeyDown(KeyCode.Backspace);
        }
        #endregion

        #region Spacebar
        public bool Spacebar()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                return true;
            else return false;
        }
        #endregion

        #region Event System
        // Just to make sure Event System is exist if you accidentally remove it.
        void InstantiateEventSystem()
        {
            EventSystem eventSystem = EventSystem.current;
            StandaloneInputModule inputModule = FindObjectOfType<StandaloneInputModule>();

            if (eventSystem == null || inputModule == null)
            {
                GameObject newEventSystem = new GameObject("EventSystem");

                if (eventSystem == null)
                    newEventSystem.AddComponent<EventSystem>();

                if (inputModule == null)
                newEventSystem.AddComponent<StandaloneInputModule>();
            }

        }
        #endregion

        #region Built-in
        new void Awake()
        {
            this.Persistent = true;
            base.Awake();

            InstantiateEventSystem();
        }

        void Update()
        {
            // Uhh... lot of checkes
            if (ArrowKeys.Progress() || ArrowKeys.Regress() || MouseClick.Left() || MouseClick.Right() || MouseScroll.Up() || MouseScroll.Down() || Return() || Backspace() || Spacebar())
            {
                // Increment the timer by the elapsed time since the last frame.
                timer += Time.deltaTime;
                // limit timer value to save memory
                timer = Mathf.Clamp(timer, 0f, 2f);
                // Check if the timer has reached the threshold value.
                if (timer >= threshold)
                    m_IsHeldDown = true;
            }
            else
            {
                // Reset the timer if the mouse button is not being held down.
                timer = 0f;
                m_IsHeldDown = false;
            }
        }
        #endregion
    }

    #region Arrow Keys
    public class ArrowKeys
    {
        public bool Progress()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                return true;
            else return false;
        }

        public bool Regress()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                return true;
            else return false;
        }
    }
    #endregion

    #region Mouse Click
    public class MouseClick
    {
        public bool Left()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                return true;
            else if (Input.GetKeyUp(KeyCode.Mouse0))
                return false;
            else return false;
        }

        public bool Right()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
                return true;
            else if (Input.GetKeyUp(KeyCode.Mouse1))
                return false;
            else return false;
        }
    }
    #endregion

    #region Mouse Scroll
    public class MouseScroll
    {
        public bool Up()
        {
            if (Input.mouseScrollDelta.y >= 1f)
                return true;
            else return false;
        }

        public bool Down()
        {
            if (Input.mouseScrollDelta.y <= -1f)
                return true;
            else return false;
        }
    }
    #endregion
#endif
}
