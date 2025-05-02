using Unity.Netcode.Components;


public class clientNetwork : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
