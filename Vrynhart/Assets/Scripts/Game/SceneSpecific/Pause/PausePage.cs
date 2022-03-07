using UnityEngine;

public class PausePage : MonoBehaviour
{
    public virtual void Initialize(PlayerController player) { }

    protected virtual void Update()
    {
        if (CustomInput.IsController())
        {
            if (CustomInput.GetKeyDown(CustomInput.Cancel))
                gameObject.SetActive(false);
        }
    }
}
