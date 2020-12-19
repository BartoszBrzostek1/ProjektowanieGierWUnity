using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Vector2 //Wektor dwuwymiarowy
{
    public int x;
    public int y;

    public Vector2()
    {

    }

    public Vector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

}

public class Move
{
    public Vector2 Beginning_Move; //początek ruchu gracza
    public Vector2 End_Move; //koniec ruchu gracza
    public List<Move> Children; //lista możliwych ruchów do wykonania przez gracza
    public Move(Vector2 Beginning_Move, Vector2 End_Move)
    {
        this.Beginning_Move = Beginning_Move;
        this.End_Move = End_Move;
    }
}

public class DataGameScript : MonoBehaviour
{

    public int Board_size = 10; //Wymiary Warcabnicy = 10x10
    public int BeginRows = 4; //ilość rzędów ustwień pionków graczy na początku gry = 4
    private static DataGameScript instance = null;

    public static DataGameScript GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindGameObjectWithTag("DataGameScript").GetComponent<DataGameScript>();
        }
        return instance;
    }

    public DataPawn[,] Board;
    public DataPawn[,] board //dwuwymiarowa tablica do trzymania pozycji pionków (referencje)
    {
        get
        {
            return Board; //akcesor 
        }
    }

    public void RegisterPawnPosition(DataPawn data, Vector2 position) //rejestrowanie pinoków 
    {
        Board[position.x, position.y] = data;
    }

    static List<Vector2> CheckBorderNoKill(Vector2 filed, DataPawn[,] Board, int BoardSize) //sprawdzamy niebijące ruchy na planszy uwzgedniając granice planszy
    {
        List<Vector2> list = new List<Vector2>();

        DataPawn pawn = Board[filed.x, filed.y];

        if (pawn.Queen == false)
        {
            Vector2 target = new Vector2(filed.x + 1, filed.y + pawn.Owner.Direction);
            if (target.x >= 0 && target.x < BoardSize && target.y >= 0 && target.y < BoardSize && Board[target.x, target.y] == null) //sprawdzanie granicy planszy
            {
                list.Add(target); //dodajemy do listy wyjątek, w którym pionek nie może bić
            }
            target = new Vector2(filed.x - 1, filed.y + pawn.Owner.Direction);
            if (target.x >= 0 && target.x < BoardSize && target.y >= 0 && target.y < BoardSize && Board[target.x, target.y] == null)
            {
                list.Add(target); //dodajemy do listy wyjątek, w którym pionek nie może bić
            }

        }
        else
        {
            throw new System.Exception("Jeszcze nic tutaj nie ma");
        }

        return list;
    }

    static List<Vector2> CheckBorderKill(Vector2 filed, DataPawn[,] Board, int BoardSize) //funkcja do bicia pionków
    {
        List<Vector2> List = new List<Vector2>();
        DataPawn pawn = Board[filed.x, filed.y];
        if (pawn.Queen == false)
        {
            for (int y = -1; y < 1; y++) // sprawdzamy możliwosci ruchu do okoła
            {
                for (int x = -1; x <= 1; x += 2)
                {
                    Vector2 target = new Vector2(filed.x + x, filed.y + y);
                    Vector2 behind_target = new Vector2(filed.x + 2 * x, filed.y + 2 * y);
                    if (behind_target.x >= 0 && behind_target.x < BoardSize && behind_target.y >= 0 && behind_target.y < BoardSize)
                    { //sprawdzamy, czy dane pole jest zajęte
                        DataPawn targetPawn = Board[target.x, target.y];
                        if (targetPawn != null && targetPawn.Owner != pawn.Owner && Board[behind_target.x, behind_target.y] == null) //warunki do tego aby nie bic swoich pionków
                        {
                            List.Add(behind_target);
                        }
                    }
                }
            }
        }


        return List;
    }

    static void ToMove(Vector2 Beginning_Move, Vector2 End_Move, DataPawn[,] Board) //symulacja możliwych ruchów
    {
        Vector2 step = new Vector2(Beginning_Move.x - End_Move.x, Beginning_Move.y - End_Move.y);
        if (Mathf.Abs(step.y) > 1) //warunek do biciam jeśli przesuneliśmy się o więcej niż jedno pole to znaczy, że pionek zbił
        {
            if (Board[Beginning_Move.x, Beginning_Move.y].Queen == false) //jeśli pionek nie jest królową
            {
                DataPawn killed = Board[Beginning_Move.x + (step.x) / Mathf.Abs(step.x), Beginning_Move.y + (step.y) / Mathf.Abs(step.y)]; //zbity pionek
                if (killed != null) //jeśli zbijemy pionek
                {
                    Board[Beginning_Move.x + (step.x) / Mathf.Abs(step.x), Beginning_Move.y + (step.y) / Mathf.Abs(step.y)] = null; //usuwanie zbitego pionka
                    Board[End_Move.x, End_Move.y] = Board[Beginning_Move.x, Beginning_Move.y];
                    Board[Beginning_Move.x, Beginning_Move.y] = null; //przesuwamy pionek bijący

                }
            }
            else
            {
                throw new System.Exception("Jeszcze nie dodane");
            }
        }
        else //jeśli nie bijemy
        {
            Board[End_Move.x, End_Move.y] = Board[Beginning_Move.x, Beginning_Move.y];
            Board[Beginning_Move.x, Beginning_Move.y] = null; //...to po prostu przesuwamy pionek
        }
    }

    static int ComboCounter(Vector2 position, DataPawn[,] Board, int BoardSize, int counter, Move move) //długość możliwego wielokrotnego bicia
    {
        int lenghtCombo = 1;
        List<Vector2> captured = CheckBorderKill(position, Board, BoardSize);
        if (captured.Count == 0) //kiedy kombinacja nie może zbić kolejnego pionka
        {
            move.Children = null;
            return 1; //koniec kombosa
        }
        else
        {
            foreach (Vector2 target in captured) //dla każdego pionka do zbicia wykonujemy kolejny ruch
            {
                Move child = new Move(position, target);
                DataPawn[,] clone = Board.Clone() as DataPawn[,];
                ToMove(position, target, clone); //tworzenie kopi planszy po wykonaniu ruchu
                int newComboCounter = ComboCounter(target, clone, BoardSize, counter + 1, child); //pole po ruchu
                if (newComboCounter > lenghtCombo)
                {
                    move.Children.Clear();
                    move.Children.Add(child);
                    lenghtCombo = newComboCounter;
                }
                else if (newComboCounter == lenghtCombo)
                {
                    move.Children.Add(child);
                }

            }
            //wykonywanie się kombosa
        }


        return lenghtCombo;
    }

    public static List<Move> PossibleMoves(DataPawn[,] Board, int BoardSize, Player player) //zwracamy liste ruchów
    {
        List<Move> List = new List<Move>();
        bool foundMove = false;
        int maxCombo = 0;

        for (int y = 0; y < BoardSize; y++)
        {
            for (int x = 0; x < BoardSize; x++) //przechodzimy sobie po każdych pionkach
            {
                DataPawn pawnMove;
                if ((pawnMove = Board[x, y]) != null && pawnMove.Owner == player) //wybieramy aktualny pionek
                {
                    Vector2 pawnPosition = new Vector2(x, y);
                    List<Vector2> captured = CheckBorderKill(pawnPosition, Board, BoardSize); //sprawdzamy czy są pioni do bicia
                    if (captured.Count == 0 && foundMove == false) //jeśli nie ma pionków do bicia
                    {
                        List<Vector2> noKill = CheckBorderNoKill(pawnPosition, Board, BoardSize); //szukamy ruchy niebijące
                        foreach (Vector2 target in noKill) // dodajemy ruchy do możkiwych do wykonania
                        {
                            List.Add(new Move(pawnPosition, target));
                        }
                    }
                    else //jeśli są pionki do bicia
                    {
                        foundMove = true;

                        List<Vector2> possibleKills = new List<Vector2>();
                        foreach (Vector2 target in captured) //wszystkie znalezione ruchy bijące
                        {
                            Move move = new Move(pawnPosition, target); //tworzymy ruch z pozycji początkowej do pozycji bijącej
                            DataPawn[,] cloneBoard = Board.Clone() as DataPawn[,];
                            ToMove(pawnPosition, target, cloneBoard);
                            int pawnCombo = ComboCounter(target, cloneBoard, BoardSize, 0, move); //sprawdzamy czy jest jakieś combo do wykonania
                            if (pawnCombo > maxCombo) //jeśli znalazło się wieksze combo od aktualnego
                            {
                                List.Clear(); //czyścimy liste ruchów
                                maxCombo = pawnCombo;
                                List.Add(move); //...aby tylko móc wykona ruch o najwyższej wartości combo
                            }
                            else if (pawnCombo == maxCombo) //jeśli są równe
                            {
                                List.Add(move); //to po prostu dodajemy dany ruch do listy możliwych ruchów
                            }
                        }
                    }
                }
            }
        }


        return List;
    }

    void Start()
    {
        Board = new DataPawn[Board_size, Board_size];
    }


    void Update()
    {
        
    }

}
