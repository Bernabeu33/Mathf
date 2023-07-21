namespace GameLoader
{
    public interface IAsyncObject 
    {
        object AsyncResult { get; }
        
        bool IsCompleted { get; }

        bool IsError { get; }
        
        string AsyncMessage { get; }

        bool IsSuccess { get; }
    }
}


