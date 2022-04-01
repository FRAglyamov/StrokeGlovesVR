using UnityEngine;

public class Mirror : MonoBehaviour
{
    // Remark: Maybe easier just leave On and Off option and just mirror the original hand (not 2 hand)?
    // Can be problems with some exercises. And if need to move hand to opposite side.
    public enum MirrorMode
    {
        None,
        Both,
        Opposite
    }

    public MirrorMode mirrorMode;

    [SerializeField]
    private Transform originTransform;
    [SerializeField]
    private Transform mirroredTransform;
    [SerializeField]
    private GameObject originHand;
    [SerializeField]
    private GameObject mirroredHand;

    private void Update()
    {
        if (mirrorMode == MirrorMode.None)
        {
            return;
        }
        mirroredTransform.position.Set(-originTransform.position.x, originTransform.position.y, originTransform.position.z);
    }

    public void ChangeMirrorMode(MirrorMode mirrorMode)
    {
        switch (mirrorMode)
        {
            case MirrorMode.None:
                mirroredHand.SetActive(false);
                originHand.SetActive(true);
                break;
            case MirrorMode.Both:
                mirroredHand.SetActive(true);
                originHand.SetActive(true);
                break;
            case MirrorMode.Opposite:
                mirroredHand.SetActive(true);
                originHand.SetActive(false);
                break;
            default:
                break;
        }
    }

}
