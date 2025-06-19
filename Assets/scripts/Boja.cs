using UnityEngine;

public static class Boja
{
    public enum boje
    {
        CRVENA,
        PLAVA,
        ZUTA,
    }

    public static Color GetBojaFromEnum(boje b)
    {
        switch (b)
        {
            case boje.CRVENA:
                return Color.red;
            case boje.PLAVA:
                return Color.blue;
            case boje.ZUTA:
                return Color.yellow;
            default:
                return Color.black;
        }
    }

    public static boje GetRandomBoja()
    {
        boje[] allBoje = (boje[])System.Enum.GetValues(typeof(boje));
        int randomIndex = Random.Range(0, allBoje.Length);
        return allBoje[randomIndex];
    }
}
