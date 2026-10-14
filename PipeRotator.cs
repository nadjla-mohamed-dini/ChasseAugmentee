using UnityEngine;
using UnityEngine.InputSystem;

public class PipeRotator : MonoBehaviour
{
    public enum PipeType
    {
        Straight,
        Corner,
        T,
        End
    }
  

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    [Header("Type de tuyau")]
    public PipeType pipeType;

    [Header("Position dans la grille")]
    public int row;
    public int column;


    private void Update()
    {
        // Clic souris
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckHit(Mouse.current.position.ReadValue());
        }

        // Touch écran
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            CheckHit(
                Touchscreen.current.primaryTouch.position.ReadValue()
            );
        }
    }


    private void CheckHit(Vector2 screenPosition)
    {
        Camera cam = Camera.main;

        if (cam == null)
            return;

        Ray ray = cam.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                RotatePipe();
            }
        }
    }


    private void RotatePipe()
    {
        transform.Rotate(0f, 90f, 0f);

        PuzzleManager manager =
            GetComponentInParent<PuzzleManager>();

        if (manager != null)
        {
            manager.CheckPuzzle();
        }
    }


    public void Shuffle()
    {
        int randomTurns = Random.Range(0, 4);

        transform.Rotate(
            0f,
            randomTurns * 90f,
            0f
        );
    }


    public bool HasConnection(Direction direction)
    {
        int rotation =
            Mathf.RoundToInt(transform.localEulerAngles.y / 90f) % 4;

        


        // =========================
        // STRAIGHT
        // =========================
        if (pipeType == PipeType.Straight)
        {
            // Y = 0° ou 180° : vertical
            if (rotation == 0 || rotation == 2)
            {
                return direction == Direction.Up ||
                    direction == Direction.Down;
            }

            // Y = 90° ou 270° : horizontal
            return direction == Direction.Left ||
                direction == Direction.Right;
        }


        // =========================
        // CORNER
        // =========================
        if (pipeType == PipeType.Corner)
        {
            // Décalage de 180° pour correspondre au visuel
            int cornerRotation = (rotation + 2) % 4;

            switch (cornerRotation)
            {
                case 0:
                    return direction == Direction.Up ||
                        direction == Direction.Right;

                case 1:
                    return direction == Direction.Down ||
                        direction == Direction.Right;

                case 2:
                    return direction == Direction.Down ||
                        direction == Direction.Left;

                case 3:
                    return direction == Direction.Up ||
                        direction == Direction.Left;
            }
        }


        // =========================
        // T
        // =========================
        if (pipeType == PipeType.T)
        {
            switch (rotation)
            {
                // Y = 0° : Haut + Gauche + Droite
                case 0:
                    return direction != Direction.Down;

                // Y = 90° : Haut + Bas + Gauche
                case 1:
                    return direction != Direction.Right;

                // Y = 180° : Bas + Gauche + Droite
                case 2:
                    return direction != Direction.Up;

                // Y = 270° : Haut + Bas + Droite
                case 3:
                    return direction != Direction.Left;
            }
        }


        // =========================
        // END
        // =========================
        if (pipeType == PipeType.End)
        {
            switch (rotation)
            {
                // Y = 0°
                case 0:
                    return direction == Direction.Up;

                // Y = 90°
                case 1:
                    return direction == Direction.Left;

                // Y = 180°
                case 2:
                    return direction == Direction.Down;

                // Y = 270°
                case 3:
                    return direction == Direction.Right;
            }
        }

        return false;
    }
}