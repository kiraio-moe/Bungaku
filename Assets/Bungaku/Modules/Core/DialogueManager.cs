using Ink.Runtime;
using Bungaku.Utility;
using TMPro;
using UnityEngine;

namespace Bungaku.Core
{
    [AddComponentMenu("Bungaku/Managers/Dialogue Manager")]
    public class DialogueManager : MonoSingleton<DialogueManager>
    {
        [Header("UI")]
        [SerializeField] TextMeshProUGUI m_DialogueText;

        [Header("Story")]
        [SerializeField] bool m_Autoplay;
        [SerializeField] TextAsset m_Story;

        Story _currentStory;
        // bool _dialogueIsPlaying;

        new void Awake()
        {
            if (m_DialogueText == null) return;
        }

        void Start()
        {
            if (m_Autoplay)
            {
                if (m_Story != null)
                    EnterDialogueMode(m_Story);
            }
        }

        public void EnterDialogueMode(TextAsset story)
        {
            _currentStory = new Story(story.text);
            // _dialogueIsPlaying = true;

            ContinueStory();
        }

        void ExitDialogueMode()
        {
            // _dialogueIsPlaying = false;
            m_DialogueText.text = string.Empty;
        }

        public void ContinueStory()
        {
            if (_currentStory.canContinue)
                m_DialogueText.text = _currentStory.Continue();
            else
                ExitDialogueMode();
        }
    }
}
