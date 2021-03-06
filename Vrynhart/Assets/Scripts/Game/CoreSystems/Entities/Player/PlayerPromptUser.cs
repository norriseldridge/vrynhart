using UnityEngine;

public class PlayerPromptUser : PromptUser
{
    public override bool AcceptedPrompt => _accepted;
    bool _accepted = false;

    void Update()
    {
        _accepted = CustomInput.GetKeyDown(CustomInput.Interact);
    }
}
