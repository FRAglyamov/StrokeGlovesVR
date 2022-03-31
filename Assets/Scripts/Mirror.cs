using UnityEngine;

public class Mirror : MonoBehaviour
{
    // Remark: Maybe easier just leave On and Off option and just mirror the original hand (not 2 hand)?
    public enum MirrorMode
    {
        None,
        Both,
        Opposite
    }

    public MirrorMode mirrorMode;

    [SerializeField]
    private Transform _originTransform;
    [SerializeField]
    private Transform _mirorredTransform;

    private void Update()
    {
        if (mirrorMode == MirrorMode.None)
        {
            return;
        }
        _mirorredTransform.position.Set(-_originTransform.position.x, _originTransform.position.y, _originTransform.position.z);
    }

    public void ChangeMirrorMode(MirrorMode mirrorMode)
    {
        switch (mirrorMode)
        {
            case MirrorMode.None:
                // Disable opposite hand object
                _mirorredTransform.gameObject.SetActive(false);
                // Enable origin hand object
                _originTransform.gameObject.SetActive(true);
                break;
            case MirrorMode.Both:
                // Enable both hand objects
                _mirorredTransform.gameObject.SetActive(true);
                _originTransform.gameObject.SetActive(true);
                break;
            case MirrorMode.Opposite:
                // Enable opposite hand object
                _mirorredTransform.gameObject.SetActive(true);
                // Disable origin hand object
                _originTransform.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

}
