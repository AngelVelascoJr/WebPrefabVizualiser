using UnityEngine;
using VRChatMigration;

public class HapticFeedback : MonoBehaviour
{
    [VRChatLegacy("VRC_Pickup", "Interactive pickup")]
    [SerializeField] string _legacyVrcPickup;

    [SerializeField] private float hapticDuration = 0.05f;
    [SerializeField] private float hapticAmplitudeDesbaste = 0.5f;
    [SerializeField] private float hapticFrequencyDesbaste = 200f;
    [SerializeField] private float hapticAmplitudePulido = 0.2f;
    [SerializeField] private float hapticFrequencyPulido = 50f;
    [SerializeField] private float Desgaste = 0f;

    private void Update()
    {
        var probe = gameObject.GetComponent<ProbeBehabiour>();
        if (probe != null)
            Desgaste = probe.Desgaste;
    }

    // VRChat: PlayHapticEventInHand
    public void hapticFeedbackDesbaste() { }

    public void hapticFeedbackPulido() { }

    public void hapticFeedbackCotton() { }
}
