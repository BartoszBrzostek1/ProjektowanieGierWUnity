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

    public override bool Equals(object obj)
    {
        if(obj.GetType()==typeof(Vector2))
        {
            Vector2 z = obj as Vector2;
            return z.x == this.x && z.y == this.y;
        }
        return false;
    }

}

public class Move
{
    public Vector2 Beginning_Move; //początek ruchu gracza
    public Vector2 End_Move; //koniec ruchu gracza
    public List<Move> Children; //lista możliwych ruchów do wykonania przez gracza
    public Move(Vector2 beginning_Move, Vector2 end_Move)
    {
        Beginning_Move = beginning_Move;
        End_Move = end_Move;
    }
    public override bool Equals(object obj) //przesłaniamy Equals
    {
        if(obj.GetType() == typeof(Move)) //sprawdzamy typ naszego obiektu 
        {
            Move z = obj as Move;
            return z.Beginning_Move.Equals(this.Beginning_Move) && z.End_Move.Equals(this.End_Move); //zwracamy ruchy
        }
        return false;
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

    public int Win() //wymóg wygranej
    {
        return (ListMoves.Count == 0 ? Controller.GetInstance().MarkedPlayerIndex : -1);
    }

    public void RegisterPawnPosition(DataPawn data, Vector2 position) //rejestrowanie pinoków 
    {
        Board[position.x, position.y] = data;
    }

    private List<Move> ListMoves;

    public void UpdateListMoves() //aktualizujemy liste możliwych ruchów
    {
        ListMoves = PossibleMoves(Board, Board_size, Controller.GetInstance().MarkedPlayer);
    }

    public static bool[,] QueenProperty(DataPawn[,] Board)
    {
        Vector2 size = new Vector2(Board.GetLength(0), Board.GetLength(1));
        bool[,] copy = new bool[size.x, size.y];
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (Board[x, y])
                {
                    copy[x, y] = Board[x, y].Queen;
                }
            }
        }
        return copy;
    }

    public static void RestoreOrginalProperty(DataPawn[,] Board, bool[,] copy)
    {
        Vector2 size = new Vector2(Board.GetLength(0), Board.GetLength(1));
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (Board[x, y])
                {
                    Board[x, y].Queen = copy[x, y];
                }
            }
        }
    }

    public Move IsMoves(Move selectedMove) //sprawdza czy zapsiane ruchy mają odpowedni typ
    {
        foreach (Move move in ListMoves)
        {
            if (selectedMove.Equals(move))
            {
                return move;
            }
        }
        return null;
    }

    static List<Vector2> CheckBorderNoKill(Vector2 field, DataPawn[,] Board, int BoardSize) //sprawdzamy niebijące ruchy na planszy uwzgedniając granice planszy
    {
        List<Vector2> list = new List<Vector2>();

        DataPawn pawn = Board[field.x, field.y];

        if (pawn.Queen == false)
        {
            Vector2 target = new Vector2(field.x + 1, field.y + pawn.Owner.Direction);
            if (target.x >= 0 && target.x < BoardSize && target.y >= 0 && target.y < BoardSize && Board[target.x, target.y] == null) //sprawdzanie granicy planszy
            {
                list.Add(target); //dodajemy do listy wyjątek, w którym pionek nie może bić
            }
            target = new Vector2(field.x - 1, field.y + pawn.Owner.Direction);
            if (target.x >= 0 && target.x < BoardSize && target.y >= 0 && target.y < BoardSize && Board[target.x, target.y] == null)
            {
                list.Add(target); //dodajemy do listy wyjątek, w którym pionek nie może bić
            }

        }
        else
        {
            bool leftFinished = false, rightFinished = false;
            for (int y = field.y + 1; y < BoardSize; y++)
            {
                int distance = Mathf.Abs(field.y - y);

                if (!leftFinished && field.x + distance < BoardSize && Board[field.x + distance, y] == null)
                {
                    list.Add(new Vector2(field.x + distance, y));
                }
                else
                {
                    leftFinished = true;
                }

                if (!rightFinished && field.x - distance >= 0 && Board[field.x - distance, y] == null)
                {
                    list.Add(new Vector2(field.x - distance, y));
                }
                else
                {
                    rightFinished = true;
                }
            }

            leftFinished = false;
            rightFinished = false;
            for (int y = field.y - 1; y >= 0; y--)
            {
                int distance = Mathf.Abs(field.y - y);

                if (!leftFinished && field.x + distance < BoardSize && Board[field.x + distance, y] == null)
                {
                    list.Add(new Vector2(field.x + distance, y));
                }
                else
                {
                    leftFinished = true;
                }

                if (!rightFinished && field.x - distance >= 0 && Board[field.x - distance, y] == null)
                {
                    list.Add(new Vector2(field.x - distance, y));
                }
                else
                {
                    rightFinished = true;
                }
            }
        }

        return list;
    }

    static List<Vector2> CheckBorderKill(Vector2 field, DataPawn[,] Board, int BoardSize) //funkcja do bicia pionków
    {
        List<Vector2> List = new List<Vector2>();
        DataPawn pawn = Board[field.x, field.y];
        if (pawn.Queen == false)
        {
            for (int y = -1; y <= 1; y += 2) // sprawdzamy możliwosci ruchu do okoła
            {
                for (int x = -1; x <= 1; x += 2)
                {
                    Vector2 target = new Vector2(field.x + x, field.y + y);
                    Vector2 behind_target = new Vector2(field.x + 2 * x, field.y + 2 * y);
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

        else
        {
            bool leftFinished = false, rightFinished = false;
            for (int y = field.y + 1; y < BoardSize - 1; y++)  //pętla po wszystkich y w góre
            {

                int targetX = field.x + Mathf.Abs(field.y - y);
                int behindX = targetX + 1;

                //warunki dla "w prawo"
                if (behindX < BoardSize && !leftFinished && Board[targetX, y] != null && Board[targetX, y].Owner != pawn.Owner) //jeżeli napotkaliśmy pionek do zbicia
                {
                    for (int yy = y + 1; yy < BoardSize; yy++)   //musimy do możliwych ruchów dodać wszystkie pola znajdujące się za pionkiem do zbicia, aż nie natrafimy na pole zajęte
                    {
                        behindX = field.x + Mathf.Abs(field.y - yy);
                        if (behindX >= BoardSize || Board[behindX, yy] != null)
                        {
                            break;
                        }
                        else
                        {
                            List.Add(new Vector2(behindX, yy));
                        }
                    }
                    leftFinished = true;
                }

                //warunki dla "w lewo"
                targetX = field.x - Mathf.Abs(field.y - y);
                behindX = targetX - 1;
                if (behindX >= 0 && !rightFinished && Board[targetX, y] != null && Board[targetX, y].Owner != pawn.Owner)
                {
                    for (int yy = y + 1; yy < BoardSize; yy++)
                    {
                        behindX = field.x - Mathf.Abs(field.y - yy);
                        if (behindX < 0 || Board[behindX, yy] != null)
                        {
                            break;
                        }
                        else
                        {
                            List.Add(new Vector2(behindX, yy));
                        }
                    }
                    leftFinished = true;
                }

            }

            leftFinished = false;
            rightFinished = false;

            for (int y = field.y - 1; y > 0; y--) //pętla po wszystkich y w dół
            {
                int targetX = field.x + Mathf.Abs(field.y - y);
                int behindX = targetX + 1;

                //warunki dla "w prawo"
                if (behindX < BoardSize && !leftFinished && Board[targetX, y] != null && Board[targetX, y].Owner != pawn.Owner)
                {
                    for (int yy = y - 1; yy >= 0; yy--)
                    {
                        behindX = field.x + Mathf.Abs(field.y - yy);
                        if (behindX >= BoardSize || Board[behindX, yy] != null)
                        {
                            break;
                        }
                        else
                        {
                            List.Add(new Vector2(behindX, yy));
                        }
                    }
                    leftFinished = true;
                }

                //warunki dla "w lewo"
                targetX = field.x - Mathf.Abs(field.y - y);
                behindX = targetX - 1;
                if (behindX >= 0 && !rightFinished && Board[targetX, y] != null && Board[targetX, y].Owner != pawn.Owner)
                {
                    for (int yy = y - 1; yy >= 0; yy--)
                    {
                        behindX = field.x - Mathf.Abs(field.y - yy);
                        if (behindX < 0 || Board[behindX, yy] != null)
                        {
                            break;
                        }
                        else
                        {
                            List.Add(new Vector2(behindX, yy));
                        }
                    }
                    leftFinished = true;
                }
            }
        }


        return List;
    }

    static void ToMove(Vector2 Beginning_Move, Vector2 End_Move, DataPawn[,] Board, System.Action<DataPawn, Vector2> moveAction = null, System.Action<DataPawn> deleteAction = null, System.Action<DataPawn> promoteAction = null) //symulacja możliwych ruchów (callback)
    {
        DataPawn marked = Board[Beginning_Move.x, Beginning_Move.y];
        Vector2 step = new Vector2((End_Move.x - Beginning_Move.x) / Mathf.Abs(End_Move.x - Beginning_Move.x), (End_Move.y - Beginning_Move.y) / Mathf.Abs(End_Move.y - Beginning_Move.y));
        Debug.Assert(marked != null);

        if (marked.Queen == false)
        {

            if (Mathf.Abs(End_Move.y - Beginning_Move.y) > 1)
            {
                DataPawn killed = Board[Beginning_Move.x + step.x, Beginning_Move.y + step.y];
                Board[Beginning_Move.x + step.x, Beginning_Move.y + step.y] = null;
                if (deleteAction != null)
                {
                    deleteAction(killed);
                }
            }

            Board[End_Move.x, End_Move.y] = marked;
            Board[Beginning_Move.x, Beginning_Move.y] = null;
            if (moveAction != null)
            {
                moveAction(Board[End_Move.x, End_Move.y], End_Move);
            }

            //sprawdzamy promocje na queen
            if ((End_Move.y == 0 && marked.Owner.Direction == -1) || (End_Move.y == Board.GetUpperBound(0) && marked.Owner.Direction == 1))
            {
                marked.Queen = true;
                if (promoteAction != null)
                {
                    promoteAction(marked);
                }
            }
        }
        else
        {
            //sprawdza wrogów na na plnszy na drodze
            DataPawn killed;
            for (int i = 1; i < Mathf.Abs(End_Move.y - Beginning_Move.y); i++)
            {

                if ((killed = Board[Beginning_Move.x + i * step.x, Beginning_Move.y + i * step.y]) != null && killed.Owner != marked.Owner)
                {
                    Board[Beginning_Move.x + i * step.x, Beginning_Move.y + i * step.y] = null;
                    if (deleteAction != null)
                    {
                        deleteAction(killed);
                    }
                    break;
                }

            }

            Board[End_Move.x, End_Move.y] = marked;
            Board[Beginning_Move.x, Beginning_Move.y] = null;

            if (moveAction != null)
            {
                moveAction(Board[End_Move.x, End_Move.y], End_Move);
            }

        }
    }

    static int ComboCounter(Vector2 position, DataPawn[,] Board, int BoardSize, int counter, Move move) //długość możliwego wielokrotnego bicia
    {
        int lenghtCombo = 1;
        List<Vector2> captured = CheckBorderKill(position, Board, BoardSize);
        if (captured.Count == 0) //kiedy kombinacja nie może zbić kolejnego pionka
        {
            move.Children = null;
            return counter; //koniec kombosa
        }
        else
        {
            move.Children = new List<Move>();
            foreach (Vector2 target in captured) //dla każdego pionka do zbicia wykonujemy kolejny ruch
            {
                Move child = new Move(position, target);
                DataPawn[] clone = CopyLine(Board, position, target);
                bool[,] queenData = DataGameScript.QueenProperty(Board);
                ToMove(position, target, Board); //tworzenie kopi planszy po wykonaniu ruchu
                int newComboCounter = ComboCounter(target, Board, BoardSize, counter + 1, child); //pole po ruchu
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
                RestoreLine(Board, clone, position, target);
                DataGameScript.RestoreOrginalProperty(Board, queenData);

            }
            //wykonywanie się kombosa
        }


        return lenghtCombo;
    }

    public static DataPawn[] CopyLine(DataPawn[,] Board, Vector2 beginning_Move, Vector2 end_Move)
    {
        DataPawn[] data = new DataPawn[Mathf.Abs(beginning_Move.y - end_Move.y) + 1];
        Vector2 dir = new Vector2((end_Move.x - beginning_Move.x) / Mathf.Abs(end_Move.x - beginning_Move.x),
            (end_Move.y - beginning_Move.y) / Mathf.Abs(end_Move.y - beginning_Move.y));
        for (int g = 0; g < data.GetLength(0); g++)
        {
            data[g] = Board[beginning_Move.x + dir.x * g, beginning_Move.y + dir.y * g];
        }
        return data;
    } //kopiowanie fragmentu planszy (kopiujemy linie ruchu pionka)

    public static void RestoreLine(DataPawn[,] Board, DataPawn[] copy, Vector2 beginning_Move, Vector2 end_Move)
    {
        Vector2 dir = new Vector2((end_Move.x - beginning_Move.x) / Mathf.Abs(end_Move.x - beginning_Move.x),
            (end_Move.y - beginning_Move.y) / Mathf.Abs(end_Move.y - beginning_Move.y));
        for (int f = 0; f < copy.GetLength(0); f++)
        {
            Board[beginning_Move.x + dir.x * f, beginning_Move.y + dir.y * f] = copy[f];
        }
    }

    private static List<Move> PossibleMoves(DataPawn[,] Board, int BoardSize, Player player) //zwracamy liste ruchów
    {
        List<Move> List = new List<Move>();
        bool[,] copy = QueenProperty(Board); //zapisujemy dane pionków, ponieważ mogą ulec zmianie w trakcie rekurencyjnego szukania ruchów
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

                        //List<Vector2> possibleKills = new List<Vector2>();
                        foreach (Vector2 target in captured) //wszystkie znalezione ruchy bijące
                        {
                            Move move = new Move(pawnPosition, target); //tworzymy ruch z pozycji początkowej do pozycji bijącej
                            DataPawn[] cloneBoard = CopyLine(Board, pawnPosition, target);
                            bool[,] queenData = DataGameScript.QueenProperty(Board);
                            ToMove(pawnPosition, target, Board);
                            int pawnCombo = ComboCounter(target, Board, BoardSize, 1, move); //sprawdzamy czy jest jakieś combo do wykonania
                            RestoreLine(Board, cloneBoard, pawnPosition, target);
                            DataGameScript.RestoreOrginalProperty(Board, queenData);
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

        RestoreOrginalProperty(Board, copy); //po symulacji ruchu przywracamy orginalne dane pionkom
        return List;
    }

    public Vector2 PawnFiled(DataPawn pawn) //sprawdzamy na którym polu referencja pionka jest równa referencji pionka w tablicy
    {
        for (int y = 0; y < Board_size; y++)
        {
            for (int x = 0; x < Board_size; x++)
            {
                if (pawn == Board[x, y])
                {
                    return new Vector2(x, y); //przypisujemy pionka na jego pole
                }
            }
        }
        throw new System.Exception("Pionek nie jest na planszy");
    }

    public bool ToMovePawn(Move move)
    {
        ToMove(move.Beginning_Move, move.End_Move, Board, ViewBoard.GetInstance().MovePawn, ViewBoard.GetInstance().DeletePawn, ViewBoard.GetInstance().PromotePawn);
        if (move.Children == null || move.Children.Count == 0) //jeśli nie ma tablicy Children, lub jest ona pusta...
        {
            return false;
        } //to nie ma następstw ruchów danego pionka, którym wybraliśmy, wiec kończymy ture
        else
        {
            ListMoves = move.Children; //aktualizujemy naszą listę ruchów
            return true;
        }
    }
     


    public void Initializet()
    {
        Board = new DataPawn[Board_size, Board_size];
    }


}
