using UnityEngine;

public class P41001 : Pray
{
    public GameObject P41001VFX;
    public override void CastPray()
    {
        PhaseManager.Instance.OnProtectedBy41001 = true;

        GameObject go = Instantiate(P41001VFX);
        Destroy(go, 3f);
        Destroy(gameObject, 3f);
    }
}
