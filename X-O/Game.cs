using System;
using System.Collections.Generic;
class Game
{
    public enum Letter { X, O }

    public State currentState;
    Player player1, player2;

    public static void Main(string[] args)
    {
        Player player1 = new ConsolePlayer(Letter.X);
        Player player2 = new AIPlayer(Letter.O, Letter.X, 2);
        new Game(3, player1, player2).Play();
    }

    public Game(int girdSize, Player player1, Player player2)
    {
        this.currentState = new State(girdSize);
        this.player1 = player1;
        this.player2 = player2;
    }

    public int PlayerTurn(Player player)
    {
        player.DrawState(currentState);
        currentState = player.ChoiceNextState(currentState);
        if (currentState.isWinForLetter(player.MyLetter))
        {
            player1.ShowWinMessage();
            return -1;
        }
        return currentState.isTie() ? 0 : 1;
    }

    public void Play()
    {
        while (true)
        {
            if (PlayerTurn(player1) != 1) break;
            if (PlayerTurn(player2) != 1) break;
        }
    }

}


