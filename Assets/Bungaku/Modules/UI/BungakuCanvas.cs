using Bungaku.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bungaku.UI
{
    [AddComponentMenu("Bungaku/UI/Bungaku Canvas")]
    public class BungakuCanvas : MonoBehaviourSingleton<BungakuCanvas>
    {
        [Header("Canvases")]

        [Tooltip("Scenery background canvas")]
        [SerializeField] public Canvas m_SceneryCanvas;

        [Tooltip("Canvas to place characters")]
        [SerializeField] public Canvas m_CharacterCanvas;

        [Tooltip("Dialogue UI canvas")]
        [SerializeField] public Canvas m_DialogueCanvas;

        [Tooltip("Canvas to store choices")]
        [SerializeField] public Canvas m_ChoicesCanvas;

        [Header("Dialogue UI Placeholders")]

        [Tooltip("Speaker name placeholder")]
        [SerializeField] public TextMeshProUGUI m_SpeakerName;

        [Tooltip("Dialogue text placeholder")]
        [SerializeField] public TextMeshProUGUI m_DialogueText;

        [Tooltip("Choice button prefab")]
        [SerializeField] public Button m_ChoicePrefab;

        new void Awake()
        {
            this.Persistent = true;
            base.Awake();
        }
    }
}
