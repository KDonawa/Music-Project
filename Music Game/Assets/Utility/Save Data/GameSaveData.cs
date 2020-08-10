[System.Serializable]
public class GameSaveData
{
    public bool isNewGame = true;

    public GameSaveData()
    {
        isNewGame = GameManager.Instance.isNewGame;
    }
}