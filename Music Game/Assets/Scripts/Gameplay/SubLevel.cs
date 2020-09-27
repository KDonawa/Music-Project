using UnityEngine;
using System.Collections.Generic;

namespace KD.MusicGame.Gameplay
{
    [CreateAssetMenu(fileName = "New SubLevel", menuName = "Game/SubLevel")]
    public class SubLevel : ScriptableObject
    {
        public string[] notes = null;
    }
}