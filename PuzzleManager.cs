using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    private PipeRotator[] pipes;
    private PipeRotator[,] grid = new PipeRotator[4, 4];

    private bool puzzleCompleted = false;

    [Header("Interface")]
    public GameObject winPanel;

    private const int rows = 4;
    private const int columns = 4;

    void Start()
    {
        pipes = GetComponentsInChildren<PipeRotator>();

        // Remplit la grille grâce aux Row / Column
        foreach (PipeRotator pipe in pipes)
        {
            grid[pipe.row, pipe.column] = pipe;
        }



        if (winPanel != null)
            winPanel.SetActive(false);

        // Mélange le puzzle
        foreach (PipeRotator pipe in pipes)
            pipe.Shuffle();

        // Si le puzzle est déjà résolu après le mélange,
        // on mélange à nouveau
        while (HasValidPath())
        {
            foreach (PipeRotator pipe in pipes)
                pipe.Shuffle();
        }

        CheckPuzzle();
    }


    public void CheckPuzzle()
    {
        if (puzzleCompleted)
            return;

        PipeRotator start = grid[0, 0];
        PipeRotator end = grid[3, 3];

        if (start == null || end == null)
        {
            Debug.LogWarning("Entrée ou sortie introuvable !");
            return;
        }

        HashSet<PipeRotator> visited = new HashSet<PipeRotator>();
        Queue<PipeRotator> queue = new Queue<PipeRotator>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            PipeRotator current = queue.Dequeue();

            // On a atteint Pipe_16
            if (current == end)
            {
                PuzzleCompleted();
                return;
            }

            int row = current.row;
            int column = current.column;

            // HAUT
            TryVisit(
                current,
                row - 1,
                column,
                PipeRotator.Direction.Up,
                PipeRotator.Direction.Down,
                visited,
                queue
            );

            // DROITE
            TryVisit(
                current,
                row,
                column + 1,
                PipeRotator.Direction.Right,
                PipeRotator.Direction.Left,
                visited,
                queue
            );

            // BAS
            TryVisit(
                current,
                row + 1,
                column,
                PipeRotator.Direction.Down,
                PipeRotator.Direction.Up,
                visited,
                queue
            );

            // GAUCHE
            TryVisit(
                current,
                row,
                column - 1,
                PipeRotator.Direction.Left,
                PipeRotator.Direction.Right,
                visited,
                queue
            );
        }
    }


    private void TryVisit(
        PipeRotator current,
        int neighbourRow,
        int neighbourColumn,
        PipeRotator.Direction direction,
        PipeRotator.Direction oppositeDirection,
        HashSet<PipeRotator> visited,
        Queue<PipeRotator> queue)
    {
        // En dehors de la grille
        if (neighbourRow < 0 || neighbourRow >= rows ||
            neighbourColumn < 0 || neighbourColumn >= columns)
        {
            return;
        }

        PipeRotator neighbour =
            grid[neighbourRow, neighbourColumn];

        if (neighbour == null)
            return;

        if (visited.Contains(neighbour))
            return;

        bool currentOpen =
            current.HasConnection(direction);

        bool neighbourOpen =
            neighbour.HasConnection(oppositeDirection);

        if (currentOpen && neighbourOpen)
        {

            visited.Add(neighbour);
            queue.Enqueue(neighbour);
        }
    }

    private bool HasValidPath()
    {
        PipeRotator start = grid[0, 0];
        PipeRotator end = grid[3, 3];

        if (start == null || end == null)
            return false;

        HashSet<PipeRotator> visited = new HashSet<PipeRotator>();
        Queue<PipeRotator> queue = new Queue<PipeRotator>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            PipeRotator current = queue.Dequeue();

            if (current == end)
                return true;

            int row = current.row;
            int column = current.column;

            TryVisit(
                current,
                row - 1,
                column,
                PipeRotator.Direction.Up,
                PipeRotator.Direction.Down,
                visited,
                queue
            );

            TryVisit(
                current,
                row,
                column + 1,
                PipeRotator.Direction.Right,
                PipeRotator.Direction.Left,
                visited,
                queue
            );

            TryVisit(
                current,
                row + 1,
                column,
                PipeRotator.Direction.Down,
                PipeRotator.Direction.Up,
                visited,
                queue
            );

            TryVisit(
                current,
                row,
                column - 1,
                PipeRotator.Direction.Left,
                PipeRotator.Direction.Right,
                visited,
                queue
            );
        }

        return false;
    }


    private void PuzzleCompleted()
    {
        puzzleCompleted = true;

        if (winPanel != null)
            winPanel.SetActive(true);
    }
}