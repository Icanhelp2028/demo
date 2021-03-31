namespace Core.Common.Services
{
    /// <summary>流水号生成</summary>
    public interface ISequenceService
    {
        /// <summary>由时间生成的唯一流水号</summary>
        long GenerateSequence(int count = 1);

        /// <summary>由递增生成的唯一id</summary>
        long GenerateIdentity(int count = 1);
    }
}