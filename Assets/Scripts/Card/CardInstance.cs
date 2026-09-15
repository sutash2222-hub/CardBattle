using System;

[Serializable]
public class CardInstance
{
    public CardData baseData;
    public int currentPower;
    public bool isOnBoard;
    public bool isRevealed;

    public CardInstance(CardData data)
    {
        baseData = data;
        currentPower = data.power;
        isOnBoard = false;
        isRevealed = false;
    }

    public void ResetPower()
    {
        currentPower = baseData.power;
    }
}
