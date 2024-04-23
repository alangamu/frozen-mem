using System;

namespace Assets.Scripts
{
    [Serializable]
    public class TileController
    {
        public int TileIndex { get; private set; }
        public string PlayerId { get; private set; }
        public int TileId { get; private set; }
        public bool IsDone { get; private set; }

        public TileController(int tileId, int tileIndex, string playerId)
        {
            TileIndex = tileIndex;
            TileId = tileId;
            IsDone = false;
            PlayerId = playerId;
        }

        public void SetIsDone(bool isDone)
        {
            IsDone = isDone;
        }
    }
}