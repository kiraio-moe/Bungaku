using Bungaku.InputSystem;
using Bungaku.UI;
using Bungaku.Utility;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if USING_LEGACY_INPUT_SYSTEM
using UnityEngine.EventSystems;
#endif

namespace Bungaku.Core
{
    [AddComponentMenu("Bungaku/Managers/Dialogue Manager")]
    public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
    {
        [Header("Inputs")]

        [Tooltip("Use Arrow Keys to progress/regress the story?")]
        [SerializeField] bool m_UseArrowKeys = true;

        [Tooltip("Use Mouse Button to progress/regress the story?")]
        [SerializeField] bool m_UseMouseButton = true;

        [Tooltip("Use Mouse Scroll to progress/regress the story?")]
        [SerializeField] bool m_UseMouseScroll = true;

        [Tooltip("Use Return key to progress and Backspace key to regress the story?")]
        [SerializeField] bool m_UseReturnBackspace = true;

        [Tooltip("Use Spacebar key to progress the story?")]
        [SerializeField] bool m_UseSpacebar = true;

        bool allowInput = true;

        [Header("Story")]

        [Tooltip("Autoplay the story?")]
        [SerializeField] bool m_Autoplay = true;

        [Tooltip("Story to play")]
        [SerializeField] TextAsset m_Story;

        Story currentStory;
        // Stack<int> history = new Stack<int>();

        [Header("Dialogue")]

        [Tooltip("Typing effect delay in seconds")]
        [Range(0f, 1f)]
        [SerializeField] float m_DialogueTypingDelay = .02f;

        string currentDialogue;
        bool isTyping = false;

        InputManager inputManager;
        BungakuCanvas bungakuCanvas;

        new void Awake()
        {
            // Make this script as persistent singleton
            this.Persistent = true;
            base.Awake();

            // Instatiate the following
            inputManager = InputManager.Instance;
            bungakuCanvas = BungakuCanvas.Instance;

            if (bungakuCanvas.m_DialogueText == null)
                Debug.LogError(
                    "Dialogue text placeholder is missing! You need to assign it at Bungaku Canvas."
                );
        }

        void Start()
        {
            if (m_Story != null && m_Autoplay)
                EnterDialogueMode(m_Story);
            else
                Debug.LogError("Story is missing! You need to assign it at Dialogue Manager");
        }

        void Update()
        {
            if (allowInput)
            {
                if (ProgressInputs())
                    ProgressStory();
                else if (RegressInputs())
                    RegressStory();

                // if (ProgressInputs() && isTyping)
                //     StopCoroutine(TypeText(bungakuCanvas.m_DialogueText, currentDialogue));
            }
        }

        #region Inputs
#if USING_LEGACY_INPUT_SYSTEM
        bool ProgressInputs()
        {
            bool input = false;

            if (m_UseArrowKeys && InputManager.ArrowKeys.Progress())
                input = true;
            if (m_UseMouseButton && InputManager.MouseClick.Left())
                input = true;
            if (m_UseMouseScroll && InputManager.MouseScroll.Up())
                input = true;
            if (m_UseReturnBackspace && inputManager.Return())
                input = true;
            if (m_UseSpacebar && inputManager.Spacebar())
                input = true;

            return input;
        }

        bool RegressInputs()
        {
            bool input = false;

            if (m_UseArrowKeys && InputManager.ArrowKeys.Regress())
                input = true;
            if (m_UseMouseButton && InputManager.MouseClick.Right())
                input = true;
            if (m_UseMouseScroll && InputManager.MouseScroll.Down())
                input = true;
            if (m_UseReturnBackspace && inputManager.Backspace())
                input = true;

            return input;
        }
#endif
        #endregion

        #region Story related functions
        public void EnterDialogueMode(TextAsset story)
        {
            // Create new Story object
            currentStory = new Story(story.text);

            // Start the story
            ProgressStory();
        }

        public void ExitDialogueMode()
        {
            // Disable dialogue UI
            bungakuCanvas.m_DialogueCanvas.enabled = false;
            // Set dialog text to empty
            bungakuCanvas.m_DialogueText.text = "";
        }

        public void ProgressStory()
        {
            // Check if we be able to progress the story
            if (currentStory.canContinue)
            {
                // Get the current line of dialogue and display it
                currentDialogue = currentStory.Continue();

                // Removes any white space from the dialogue.
                currentDialogue = currentDialogue.Trim();

                // Assign the dialogue
                bungakuCanvas.m_DialogueText.text = currentDialogue;
                // StartCoroutine(TypeText(bungakuCanvas.m_DialogueText, currentDialogue));

                // Save the current knot or stitch index in the history
                // int currentIndex = currentStory.state.currentTurnIndex;
                // history.Push(currentIndex);

                #region Choices
                // Store existing choice into a list
                List<Button> choiceButtons = new List<Button>();
                foreach (Button choice in bungakuCanvas.m_ChoicesCanvas.GetComponentsInChildren<Button>())
                    choiceButtons.Add(choice);

                // Clear any existing choices
                foreach (Button choice in choiceButtons)
                    Destroy(choice.gameObject);

                // Clear the list
                choiceButtons.Clear();

                // Get the available choices
                List<Choice> choices = currentStory.currentChoices;

                // If choices present...
                if (choices.Count > 0)
                {
                    // Disable progress/regress story input
                    allowInput = false;

                    // Create UI buttons for each choice
                    foreach (Choice choice in choices)
                    {
                        // Some checks
                        if (bungakuCanvas.m_ChoicePrefab == null)
                        {
                            Debug.LogError("Choice prefab is missing! You need to assign it at Bungaku Canvas");
                            return;
                        }

                        if (bungakuCanvas.m_ChoicesCanvas == null)
                        {
                            Debug.LogError("Choice container that act as parent choices is missing! You need to assign it at Bungaku Canvas");
                            return;
                        }

                        // Create Button from prefab
                        Button choiceButton = Instantiate(bungakuCanvas.m_ChoicePrefab, bungakuCanvas.m_ChoicesCanvas.gameObject.transform);

                        // Add to choice buttons list
                        choiceButtons.Add(choiceButton);

                        // Assign choice text
                        TextMeshProUGUI tmp = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
                        if (tmp != null)
                            tmp.text = choice.text;
                        else
                        {
                            Debug.LogError("There's no Text (Text Mesh Pro) component in children of choice button");
                            return;
                        }

                        // Add event when choice being clicked
                        choiceButton.onClick.AddListener(() => OnChoiceSelected(choice));
                    }

                    // Set Event System First Selected properties to the first index
                    EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
                }
                #endregion
            }
            else
                ExitDialogueMode();
        }

        void OnChoiceSelected(Choice choice)
        {
            // Select the chosen option and advance the story
            currentStory.ChooseChoiceIndex(choice.index);
            ProgressStory();

            // Activate input again
            allowInput = true;
        }

        public void RegressStory()
        {
            // Go back to the previous knot or stitch, if possible
            // if (history.Count > 0)
            // {
            //     int previousIndex = history.Pop();
                // currentStory.Something(previousIndex);
            // }
        }

        IEnumerator TypeText(TextMeshProUGUI textContainer, string text)
        {
            // Clear the text display
            textContainer.text = "";

            // Set is typing to true
            isTyping = true;

            // Display each character of the text one at a time
            foreach (char c in text)
            {
                textContainer.text += c;
                yield return new WaitForSeconds(m_DialogueTypingDelay);
            }

            // Stop typing
            isTyping = false;
        }
        #endregion
    }
}
