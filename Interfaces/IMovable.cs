namespace GameProject.Interfaces
{
    internal interface IMovable
    {
        bool IsMovingUp { get; set; }
        bool IsMovingLeft { get; set; }
        bool IsMovingDown { get; set; }
        bool IsMovingRight { get; set; }
        float RotationAngle { get; set; }
        int Speed { get; set; }
        void Move();
    }
}
