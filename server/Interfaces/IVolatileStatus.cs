namespace server.Interfaces
{
    public interface IVolatileStatus
    {
        bool IsConfused { get; set; }
        bool IsFlinched { get; set; }

        void Reset();
        void DecrementConfusion();
    }
}
