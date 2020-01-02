using System;
using System.Collections.Generic;

namespace Game.Interfaces
{
    public interface IGame
    {
        IGameWorld GameWorld { get; set; }

        List<IPlayer> Players { get; set; }

        Action LevelComplete { get; set; }
    }
}
